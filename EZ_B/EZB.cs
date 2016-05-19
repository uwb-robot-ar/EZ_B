using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZ_B {

  public partial class EZB {

    private long lastCommandReturnedDateTimeTicks = DateTime.Now.Ticks;
    private long lastCommandSentDateTimeTicks = DateTime.Now.Ticks;

    public string ConnectedEndPointAddress = "192.168.1.1";

    /// <summary>
    /// Get the last datetime that data was returned from the EZ-B
    /// </summary>
    public DateTime LastCommandReturnDateTime {
      get {
        return new DateTime(lastCommandReturnedDateTimeTicks);
      }
    }

    /// <summary>
    /// Get the last datetime that data was sent to the EZ-B
    /// </summary>
    public DateTime LastCommandSentDateTime {
      get {
        return new DateTime(lastCommandSentDateTimeTicks);
      }
    }

    public bool IgnoreFirmwareError = false;

    /// <summary>
    /// Event risen when there is a connection change
    /// </summary>
    public delegate void OnConnectionChangeHandler(bool isConnected);

    /// <summary>
    /// Event risen when there is a connection change
    /// </summary>
    public event OnConnectionChangeHandler OnConnectionChange;

    /// <summary>
    /// Event risen when there is debug data
    /// </summary>
    public delegate void OnLogHandler(string logTxt);

    /// <summary>
    /// Event risen when there is debug data
    /// </summary>
    public event OnLogHandler OnLog;

    /// <summary>
    /// Get the last verbose error message.
    /// Use if VerboseLogging is False to receive the last detailed error.
    /// </summary>
    public string GetLastErrorMsg {
      get {
        return _lastErrorStr.ToString();
      }
    }

    /// <summary>
    /// Gets the type of EZ-B that is connected
    /// </summary>
    public EZ_B_Type_Enum EZBType = EZ_B_Type_Enum.na;

    private byte[] _uniqueID = new byte[] { };
    private double _firmwareResponse = -1;
    private StringBuilder _lastErrorStr = new StringBuilder();
    private RandomUnique _randomUnique = new RandomUnique();
    private ConnectionTypeEnum _connectionType = ConnectionTypeEnum.TCP;
    private TcpClient _tcpClient = new TcpClient();

    internal enum ConnectionTypeEnum {
      TCP
    }

    public enum EZ_B_Type_Enum {
      na,
      ezb3,
      ezb4
    }

    internal enum CommandEnum {

      CmdUnknown = 0,
      CmdReleaseAllServos = 1,
      CmdGetUniqueID = 2,
      CmdEZBv3 = 3,
      CmdEZBv4 = 4,
      CmdSoundBeep = 5,
      CmdEZServo = 6,
      CmdI2CWrite = 10,
      CmdI2CRead = 11,
      CmdBootLoader = 14,
      CmdSetPWMSpeed = 15, // +23 (38)
      CmdSetServoSpeed = 39, // +23 (62)
      CmdPing = 0x55,
      CmdSetDigitalPortOn = 100, // +23 (123)
      CmdSetDigitalPortOff = 124, // +23 (147)
      CmdGetDigitalPort = 148, // +23 (171)
      CmdSetServoPosition = 172, // +23 (195)
      CmdGetADCValue = 196, // +7 (203)
      CmdSendSerial = 204, // +23 (227)
      CmdHC_SR04 = 228, // +23 (251)
      CmdSoundStreamCmd = 254
    }

    internal enum CmdEZBv3Enum {

      CmdV3WriteConfiguration = 0,
      CmdV3ReadConfiguration = 1,
      CmdV3BV4113 = 35,
      CmdV3RoboSapien = 86,
      CmdV3RoboQuad = 87,
    }

    internal enum CmdEZBv4Enum {

      CmdV4SetLipoBatteryProtectionState = 0,
      CmdV4SetBatteryMonitorVoltage = 1,
      CmdV4GetBatteryVoltage = 2,
      CmdV4GetCPUTemp = 3,
      CmdV4SetAX12Servo = 4,

      CmdV4UARTExpansion0Init = 5,
      CmdV4UARTExpansion0Write = 6,
      CmdV4UARTExpansion0AvailableBytes = 7,
      CmdV4UARTExpansion0Read = 8,

      CmdV4UARTExpansion1Init = 9,
      CmdV4UARTExpansion1Write = 10,
      CmdV4UARTExpansion1AvailableBytes = 11,
      CmdV4UARTExpansion1Read = 12,

      CmdV4UARTExpansion2Init = 13,
      CmdV4UARTExpansion2Write = 14,
      CmdV4UARTExpansion2AvailableBytes = 15,
      CmdV4UARTExpansion2Read = 16,

      CmdV4I2CClockSpeed = 17,
      CmdV4UARTClockSpeed = 18,

      CmdV4ResetToDefaults = 19
    }

    internal enum CmdSoundv4Enum {

      CmdSoundInitStop = 0,
      CmdSoundLoad = 1,
      CmdSoundPlay = 2
    }
    private bool _isConnected = false;

    /// <summary>
    /// Returns true if currently connected to an EZ-B
    /// </summary>
    public bool IsConnected {
      get {
        return _isConnected;
      }
    }

    private bool verboseLogging = false;

    /// <summary>
    /// Set to TRUE to enable verbose logging. Only use this if you are debugging. This will produce lots of data.
    /// </summary>
    public bool VerboseLogging {
      get {
        return verboseLogging;
      }
      set {
        verboseLogging = value;
      }
    }

    /// <summary>
    /// Interact with the BV4615 i2c RC-5 Infrared Decoder
    /// </summary>
    public BV4615 BV4615;

    /// <summary>
    /// The Sure Electronics i2c Dual-Axis Magnetic Sensor Module (DC-SS503V100)
    /// </summary>
    public SureDualAxisCompass SureDualAxisCompass;

    /// <summary>
    /// Control a iRobot Roomba
    /// </summary>
    public Roomba Roomba;

    /// <summary>
    /// Send serial commands from any digital port
    /// </summary>
    public Uart Uart;

    /// <summary>
    /// Communicate to a TellyMate TV Board @ 57600 on Port D1
    /// </summary>
    public TellyMate TellyMate;

    /// <summary>
    /// Common methods and functionality for using Modified Servos to drive wheels.
    /// </summary>
    public Movement Movement;

    /// <summary>
    /// Servo commands. Control regular and modified servos.
    /// </summary>
    public Servo Servo;

    /// <summary>
    /// Analog To Digital Convertor (ADC) commands. Read voltages and values from the ADC Ports of the EZ-B
    /// </summary>
    public ADC ADC;

    /// <summary>
    /// Commands to read and write digital ports on the EZ-B
    /// </summary>
    public Digital Digital;

    /// <summary>
    /// Commands to get the distance from a HC-SR04 Ping Sensor
    /// </summary>
    public HC_SR04 HC_SR04;

    /// <summary>
    /// Send a I2C command out of the I2C interface
    /// </summary>
    public I2C I2C;

    /// <summary>
    /// Control multicolor BlinkM via I2C interface
    /// </summary>
    public BlinkM BlinkM;

    /// <summary>
    /// Control a MP3 Trigger
    /// </summary>
    public MP3Trigger MP3Trigger;

    /// <summary>
    /// Control the BV4113 EZ-Robot Motor Controller
    /// </summary>
    public BV4113 BV4113;

    /// <summary>
    /// Allows recording and replaying of communication between the computer and EZ-B
    /// </summary>
    public Recorder Recorder;

    /// <summary>
    /// MMA7455 Accelerometer
    /// </summary>
    public MMA7455 MMA7455;

    /// <summary>
    /// Unique name for this EZB Instance
    /// </summary>
    public string Name = Guid.NewGuid().ToString();

    /// <summary>
    /// Control PWM (Pulse Wave Modulation) output
    /// </summary>
    public PWM PWM;

    /// <summary>
    /// Controls a sabertooth motor controller over the serial interface
    /// </summary>
    public SabertoothSerial SabertoothSerial;

    /// <summary>
    /// Sound beep test for the v4
    /// </summary>
    public Soundv4 SoundV4;

    /// <summary>
    /// Manages settings specific to the EZ-B v4
    /// </summary>
    public EZBv4Manager EZBv4Manager;

    /// <summary>
    /// Helper Class for controlling Dynamixel AX12 servos directly and bypassing the Servo class
    /// </summary>
    public ServoAX12 ServoAX12;

    /// <summary>
    /// Helper Class for controlling the RGB LED Eyes that ships with JD, and can be purchased optionally seperate
    /// </summary>
    public RGBEyes RGBEyes;

    /// <summary>
    /// Helper Class for controlling the WowWee MIP balancing robot
    /// </summary>
    public MIP MIP;

    /// <summary>
    /// Helper class for making synthesized music on the ez-b v4 speaker
    /// </summary>
    public MusicSynth MusicSynth;

    /// <summary>
    /// Create an instance of the EZCommunicator and assign unique name
    /// </summary>
    public EZB(string name) {

      init();

      Name = name;
    }

    /// <summary>
    /// Create an instance of the EZCommunicator.
    /// </summary>
    public EZB() {

      init();
    }

    private void init() {

      TellyMate = new TellyMate(this);
      Movement = new Movement(this);
      Servo = new Servo(this);
      ADC = new ADC(this);
      Digital = new Digital(this);
      HC_SR04 = new HC_SR04(this);
      Uart = new Uart(this);
      Roomba = new Roomba(this);
      I2C = new I2C(this);
      BlinkM = new BlinkM(this);
      MP3Trigger = new MP3Trigger(this);
      SureDualAxisCompass = new SureDualAxisCompass(this);
      BV4615 = new BV4615(this);
      BV4113 = new BV4113(this);
      Recorder = new Recorder(this);
      MMA7455 = new MMA7455(this);
      PWM = new PWM(this);
      SabertoothSerial = new SabertoothSerial(this);
      SoundV4 = new EZ_B.Soundv4(this);
      EZBv4Manager = new EZ_B.EZBv4Manager(this);
      ServoAX12 = new EZ_B.ServoAX12(this);
      RGBEyes = new EZ_B.RGBEyes(this);
      MIP = new EZ_B.MIP(this);
      MusicSynth = new EZ_B.MusicSynth(this);
    }

    /// <summary>
    /// Manually send text to the log event
    /// </summary>
    public void Log(bool verbose, string txt, params object[] args) {

      if (!VerboseLogging && verbose)
        return;

      try {

        string msg = string.Format("{0} - ", DateTime.Now);

        msg += string.Format(txt, args);

        if (_lastErrorStr.Length >= 250000)
          _lastErrorStr.Clear();

        _lastErrorStr.Append(msg).AppendLine();

        if (OnLog != null)
          OnLog(msg);
      } catch {
      }
    }

    /// <summary>
    /// Return the firmware version in a string of the EZ-B
    /// </summary>
    public string GetFirmwareVersion() {

      if (_firmwareResponse == 0)
        return "Unknown EZ-Robot OS";

      if (_firmwareResponse == 4)
        return "EZ-B v4 OS";

      return string.Format("EZ-Robot OS v{0}", _firmwareResponse);
    }

    /// <summary>
    /// Return the firmware version number of the EZ-B
    /// </summary>
    public double GetFirmwareVersionRaw() {

      return _firmwareResponse;
    }

    /// <summary>
    /// Sends a ping request to the EZ-B to see if it's still responding. Returns a True if so, false if it isn't
    /// </summary>
    public async Task<bool> PingController() {

      byte tmp = (await sendCommand(1, CommandEnum.CmdPing))[0];

      if (tmp <= 15)
        _firmwareResponse = tmp;
      else
        _firmwareResponse = (double)tmp / 10;

      return (_firmwareResponse != 0);
    }

    /// <summary>
    /// Connect to an EZ-B.
    /// 1) Hostname can be a communication PORT. Get the port name from GetAvailableCommunicationPorts()
    /// 2) Hostname can be an IP Address, example: 192.168.1.5:6666
    /// 3) Leave TCPPassword blank if connecting to an EZ-B. It is only used when connecting to another EZ-Builder instance
    /// 4) Baudrate is not used for TCP connections
    /// </summary>
    public async Task Connect(string hostname) {

            if (_isConnected)
                throw new Exception("Already connected.");
            //else
                //throw new Exception("NOT");

      Log(false, "Attempting connection on {0}", hostname);

      if (hostname == string.Empty)
        throw new Exception("No connection method specified");

      _connectionType = ConnectionTypeEnum.TCP;

      string[] parts = hostname.Split(':');
      string ipAddress = parts[0];
      int port = 23;

      if (parts.Length > 1)
        port = Convert.ToInt16(parts[1]);

      await _tcpClient.Connect(ipAddress, port, 3000);

      ConnectedEndPointAddress = ipAddress;

      _isConnected = true;

      if (!await PingController())
        throw new Exception("Controller Not Responding");

      Log(false, "Connected to {0}", hostname);

      Log(false, "EZ-B reports " + GetFirmwareVersion());

      if (_firmwareResponse == 4) {

        EZBType = EZ_B_Type_Enum.ezb4;

        Log(false, "Welcome to EZ-B v4 Beta!");

        _uniqueID = await sendCommand(12, CommandEnum.CmdGetUniqueID);

        Log(false, "EZ-B v4 ID: {0}", GetUniqueIDString());

        Servo.SERVO_MAX = 180;
        Servo.SERVO_CENTER = 90;
      } else {

        throw new Exception("This device is not an EZ-B. Please follow the online tutorials on the EZ-Robot website.");
      }

      Servo.Init();
      Digital.Init();

      if (OnConnectionChange != null)
        OnConnectionChange(_isConnected);

      Log(false, "Connected");
    }

    /// <summary>
    /// Disconnect from the EZ-B
    /// </summary>
    public void Disconnect() {

      try {

        _isConnected = false;

        _tcpClient.Disconnect();

        if (OnConnectionChange != null)
          OnConnectionChange(_isConnected);

        Log(false, "Disconnected");
      } catch (Exception ex) {

        Log(false, "Error closing port: {0}", ex);
      }
    }

    internal async Task sendCommand(CommandEnum cmd) {

      await sendCommand(0, cmd, new byte[] { });
    }

    internal async Task sendCommand(CommandEnum cmd, params byte[] cmdArgs) {

      await sendCommand(0, cmd, cmdArgs);
    }

    internal async Task<byte[]> sendCommand(int bytesToExpect, CommandEnum cmd, params byte[] cmdArgs) {

      if (cmd == CommandEnum.CmdUnknown)
        throw new Exception("Unknown command. Why? How?");

      List<byte> cmdData = new List<byte>();
      cmdData.Add((byte)cmd);
      cmdData.AddRange(cmdArgs);

      return await sendCommandData(bytesToExpect, cmdData.ToArray());
    }

    internal async Task<byte[]> sendCommandData(int bytesToExpect, params byte[] cmdData) {

      if (Recorder.IsRecording && bytesToExpect == 0)
        Recorder.AddItem(cmdData);

      if (verboseLogging) {

        StringBuilder sbSend = new StringBuilder();

        sbSend.Append("Sending: ");

        foreach (byte c in cmdData)
          sbSend.AppendFormat("{0} ", c.ToString());

        Log(true, sbSend.ToString());
      }

      List<byte> retArray = new List<byte>();

      if (!_tcpClient.IsConnected)
        _isConnected = false;

      if (!_isConnected) {

        for (int x = 0; x < bytesToExpect; x++)
          retArray.Add(0x00);

        return retArray.ToArray();
      }

      if (_tcpClient.Available > 0) {

        Log(false, "*Warning: There is {0} bytes of unexpected data available on the TCP connection... Clearing", _tcpClient.Available);

        byte[] b = await _tcpClient.ReadBytes(_tcpClient.Available);

        string resp = string.Empty;

        foreach (byte bb in b)
          resp += bb.ToString() + " ";

        Log(false, "Unexpected Data: {0}", resp);
      }

      await _tcpClient.Send(cmdData);

      lastCommandSentDateTimeTicks = DateTime.Now.Ticks;

      if (bytesToExpect > 0) {

        for (int x = 0; x < bytesToExpect; x++)
          retArray.Add(await _tcpClient.ReadByte());

        if (verboseLogging) {

          StringBuilder sbExpect = new StringBuilder();
          sbExpect.AppendFormat("Expecting: {0} bytes", bytesToExpect).AppendLine();
          sbExpect.Append("Received: ");

          foreach (byte b in retArray)
            sbExpect.AppendFormat("{0} ", b.ToString());

          Log(true, sbExpect.ToString());
        }

        lastCommandReturnedDateTimeTicks = DateTime.Now.Ticks;
      }

      return retArray.ToArray();
    }

    /// <summary>
    /// Return a random number within specified range.
    /// Using this random number generating function will provide a common seed.
    /// </summary>
    public int GetRandomNumber(int lowestVal, int highestVal) {

      return _randomUnique.GetRandomNumber(lowestVal, highestVal);
    }

    /// <summary>
    /// Return a random number and tries to make the returned value unique from the last time this function was called.
    /// </summary>
    public int GetRandomUniqueNumber(int lowestVal, int highestVal) {

      return _randomUnique.GetRandomUniqueNumber(lowestVal, highestVal);
    }

    /// <summary>
    /// Returns a byte array unique ID of the EZ-B v4
    /// </summary>
    public byte[] GetUniqueIDBytes() {

      if (EZBType != EZ_B_Type_Enum.ezb4)
        throw new Exception("This command is only available for the EZ-B v4");

      return _uniqueID;
    }

    /// <summary>
    /// Returns a byte array unique ID of the EZ-B v4
    /// </summary>
    public string GetUniqueIDString() {

      string retStr = string.Empty;

      foreach (byte b in GetUniqueIDBytes())
        if (retStr == string.Empty)
          retStr += b.ToString();
        else
          retStr += "-" + b.ToString();

      return retStr;
    }

    /// <summary>
    /// Clear the log file
    /// </summary>
    public void ClearLog() {

      _lastErrorStr.Clear();
    }
  }
}
