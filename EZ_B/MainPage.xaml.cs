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



        public MainPage()
        {
            this.InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;
            Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode = Windows.UI.ViewManagement.ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = 800, Height = 480 });

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

                    // _video.OnStart += Video_OnStart;
                    errorStatus.Text = _ezb.GetLastErrorMsg;

                    isEscape();

                    await _video.Start(_ezb, _ezb.ConnectedEndPointAddress, 24);


                    _video.CameraSetting(EZBv4Video.CameraSettingsEnum.Res640x480);
                //    _video.CameraSetting(EZBv4Video.CameraSettingsEnum.START);

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
                _ezb.Movement.GoForward();
            }
        }

        /*
         * Handler for Stop Button
         */
        private void Stop_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(_ezb != null)
            {
                _ezb.Movement.GoStop();
            }
        }

        /*
         * Handler for Reverse Button
         */
        private void Reverse_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                _ezb.Movement.GoReverse();
            }
        }

        /*
         * Handler for Right Button
         */
        private void Right_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                _ezb.Movement.GoRight();
            }
        }

        /*
         * Handler for Left Button
         */
        private void Left_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (_ezb != null)
            {
                _ezb.Movement.GoLeft();
            }
        }



        private async void Video_OnImageDataReady(byte[] imageData)
        {
            
            //tappedStatus.Text = "OnIMAGE!";
            //MusicSynth m = new MusicSynth(_ezb);
            //m.PlayNote(MusicSynth.NotesEnum.C1, 1000);


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
            //tappedStatus.Text = "hello";
            MusicSynth m = new MusicSynth(_ezb);
            m.PlayNote(MusicSynth.NotesEnum.C1, 1000);
        }

        private async void _timer_Tick(Object sender, Object e)
        {
           
            tappedStatus.Text = string.Format("ADC 0: {0}", await _ezb.ADC.GetADCValue12Bit(EZ_B.ADC.ADCPortEnum.ADC0));
            //if(_video.IsRunning)
            //{
            //    tappedStatus.Text = string.Format("Camera is running");
            //}
            //else
            //{
            //    tappedStatus.Text = string.Format("Camera NOT running");
            //}
        }

        private void connectStatus_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
}


