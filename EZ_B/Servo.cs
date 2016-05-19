using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZ_B {

  public class Servo {

    EZB _ezb;

    private volatile int[] _servoPositions;
    private volatile bool[] _servoReleased;
    private volatile int[] _servoSpeeds;
    private volatile DateTime[] _servoLastMoveTime;
    private volatile int[] _servoFineTune;
    private volatile int[] _servoMax;
    private volatile int[] _servoMin;

    /// <summary>
    /// Event that is raised when a servo is moved
    /// </summary>
    public event OnServoMoveHandler OnServoMove;
    public delegate void OnServoMoveHandler(Classes.ServoItem[] servos);

    /// <summary>
    ///  The slowest speed for a servo (0)
    /// </summary>
    public static readonly int SERVO_SPEED_FASTEST = 0;

    /// <summary>
    /// The slowest speed for a servo (20)
    /// </summary>
    public static readonly int SERVO_SPEED_SLOWEST = 20;

    /// <summary>
    ///  The maximum value for a servo (100)
    /// </summary>
    public static int SERVO_MAX = 180;

    /// <summary>
    ///  The ideal center value of a servo (50 for EZ-B v3 and 90 for EZ-B v4)
    /// </summary>
    public static int SERVO_CENTER = 90;

    /// <summary>
    ///  The minimum value of a servo (1)
    /// </summary>
    public static int SERVO_MIN = 1;

    /// <summary>
    ///  The value of a servo to disable
    /// </summary>
    public static readonly int SERVO_OFF = 0;

    /// <summary>
    /// List of Servo Ports
    /// </summary>
    [FlagsAttribute]
    public enum ServoPortEnum {
      D0 = 0,
      D1,
      D2,
      D3,
      D4,
      D5,
      D6,
      D7,
      D8,
      D9,
      D10,
      D11,
      D12,
      D13,
      D14,
      D15,
      D16,
      D17,
      D18,
      D19,
      D20,
      D21,
      D22,
      D23,

      NA,

      V0,
      V1,
      V2,
      V3,
      V4,
      V5,
      V6,
      V7,
      V8,
      V9,
      V10,
      V11,
      V12,
      V13,
      V14,
      V15,
      V16,
      V17,
      V18,
      V19,
      V20,
      V21,
      V22,
      V23,
      V24,
      V25,
      V26,
      V27,
      V28,
      V29,
      V30,
      V31,

      AX0,
      AX1,
      AX2,
      AX3,
      AX4,
      AX5,
      AX6,
      AX7,
      AX8,
      AX9,
      AX10,
      AX11,
      AX12,
      AX13,
      AX14,
      AX15,
      AX16,
      AX17,
      AX18,
      AX19,
      AX20,
      AX21,
      AX22,
      AX23,
      AX24,
      AX25,
      AX26,
      AX27,
      AX28,
      AX29,
      AX30,
      AX31,
      AX32,
      AX33,
      AX34,
      AX35,
      AX36,
      AX37,
      AX38,
      AX39,
      AX40,
      AX41,
      AX42,
      AX43,
      AX44,
      AX45,
      AX46,
      AX47,
      AX48,
      AX49,
      AX50,
      AXV0,
      AXV1,
      AXV2,
      AXV3,
      AXV4,
      AXV5,
      AXV6,
      AXV7,
      AXV8,
      AXV9,
      AXV10,
      AXV11,
      AXV12,
      AXV13,
      AXV14,
      AXV15,
      AXV16,
      AXV17,
      AXV18,
      AXV19,
      AXV20,
      AXV21,
      AXV22,
      AXV23,
      AXV24,
      AXV25,
      AXV26,
      AXV27,
      AXV28,
      AXV29,
      AXV30,
      AXV31,
      AXV32,
      AXV33,
      AXV34,
      AXV35,
      AXV36,
      AXV37,
      AXV38,
      AXV39,
      AXV40,
      AXV41,
      AXV42,
      AXV43,
      AXV44,
      AXV45,
      AXV46,
      AXV47,
      AXV48,
      AXV49,
      AXV50
    }

    protected internal Servo(EZB ezb) {

      _ezb = ezb;

      int size = Enum.GetNames(typeof(ServoPortEnum)).Length;

      _servoLastMoveTime = new DateTime[size];
      _servoPositions = new int[size];
      _servoReleased = new bool[size];
      _servoSpeeds = new int[size];
      _servoFineTune = new int[size];
      _servoMin = new int[size];
      _servoMax = new int[size];
      for (int i = 0; i < _servoPositions.Length; i++) {

        _servoLastMoveTime[i] = DateTime.Now;
        _servoPositions[i] = 0;
        _servoReleased[i] = true;
        _servoSpeeds[i] = 0;
        _servoFineTune[i] = 0;
        _servoMax[i] = int.MaxValue;
        _servoMin[i] = int.MinValue;
      }
    }

    internal void Init() {

      for (int x = 0; x < _servoPositions.Length; x++)
        _servoPositions[x] = -1;

      for (int x = 0; x < _servoReleased.Length; x++)
        _servoReleased[x] = true;

      for (int x = 0; x < _servoSpeeds.Length; x++)
        _servoSpeeds[x] = -1;
    }

    /// <summary>
    ///  Reset the fine tuning values to 0 for each servo
    /// </summary>
    public void ResetServoFineTune() {

      for (int x = 0; x < _servoFineTune.Length; x++)
        _servoFineTune[x] = 0;
    }

    /// <summary>
    /// Return the fine tunign value of the specified servo
    /// </summary>
    public int GetServoFineTune(ServoPortEnum servoPort) {

      return _servoFineTune[(int)servoPort];
    }

    /// <summary>
    /// Set the fine tuning value for the specified servo. This means that if the fine tune value for a servo is set to 1, then every position that is specified will be incremented by 1.
    /// This allows you to fine tune a servo position across the entire application.
    /// </summary>
    public void SetServoFineTune(ServoPortEnum servoPort, int fineTune) {

      _servoFineTune[(int)servoPort] = fineTune;
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  Reset the servo min value
    /// </summary>
    public void ResetServoMinLimits() {

      for (int x = 0; x < _servoMin.Length; x++)
        _servoMin[x] = 0;
    }

    /// <summary>
    /// Return the min value that this servo will ever move 
    /// </summary>
    public int GetServoMin(ServoPortEnum servoPort) {

      return _servoMin[(int)servoPort];
    }

    /// <summary>
    /// Set the mininum servo value that this servo will ever be able to go
    /// </summary>
    public void SetServoMin(ServoPortEnum servoPort, int min) {

      _servoMin[(int)servoPort] = min;
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  Reset the servo max value
    /// </summary>
    public void ResetServoMaxLimits() {

      for (int x = 0; x < _servoMax.Length; x++)
        _servoMax[x] = 0;
    }

    /// <summary>
    /// Return the max value that this servo will ever move 
    /// </summary>
    public int GetServoMax(ServoPortEnum servoPort) {

      return _servoMax[(int)servoPort];
    }

    /// <summary>
    /// Set the mininum servo value that this servo will ever be able to go
    /// </summary>
    public void SetServoMax(ServoPortEnum servoPort, int max) {

      _servoMax[(int)servoPort] = max;
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Set the speed and position of a servo
    /// </summary>
    public async Task SetServoPosition(ServoPortEnum servoPort, int position, int speed) {

      await SetServoSpeed(servoPort, speed);
      await SetServoPosition(servoPort, position);
    }

    public async Task SetServoPositionScalar(ServoPortEnum servoPort, int servoMin, int servoMax, int clientWidthMin, int clientWidthMax, int clientPosition, bool invert) {

      await SetServoPositionScalar(servoPort, servoMin, servoMax, (float)clientWidthMin, (float)clientWidthMax, (float)clientPosition, invert);
    }

    public async Task SetServoPositionScalar(ServoPortEnum servoPort, int servoMin, int servoMax, float clientWidthMin, float clientWidthMax, float clientPosition, bool invert) {

      if (clientWidthMin < 0) {

        clientWidthMax += Math.Abs(clientWidthMin);
        clientPosition += Math.Abs(clientWidthMin);
        clientWidthMin = 0;
      }

      float scalarX = (float)(servoMax - servoMin) / (clientWidthMax - clientWidthMin);

      int x = (int)(scalarX * clientPosition);

      if (invert)
        x = servoMax - x;
      else
        x += servoMin;

      if (x > servoMax)
        x = servoMax;

      if (x < servoMin)
        x = servoMin;

      await SetServoPosition(servoPort, x);
    }

    /// <summary>
    /// Set the position of a servo
    /// Uses the last speed specified
    /// </summary>
    public async Task SetServoPosition(ServoPortEnum servoPort, int position) {

      await SetServoPosition(new Classes.ServoItem[] { new Classes.ServoItem(servoPort, position) });
    }

    /// <summary>
    /// Set the position of a servo
    /// Uses the last speed specified
    /// </summary>
    public async Task SetServoPosition(Classes.ServoItem[] servos) {

      if (!_ezb.IsConnected)
        return;

      List<byte> cmdData = new List<byte>();

      foreach (Classes.ServoItem servo in servos) {

        if (servo.Position < _servoMin[(int)servo.Port] || servo.Position > _servoMax[(int)servo.Port])
          continue;

        _servoPositions[(int)servo.Port] = servo.Position;
        _servoReleased[(int)servo.Port] = false;
        _servoLastMoveTime[(int)servo.Port] = DateTime.Now;

        int tmpPosition = servo.Position + _servoFineTune[(int)servo.Port];

        if (tmpPosition > SERVO_MAX)
          tmpPosition = SERVO_MAX;
        else if (tmpPosition < SERVO_MIN)
          tmpPosition = SERVO_MIN;

        if (servo.Port >= ServoPortEnum.D0 && servo.Port <= ServoPortEnum.D23) {

          cmdData.AddRange(new byte[] { (byte)(EZB.CommandEnum.CmdSetServoPosition + (byte)servo.Port), (byte)tmpPosition });
        } else if (servo.Port >= ServoPortEnum.AX0 && servo.Port <= ServoPortEnum.AX50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

          byte id = (byte)(servo.Port - ServoPortEnum.AX0);
          int position = (int)(tmpPosition * 5.689);

          cmdData.Add((byte)EZB.CommandEnum.CmdEZBv4);
          cmdData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

          byte[] cmd1 = Dynamixel.GetDisableStatusPacketCmd(id);
          byte[] cmd2 = Dynamixel.MoveServoCmd(id, position);

          cmdData.Add((byte)(cmd1.Length + cmd2.Length));

          cmdData.AddRange(cmd1);
          cmdData.AddRange(cmd2);

        } else if (servo.Port >= ServoPortEnum.AXV0 && servo.Port <= ServoPortEnum.AXV50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

          List<byte> tmpData = new List<byte>();
          byte id = (byte)(servo.Port - ServoPortEnum.AXV0);
          int position = (int)(tmpPosition * 5.689);

          tmpData.Add((byte)EZB.CommandEnum.CmdEZBv4);
          tmpData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

          byte[] cmd1 = DynamixelV2.MoveServoCmd(id, position);
          byte[] cmd2 = DynamixelV2.GetDisableStatusPacketCmd(id);

          tmpData.Add((byte)(cmd1.Length + cmd2.Length));

          tmpData.AddRange(cmd1);
          tmpData.AddRange(cmd2);

          await _ezb.sendCommandData(0, tmpData.ToArray());
        }
      }

      if (cmdData.Count > 0)
        await _ezb.sendCommandData(0, cmdData.ToArray());

      if (OnServoMove != null)
        OnServoMove(servos);
    }

    /// <summary>
    /// Set the speed of a servo
    /// </summary>
    public async Task SetServoSpeed(ServoPortEnum servoPort, int speed) {

      await SetServoSpeed(new ServoPortEnum[] { servoPort }, speed);
    }

    /// <summary>
    /// Set the speed of multiple servos
    /// </summary>
    /// <param name="servoPort"></param>
    /// <param name="speed"></param>
    public async Task SetServoSpeed(ServoPortEnum[] servoPorts, int speed) {

      if (!_ezb.IsConnected)
        return;

      if (speed > SERVO_SPEED_SLOWEST)
        speed = SERVO_SPEED_SLOWEST;
      else if (speed < SERVO_SPEED_FASTEST)
        speed = SERVO_SPEED_FASTEST;

      List<byte> cmdData = new List<byte>();

      foreach (ServoPortEnum servoPort in servoPorts) {

        if (_servoReleased[(int)servoPort])
          return;

        if (_servoSpeeds[(int)servoPort] == speed)
          return;

        _servoSpeeds[(int)servoPort] = speed;

        if (servoPort >= ServoPortEnum.D0 && servoPort <= ServoPortEnum.D23) {

          cmdData.Add((byte)((byte)EZB.CommandEnum.CmdSetServoSpeed + servoPort));
          cmdData.Add((byte)speed);
        } else if (servoPort >= ServoPortEnum.AX0 && servoPort <= ServoPortEnum.AX50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

          byte id = (byte)(servoPort - ServoPortEnum.AX0);
          int position = speed * 51;

          cmdData.Add((byte)EZB.CommandEnum.CmdEZBv4);
          cmdData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

          byte[] cmd = Dynamixel.ServoSpeed(id, position);

          cmdData.Add((byte)cmd.Length);

          cmdData.AddRange(cmd);
        } else if (servoPort >= ServoPortEnum.AXV0 && servoPort <= ServoPortEnum.AXV50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

          byte id = (byte)(servoPort - ServoPortEnum.AXV0);
          int position = speed * 51;

          cmdData.Add((byte)EZB.CommandEnum.CmdEZBv4);
          cmdData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

          byte[] cmd = DynamixelV2.ServoSpeed(id, position);

          cmdData.Add((byte)cmd.Length);

          cmdData.AddRange(cmd);
        }
      }

      await _ezb.sendCommandData(0, cmdData.ToArray());
    }

    /// <summary>
    /// Return the current speed of a servo
    /// </summary>
    public int GetServoSpeed(ServoPortEnum servoPort) {

      return _servoSpeeds[(int)servoPort];
    }

    /// <summary>
    /// Get the position of a servo
    /// </summary>
    public int GetServoPosition(ServoPortEnum servoPort) {

      return _servoPositions[(int)servoPort];
    }

    /// <summary>
    /// Release servo. Release a servo from holding its position.
    /// If modified, stops the servo.
    /// </summary>
    public async Task ReleaseServo(ServoPortEnum servoPort) {

      _servoLastMoveTime[(int)servoPort] = DateTime.Now;
      _servoReleased[(int)servoPort] = true;

      if (servoPort >= ServoPortEnum.D0 && servoPort <= ServoPortEnum.D23) {

        await _ezb.sendCommand(EZB.CommandEnum.CmdSetServoPosition + (byte)servoPort, new byte[] { (byte)0 });
      } else if (servoPort >= ServoPortEnum.AX0 && servoPort <= ServoPortEnum.AX50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

        byte id = (byte)(servoPort - ServoPortEnum.AX0);
        List<byte> cmdData = new List<byte>();

        cmdData.Add((byte)EZB.CommandEnum.CmdEZBv4);
        cmdData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

        byte[] cmd = Dynamixel.ReleaseServo(id);

        cmdData.Add((byte)cmd.Length);

        cmdData.AddRange(cmd);

        await _ezb.sendCommandData(0, cmdData.ToArray());
      } else if (servoPort >= ServoPortEnum.AXV0 && servoPort <= ServoPortEnum.AXV50 && _ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4) {

        byte id = (byte)(servoPort - ServoPortEnum.AXV0);
        List<byte> cmdData = new List<byte>();

        cmdData.Add((byte)EZB.CommandEnum.CmdEZBv4);
        cmdData.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);

        byte[] cmd = DynamixelV2.ReleaseServo(id);

        cmdData.Add((byte)cmd.Length);

        cmdData.AddRange(cmd);

        await _ezb.sendCommandData(0, cmdData.ToArray());
      }
    }

    /// <summary>
    ///  When servos have been used, they will hold their position until the EZ-B power is cycled or until they are told to release.
    ///  This will send a command to the EZ-B to release all servos
    /// </summary>
    public async Task ReleaseAllServos() {

      for (int x = 0; x < _servoPositions.Length; x++) {

        _servoLastMoveTime[x] = DateTime.Now;
        _servoReleased[x] = true;
      }

      await _ezb.sendCommand(EZB.CommandEnum.CmdReleaseAllServos);
    }

    /// <summary>
    /// Reset all the servo speeds to their default of 0 (fastest)
    /// </summary>
    public async Task ResetAllServoSpeeds() {

      foreach (Servo.ServoPortEnum item in Enum.GetValues(typeof(Servo.ServoPortEnum)))
        await SetServoSpeed(item, 0);
    }

    /// <summary>
    /// Return true if the specified servo port is in a released state
    /// </summary>
    public bool IsServoReleased(ServoPortEnum servoPort) {

      return _servoReleased[(int)servoPort];
    }

    public double GetNumberOfSecondsSinceLastMove(ServoPortEnum servoPort) {

      return (DateTime.Now - _servoLastMoveTime[(int)servoPort]).TotalSeconds;
    }
  }
}
