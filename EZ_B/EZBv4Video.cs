using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZ_B {

  public class EZBv4Video : DisposableBase {

    public event OnImageDataReadyHandler OnImageDataReady;
    public delegate void OnImageDataReadyHandler(byte[] imageData);

    /// <summary>
    /// Event raised when the JPEGStream has started
    /// </summary>
    public event OnStartHandler OnStart;
    public delegate void OnStartHandler();

    /// <summary>
    /// Event raised when the JPEGStream has stopped
    /// </summary>
    public event OnStopHandler OnStop;
    public delegate void OnStopHandler();

    private EZBGWorker _worker = new EZBGWorker("EZBv4Video");
    private TcpClient _tcpClient = new TcpClient();
    private EZB _ezb;
    private int _imageSize = 0;

    readonly byte[] TAG_EZIMAGE = new byte[] { (byte)'E', (byte)'Z', (byte)'I', (byte)'M', (byte)'G' };

    readonly uint BUFFER_SIZE = 512000;

    List<byte> _settingsToSend = new List<byte>();

    public enum CameraSettingsEnum {

      Res160x120 = 0,
      Res320x240 = 1,
      Res640x480 = 2,

      START = 3,
      STOP = 4,

      RESERVED_compression = 10,
      RESERVED_brightness = 11,
      FreqAuto = 12,
      RESERVED_ExposureMode = 13,
      RESERVED_SetReg = 14,
      RESERVED_SetRetryCount = 15,
      RESERVED_SetBufferSize = 16,

      MirrorEnable = 21,
      MirrorDisable = 22,

      Freq50hz = 23,
      Freq60hz = 24,

      BacklightOff = 25,
      BacklightOn = 26,

      IndoorAuto = 27,
      IndoorForce = 28,

      RESERVED_LED_Red = 29,
      RESERVED_LED_Green = 30,
      RESERVED_LED_Blue = 31,
    }

    public enum CameraExposureMode {
      Auto0 = 0,
      Auto1 = 1,
      Auto2 = 2,
      Auto3 = 3,
      Auto4 = 4
    }

    public enum CameraBandEnum {
      BlacknWhite = 0x18,
      ColorNegative = 0x40,
      BlacknWhiteNegative = 0x58,
      Normal = 0x00
    }

    /// <summary>
    /// Returns the size of the last camera image
    /// </summary>
    public int GetImageSize {
      get {
        return _imageSize;
      }
    }

    /// <summary>
    /// Returns the status of the camera streaming
    /// </summary>
    public bool IsRunning {
      get {
        return _worker.IsBusy;
      }
    }

    public EZBv4Video() {

      _worker.DoWork += _worker_DoWork;
    }

    /// <summary>
    /// Connect and begin receiving the camera stream
    /// </summary>
    public async Task Start(EZB ezb, string ipAddress, int port) {

      stopWorker();

      _ezb = ezb;

      await _tcpClient.Connect(ipAddress, port, 3000);

      _tcpClient.ReceiveTimeout = 3000;
      _tcpClient.SendTimeout = 3000;

      startWorker();
    }

    /// <summary>
    /// Dispose of this object
    /// </summary>
    protected override void DisposeOverride() {

      stopWorker();
    }

    private void startWorker() {

      _worker.RunWorkerAsync();
    }

    private async void stopWorker() {

      await _worker.CancelWorker();
    }

    /// <summary>
    /// Stop the camera from streaming and receiving frames
    /// </summary>
    public void Stop() {

      stopWorker();
    }

    public void CameraLED(bool red, bool green, bool blue) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_LED_Red, red ? (byte)1 : (byte)0 });
      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_LED_Green, green ? (byte)1 : (byte)0 });
      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_LED_Blue, blue ? (byte)1 : (byte)0 });
    }

    public void CameraSetRegValue(byte register, byte value) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_SetReg, (byte)register, (byte)value });
    }

    public void CameraExplosureMode(CameraExposureMode val) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_ExposureMode, (byte)val });
    }

    public void CameraBrightness(byte val) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_brightness, val });
    }

    public void CameraCompression(byte val) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_compression, val });
    }

    public void CameraSetting(CameraSettingsEnum setting) {

      _settingsToSend.Add((byte)setting);
    }

    public void CameraRetryCount(byte count) {

      _settingsToSend.AddRange(new byte[] { (byte)CameraSettingsEnum.RESERVED_SetRetryCount, count });
    }

    /// <summary>
    /// Must be 60,000 or less
    /// </summary>
    public void CameraSetBufferSize(UInt16 size) {

      List<byte> b = new List<byte>();

      b.Add((byte)CameraSettingsEnum.RESERVED_SetBufferSize);
      b.AddRange(BitConverter.GetBytes(size));

      _settingsToSend.AddRange(b.ToArray());
    }

    public void CameraBand(CameraBandEnum band) {

      CameraSetRegValue(0xff, 0x00);

      CameraSetRegValue(0x7c, 0x00);

      CameraSetRegValue(0x7d, (byte)band);

      CameraSetRegValue(0x7c, 0x05);

      CameraSetRegValue(0x7d, 0x80);

      CameraSetRegValue(0x7d, 0x80);
    }


    private async Task _worker_DoWork(Object sender, DoWorkEventArgs e) {

      List<byte> bufferImage = new List<byte>();
      byte[] bufferTmp = new byte[BUFFER_SIZE];

      try {

        if (OnStart != null)
          OnStart();

        _settingsToSend.Add((byte)CameraSettingsEnum.START);

        while (!_worker.CancellationPending) {

          if (_settingsToSend.Count > 0) {

            await _tcpClient.Send(_settingsToSend.ToArray());

            _settingsToSend.Clear();
          }

          bufferImage.AddRange(await _tcpClient.ReadBytes(BUFFER_SIZE));

          int foundStart = Functions.IndexOf(bufferImage.ToArray(), 0, TAG_EZIMAGE);

          if (foundStart == -1)
            continue;

          if (foundStart > 0)
            bufferImage.RemoveRange(0, foundStart);

          if (bufferImage.Count < TAG_EZIMAGE.Length + sizeof(UInt32))
            continue;

          int imageSize = (int)BitConverter.ToUInt32(bufferImage.GetRange(TAG_EZIMAGE.Length, sizeof(UInt32)).ToArray(), 0);

          if (bufferImage.Count <= imageSize + TAG_EZIMAGE.Length + sizeof(UInt32))
            continue;

          bufferImage.RemoveRange(0, TAG_EZIMAGE.Length + sizeof(UInt32));

          _imageSize = imageSize;

          try {

            if (OnImageDataReady != null)
              OnImageDataReady(bufferImage.GetRange(0, imageSize).ToArray());
          } catch (Exception ex) {

            _ezb.Log(false, "ezbv4 camera image render error: {0}", ex);
          }

          bufferImage.RemoveRange(0, imageSize);
        }

        await _tcpClient.Send((byte)CameraSettingsEnum.STOP);

        _tcpClient.Disconnect();

      } catch (Exception ex) {

        _ezb.Log(false, "EZ-B v4 Camera Error: {0}", ex);
      } finally {

        if (OnStop != null)
          OnStop();
      }
    }
  }
}
