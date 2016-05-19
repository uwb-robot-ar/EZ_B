using System;
using System.Threading.Tasks;

namespace EZ_B {

  public class MIP {

    EZB _ezb;
    EZBGWorker _bw;
    volatile byte[] _cmdSend = new byte[] { };

    /// <summary>
    /// Set the UART Peripheral Port
    /// </summary>
    public int UART_PORT = 0;

    /// <summary>
    /// The baud rate of the MIP communication
    /// </summary>
    public uint BAUD_RATE = 9600;

    public bool IsConnected {
      get {
        if (_bw == null)
          return false;

        return !_bw.IsBusy;
      }
    }

    protected internal MIP(EZB ezb) {

      _ezb = ezb;
    }

    async Task _bw_DoWork(object sender, DoWorkEventArgs e) {

      while (!_bw.CancellationPending) {

        if (_cmdSend.Length == 0)
          continue;

        _ezb.Uart.UARTExpansionWrite(UART_PORT, _cmdSend);

        await Task.Delay(100);

        if (_bw.CancellationPending || !_ezb.IsConnected) {

          e.Cancel = true;

          break;
        }
      }
    }

    public void Connect() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      Disconnect();

      if (!_ezb.IsConnected)
        return;

      if (_bw == null) {

        _bw = new EZBGWorker("MIP Controller");
        _bw.DoWork += _bw_DoWork;
      }

      Init(UART_PORT);

      if (_bw.IsBusy)
        _bw.RunWorkerAsync();
    }

    public async void Disconnect() {

      if (_bw != null && _bw.IsBusy)
        await _bw.CancelWorker();
    }

    /// <summary>
    /// Initialize the MIP Robot over the specified UART Port Index
    /// </summary>
    public async void Init(int UARTPort) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      UART_PORT = UARTPort;

      _ezb.Uart.UARTExpansionInit(UART_PORT, BAUD_RATE);

      await Task.Delay(500);

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x54, 0x54, 0x4D, 0x3A, 0x4F, 0x4B, 0x0D, 0x0A, 0x00 });
    }

    /// <summary>
    /// Move the MIP forward. The speed is a value between 1 and 31
    /// </summary>
    public void Forward(byte speed) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      speed = Math.Max(speed, (byte)31);

      _cmdSend = new byte[] { 0x78, (byte)(0x01 + speed) };
    }

    /// <summary>
    /// Move the MIP reverse. The speed is a value between 1 and 31
    /// </summary>
    public void Reverse(byte speed) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      speed = Math.Max(speed, (byte)31);

      _cmdSend = new byte[] { 0x78, (byte)(0x21 + speed) };
    }

    /// <summary>
    /// Move the MIP right. The speed is a value between 1 and 31
    /// </summary>
    public void Right(byte speed) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      speed = Math.Max(speed, (byte)31);

      _cmdSend = new byte[] { 0x78, 0x00, (byte)(0x41 + speed) };
    }

    /// <summary>
    /// Move the MIP left. The speed is a value between 1 and 31
    /// </summary>
    public void Left(byte speed) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      speed = Math.Max(speed, (byte)31);

      _cmdSend = new byte[] { 0x78, 0x00, (byte)(0x61 + speed) };
    }

    /// <summary>
    /// Stop the MIP
    /// </summary>
    public void Stop() {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _cmdSend = new byte[] { };

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x77 });
    }

    /// <summary>
    /// Play the sound file between 1 and 106
    /// </summary>
    public void PlaySound(byte file) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      file = Math.Max(file, (byte)106);

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x06, file });
    }

    /// <summary>
    /// Adjust the audio volume of the MIP between 0 and 6
    /// </summary>
    /// <param name="volume"></param>
    public void AdjustVolume(byte volume) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      volume = Math.Max(volume, (byte)7);

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x06, (byte)(0xf7 + volume) });
    }

    /// <summary>
    /// Set the color of the MIP chest LED. Colors can be between 0-255, and fadeInTime is 10ms intervals between 0-255
    /// </summary>
    public void SetChestLED(byte r, byte g, byte b, byte fadeInTime) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x84, r, g, b, fadeInTime });
    }

    /// <summary>
    /// Flash color of the MIP chest LED. Colors can be between 0-255, and timeOn and timeOff is 20ms intervals between 0-255
    /// </summary>
    public void FlashChestLED(byte r, byte g, byte b, byte timeOn, byte timeOff) {

      if (!_ezb.IsConnected || _ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _ezb.Uart.UARTExpansionWrite(UART_PORT, new byte[] { 0x89, r, g, b, timeOn, timeOff });
    }
  }
}
