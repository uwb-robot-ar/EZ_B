using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;         // Used to implement asynchronous methods
using Windows.Devices.Sensors;        // Orientation sensor is used to rotate the camera preview
using Windows.Graphics.Display;       // Used to determine the display orientation
using Windows.Graphics.Imaging;       // Used for encoding captured images
using Windows.Media;                  // Provides SystemMediaTransportControls
using Windows.Media.MediaProperties;  // Used for photo and video encoding
using Windows.Storage.FileProperties; // Used for image file encoding
using Windows.Storage.Streams;        // General file I/O
using Windows.System.Display;         // Used to keep the screen awake during preview and capture
using Windows.UI.Core;                // Used for updating UI from within async operations
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EZ_B
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        EZB _ezb;
        EZBv4Video _video;
        WriteableBitmap bm;
        DispatcherTimer _timer = new DispatcherTimer();
        
        /*
         * hold the direction of movement info here
         */
        private const int SURGE = 0;
        private const int YAW = 1;
        private double[] move_vec = {0, 0};
        



        public MainPage()
        {
            this.InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;
            Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = 1200, Height = 600 });

            neck_slider.Value = 90;
            neck_nod.Value = 45;
        }


        private async void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            
            if ((Button)sender == connectButton)
            {
                _ezb = new EZB();
                await _ezb.Connect("192.168.1.1");

                if (!_ezb.IsConnected)
                {
    
                    connectStatus.Text = "NOT CONNECTED!";

                    return;
                }

                if (_ezb.IsConnected)
                {
                    _timer.Start();
                    connectStatus.Text = "CONNECTED!";


                    _video = new EZBv4Video();
                   
                    _video.OnImageDataReady += Video_OnImageDataReady;

                    errorStatus.Text = _ezb.GetLastErrorMsg;

                    await _video.Start(_ezb, _ezb.ConnectedEndPointAddress, 24);

                    _video.CameraSetting(EZBv4Video.CameraSettingsEnum.Res160x120); // 640X480
                    _video.CameraSetting(EZBv4Video.CameraSettingsEnum.MirrorDisable);

                    if (_video.IsRunning)
                    {
                        cameraStatus.Text = "CAMERA STREAMING!";
                    }
                    else
                    {
                        cameraStatus.Text = "CAMERA NOT STREAMING!";
                    }

                    /*
                     * Movement code init
                     */
                    _ezb.Movement.MovementType = EZ_B.Movement.MovementTypeEnum.HBridgePWM;
                    _ezb.Movement.HBridgeLeftWheelTriggerA = EZ_B.Digital.DigitalPortEnum.D1;
                    _ezb.Movement.HBridgeLeftWheelTriggerB = EZ_B.Digital.DigitalPortEnum.D2;
                    _ezb.Movement.HBridgeRightWheelTriggerA = EZ_B.Digital.DigitalPortEnum.D3;
                    _ezb.Movement.HBridgeRightWheelTriggerB = EZ_B.Digital.DigitalPortEnum.D4;
                    _ezb.Movement.HBridgeLeftWheelPWM = EZ_B.Digital.DigitalPortEnum.D0;
                    _ezb.Movement.HBridgeRightWheelPWM = EZ_B.Digital.DigitalPortEnum.D5;

                    _ezb.Movement.SetSpeed(255);

                    /*
                     * Servo init code
                     */
                    await _ezb.Servo.ResetAllServoSpeeds(); // must reset speed to initialize

                    int servo_speed = 128; 
                    
                    //left shoulder
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D15, EZ_B.Servo.SERVO_CENTER, servo_speed);
                    //right shoulder
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D19, EZ_B.Servo.SERVO_CENTER, servo_speed);

                    //left elbow
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D14, EZ_B.Servo.SERVO_CENTER, servo_speed);
                    //right elbow
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D18, EZ_B.Servo.SERVO_CENTER, servo_speed);

                    //left wrist
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D13, EZ_B.Servo.SERVO_CENTER, servo_speed);
                    //right wrist
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D17, EZ_B.Servo.SERVO_CENTER, servo_speed);

                    //left claw
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D12, EZ_B.Servo.SERVO_CENTER, servo_speed);
                    //right claw
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D16, EZ_B.Servo.SERVO_CENTER, servo_speed);


                    // neck, vertical
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D9, EZ_B.Servo.SERVO_CENTER, servo_speed);
                    // neck, horizontal
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D10, EZ_B.Servo.SERVO_CENTER, servo_speed);

                }
            }
            return;
        }
        //end of Button_Click

        /*
         * Handler for Forward Button
         */
        private void Forward_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                BotMove.stop(move_vec, _ezb);
                BotMove.forward(move_vec, _ezb);
            }
        }

        /*
         * Handler for Stop Button
         */
        private void Stop_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(_ezb != null)
            {
                BotMove.stop(move_vec, _ezb);
            }
        }

        /*
         * Handler for Reverse Button
         */
        private void Reverse_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                BotMove.stop(move_vec, _ezb);
                BotMove.reverse(move_vec, _ezb);
            }
        }

        /*
         * Handler for Right Button
         */
        private void Right_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                BotMove.right(move_vec, _ezb);
            }
        }

        /*
         * Handler for Left Button
         */
        private void Left_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                BotMove.left(move_vec, _ezb);
            }
        }



        private async void Video_OnImageDataReady(byte[] imageData)
        {
            
            await Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.Normal,
            async () =>
            {
                MemoryStream ms = new MemoryStream(imageData);

                var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, ms.AsRandomAccessStream());

                var sbit = await decoder.GetSoftwareBitmapAsync();

                // var added
                var bm = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);

                sbit.CopyToBuffer(bm.PixelBuffer);

                videoFeed.Source = bm;
            });

            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //{
            //    imageSource.Source = (BitmapSource)imageData.AsBuffer();
            //});
        }

        private void Video_OnStart()
        {
            tappedStatus.Text = "OnStart!";
        }

        private void isEscape()
        {
           
            MusicSynth m = new MusicSynth(_ezb);
            m.PlayNote(MusicSynth.NotesEnum.C1, 1000);
        }

        private async void _timer_Tick(Object sender, Object e)
        {
           
            tappedStatus.Text = string.Format("ADC 0: {0}", await _ezb.ADC.GetADCValue12Bit(EZ_B.ADC.ADCPortEnum.ADC0));

        }

        private void connectStatus_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        /*
         * Handler for neck roll (side to side rotation)
         */
        private async void NeckSwivel(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if ((slider != null) && (_ezb != null)) // must check if _ezb is null because value change event is raised when slider value set
            {
                System.Int32 neck_pos = (int)(slider.Value);
                await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D10, neck_pos, 60); //last value (60) is speed
            }
        }

        /*
         * Handler for neck pitch (up and down) 
         */
        private async void NeckNod(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            // test if _ezb is initialized, or else it shits itself
            if ((slider != null) && (_ezb != null))
            {
                System.Int32 neck_pos = (int)(slider.Value + 45);
                await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D9, neck_pos, 60);
            }
        }

        private async void Key_Down(object sender, KeyRoutedEventArgs e)
        {
            int servo_speed = 128;

            if (_ezb != null)
            {
                if (e.Key.ToString() == "W")
                {
                    BotMove.forward(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "S")
                {
                    BotMove.reverse(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "A")
                {
                    BotMove.left(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "D")
                {
                    BotMove.right(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "U") //open LEFT
                {
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D12, EZ_B.Servo.SERVO_MIN, servo_speed);
                }
                else if (e.Key.ToString() == "I")  //close LEFT
                {
                    //left claw
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D12, EZ_B.Servo.SERVO_CENTER, servo_speed);
                }
                else if (e.Key.ToString() == "O") //close RIGHT
                {
                    //right claw
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D16, EZ_B.Servo.SERVO_CENTER, servo_speed);
                }
                else if (e.Key.ToString() == "P")  //open RIGHT
                {
                    //grab using right hand
                    await _ezb.Servo.SetServoPosition(EZ_B.Servo.ServoPortEnum.D16, EZ_B.Servo.SERVO_MIN, servo_speed);
                }
            }
        }

        private void Key_Up(object sender, KeyRoutedEventArgs e)
        {
            if (_ezb != null)
            {

                if (e.Key.ToString() == "W")
                {
                    BotMove.reverse(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "S")
                {
                    BotMove.forward(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "A")
                {
                    BotMove.right(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "D")
                {
                    BotMove.left(move_vec, _ezb);
                }
                else if (e.Key.ToString() == "U")
                {
                    
                }
                else if (e.Key.ToString() == "I")
                {
                    
                }
                else if (e.Key.ToString() == "O")
                {
                    
                }
                else if (e.Key.ToString() == "P")
                {
                    
                }
            }
        }


        /*
         * Helper class to simplify more complex bot movement functions
         */
        private class BotMove {
            
            /*
             * Constants for convenience.
             */
            private static int PORT = -1;
            private static int STARBOARD = 1;
            //private static int FORWARD  = 1;
            //private static int REVERSE = -1;
            
            /*
             * Constant for the sharpness of the turn.
             */
            //private static int TURNNING_RADIUS = 4;
            
            public static void forward(double[] move_vec, EZB bot) {
                move_vec[SURGE] += 1;
                if(move_vec[SURGE] > 1) {
                    move_vec[SURGE] = 1;
                }
                updateSpeed(move_vec[SURGE], move_vec[YAW], bot);
            }
            
            public static void reverse(double[] move_vec, EZB bot) {
                move_vec[SURGE] -= 1;
                if(move_vec[SURGE] < -1) {
                    move_vec[SURGE] = -1;
                }
                updateSpeed(move_vec[SURGE], move_vec[YAW], bot);
            }
            
            public static void left(double[] move_vec, EZB bot) {
                move_vec[YAW] -= 1;
                if(move_vec[YAW] < -1) {
                    move_vec[YAW] = -1;
                }
                updateSpeed(move_vec[SURGE], move_vec[YAW], bot);
            }
            
            public static void right(double[] move_vec, EZB bot) {                
                move_vec[YAW] += 1;
                if(move_vec[YAW] > 1) {
                    move_vec[YAW] = 1;
                }
                updateSpeed(move_vec[SURGE], move_vec[YAW], bot);
            }
            
            public static void stop(double[] move_vec, EZB bot) {
                move_vec[SURGE] = 0;
                move_vec[YAW] = 0;
                updateSpeed(0, 0, bot);
            }
            
            
            private static void updateSpeed(double surge, double yaw, EZB _ezb) {
                int port, starbrd;
                byte port_speed, starbrd_speed;
                
                port = adjustSpeed(surge, yaw, PORT);
                starbrd = adjustSpeed(surge, yaw, STARBOARD);
                
                if(port < 0) {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerA, false);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerB, true);
                    port_speed = (byte)(-1 * port);
                } else if(port > 0) {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerA, true);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerB, false);
                    port_speed = (byte)(port);
                } else {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerA, false);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeLeftWheelTriggerB, false);
                    port_speed = 0;
                }
                
                if(starbrd < 0) {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerA, false);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerB, true);
                    starbrd_speed = (byte)(-1 * starbrd);
                } else if(starbrd > 0) {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerA, true);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerB, false);
                    starbrd_speed = (byte)(starbrd);
                } else {
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerA, false);
                    _ezb.Digital.SetDigitalPort(_ezb.Movement.HBridgeRightWheelTriggerB, false);
                    starbrd_speed = 0;
                }
                    
                _ezb.Movement.SetSpeed(port_speed, starbrd_speed);
            }
            
            //update docstring
            /*
             * Helper function to calculate speed of a given tread.
             * @param d_tran Direction of translational veloicity i.e. FORWARD/REVERSE
             * @param d_rot Direction of rotational velocity i.e. PORT/STARBOARD
             * @param curr_speed Current speed of travel
             * @return Adjusted speed of specified tread
             */
            private static int adjustSpeed(double surge, double yaw, int side) {
                double adj_vec = surge - (side * yaw );
                
                int speed = (int)(adj_vec * 127);
                // clamp speed to range of byte values, i.e. 0 \leq speed \leq 255
                if(speed > 255) {
                    speed = 255;
                } else if (speed < -255) {
                    speed = -255;
                }
                return (int)(speed);
            }
        }
    }


}


