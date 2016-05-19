namespace EZ_B {

  public class SabertoothSerial {

    EZB          _ezb;
    private byte _speed = Movement.MAX_SPEED;

    /// <summary>
    /// Value of the Left Wheel when moving forward
    /// </summary>
    public byte SpeedLeftWheelForward = 127;

    /// <summary>
    /// Value of the Right Wheel when moving forward
    /// </summary>
    public byte SpeedRightWheelForward = 128;

    /// <summary>
    /// Value of the Left Wheel when turning Left
    /// </summary>
    public byte SpeedLeftWheelTurnLeft = 127;

    /// <summary>
    /// Value of the Right Wheel when turning Right
    /// </summary>
    public byte SpeedRightWheelTurnLeft = 128;

    /// <summary>
    /// Value of the Left Wheel when moving reverse
    /// </summary>
    public byte SpeedLeftWheelReverse = 1;

    /// <summary>
    /// Value of the Right Wheel when moving reverse
    /// </summary>
    public byte SpeedRightWheelReverse = 255;

    /// <summary>
    /// Value of the Left Wheel when turning right
    /// </summary>
    public byte SpeedLeftWheelTurnRight = 1;

    /// <summary>
    /// Value of the Right Wheel when turning right
    /// </summary>
    public byte SpeedRightWheelTurnRight = 255;

    /// <summary>
    /// Baud rate for the communication
    /// </summary>
    public EZ_B.Uart.BAUD_RATE_ENUM BaudRate = Uart.BAUD_RATE_ENUM.Baud_38400;

    /// <summary>
    /// Digital port used for communication to the controller
    /// </summary>
    public EZ_B.Digital.DigitalPortEnum DigitalPort = Digital.DigitalPortEnum.D0;

    protected internal SabertoothSerial(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    ///  Stop
    /// </summary>
    public void Stop() {

      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { 0 });
    }

    /// <summary>
    ///  Move forward.
    /// </summary>
    public void Forward() {

      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedLeftWheelForward });
      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedRightWheelForward });
    }

    /// <summary> 
    ///  Move reverse.
    /// </summary>
    public void Reverse() {

      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedLeftWheelReverse });
      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedRightWheelReverse });
    }

    /// <summary> 
    ///  Right.
    /// </summary>
    public void Right() {

      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedLeftWheelTurnRight });
      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedRightWheelTurnRight });
    }

    /// <summary> 
    ///  Left.
    /// </summary>
    public void Left() {

      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedLeftWheelTurnLeft });
      _ezb.Uart.SendSerial(DigitalPort, BaudRate, new byte[] { SpeedRightWheelTurnLeft });
    }
  }
}
