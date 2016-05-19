namespace EZ_B {

  public class MP3Trigger {

    EZB _ezb;

    /// <summary>
    /// Specify the communication port that the MP3 Trigger is connected with
    /// </summary>
    public Digital.DigitalPortEnum CommunicationPort = Digital.DigitalPortEnum.D0;

    /// <summary>
    /// Specify the baud rate that the MP3 Trigger is connected with.
    /// Default is 38400
    /// </summary>
    public Uart.BAUD_RATE_ENUM BaudRate = Uart.BAUD_RATE_ENUM.Baud_38400;

    protected internal MP3Trigger(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Play previous track
    /// </summary>
    public void Reverse() {

      _ezb.Uart.SendSerial(CommunicationPort, BaudRate, new byte[] { (byte)'R' });
    }

    /// <summary>
    /// Play next track
    /// </summary>
    public void Forward() {

      _ezb.Uart.SendSerial(CommunicationPort, BaudRate, new byte[] { (byte)'F' });
    }

    /// <summary>
    /// Specify volume.
    /// 0 - Loud. 
    /// 255 - Quiet.
    /// </summary>
    public void SetVolume(byte volume) {

      _ezb.Uart.SendSerial(CommunicationPort, BaudRate, new byte[] { (byte)'v', (byte)volume });
    }

    /// <summary>
    /// Start/Stop
    /// </summary>
    public void StartStop() {

      _ezb.Uart.SendSerial(CommunicationPort, BaudRate, new byte[] { (byte)'O' });
    }

    /// <summary>
    /// Play specified track number
    /// </summary>
    /// <param name="trackNumber"></param>
    public void PlayTrack(byte trackNumber) {

      _ezb.Uart.SendSerial(CommunicationPort, BaudRate, new byte[] { (byte)'p', (byte)trackNumber });
    }
  }
}
