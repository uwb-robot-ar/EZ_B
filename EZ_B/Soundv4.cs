using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EZ_B {

  public class Soundv4 : DisposableBase {

    EZB _ezb;
    MemoryStream _ms = new MemoryStream();
    EZBGWorker _threadAudio = new EZBGWorker("Soundv4 Main Thread");
    EZBGWorker _threadProgress = new EZBGWorker("Soundv4 Process Thread");
    int _playFromSample = 0;
    Stopwatch _sw = new Stopwatch();

    /// <summary>
    /// The recommended size of the the audio packets
    /// </summary>
    public static readonly int RECOMMENDED_PACKET_SIZE = 256;

    /// <summary>
    /// The recommended size of the prebuffer before playing the audio
    /// </summary>
    public static readonly int RECOMMENDED_PREBUFFER_SIZE = 20000;

    /// <summary>
    /// The sample rate at which the data is played back on the EZ-B
    /// </summary>
    public static readonly int AUDIO_SAMPLE_BITRATE = 14700;

    /// <summary>
    /// The size of each packet which is transmitted over the wire to the EZ-B.
    /// </summary>
    public int PACKET_SIZE = RECOMMENDED_PACKET_SIZE;

    /// <summary>
    /// The ammount of data to prebuffer to the EZ-B before playing the audio. The EZ-B has a 50k buffer, so this value cannot be any higher than that.
    /// </summary>
    public int PREBUFFER_SIZE = RECOMMENDED_PREBUFFER_SIZE;

    /// <summary>
    /// Event exceuted when new data is being sent to the EZ-B
    /// </summary>
    public delegate void OnAudioDataHandler(byte[] data, int minVal, int maxVal, int avgVal);

    /// <summary>
    /// Event executed when the volume value has changed
    /// </summary>
    public delegate void OnVolumeChangedHandler(decimal volume);

    /// <summary>
    /// Event executed when the audio has stopped playing
    /// </summary>
    public delegate void OnStopPlayingHandler();

    /// <summary>
    /// Event executed when the audio has begun playing
    /// </summary>
    public delegate void OnStartPlayingHandler();

    /// <summary>
    /// Event executed when the audio level is clipping. This means the volume value is amplifying the audio past the limits
    /// </summary>
    public delegate void OnClippingStatusHandler(bool isClipping);

    /// <summary>
    /// Event executed with the playing progress by sample position. The resolution of this event can be specified with Play method.
    /// In summary, you set the Play Positions by the sample index and this event will execute when the playing reaches that particular sample point.
    /// If you simply want an update of the curernt play time, use the OnPlayTime event.
    /// </summary>
    public delegate void OnProgressHandler(int samplePosition);

    /// <summary>
    /// Event executed with the playing progress by sample position with 1000ms resolution.
    /// </summary>
    public delegate void OnPlayTimeHandler(TimeSpan ts);

    public event OnAudioDataHandler OnAudioDataChanged;
    public event OnVolumeChangedHandler OnVolumeChanged;
    public event OnStopPlayingHandler OnStopPlaying;
    public event OnStartPlayingHandler OnStartPlaying;
    public event OnClippingStatusHandler OnClippingStatus;
    public event OnProgressHandler OnPlayProgress;
    public event OnPlayTimeHandler OnPlayTime;

    private decimal _volume = 100;
    private int[] _progressPositions = new int[] { };

    public decimal Volume {
      get {
        return _volume;
      }
      set {

        if (_volume != value && OnVolumeChanged != null)
          OnVolumeChanged(value);

        _volume = value;
      }
    }

    protected internal Soundv4(EZB ezb) {

      _ezb = ezb;

      _threadAudio.DoWork += _threadAudio_DoWork;
      _threadAudio.RunWorkerCompleted += _threadAudio_RunWorkerCompleted;

      _threadProgress.DoWork += _threadProgress_DoWork;
    }

    /// <summary>
    /// Play the Audio Data out of the EZ-B. 
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayDataWait(byte[] data) {

      await PlayDataWait(data, _volume);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(byte[] data) {

      await PlayData(data, _volume, new int[] { });
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(byte[] data, decimal volume) {

      await PlayData(data, volume, new int[] { });
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(byte[] data, decimal volume, int[] progressPositions) {

      using (MemoryStream ms = new MemoryStream(data))
        await PlayData(ms, _volume);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayDataWait(byte[] data, decimal volume) {

      using (MemoryStream ms = new MemoryStream(data))
        await PlayDataWait(ms, _volume);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms) {

      await PlayData(ms, _volume);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms, int[] progressPositions) {

      await PlayData(ms, _volume, progressPositions);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms, int[] progressPositions, int playFromSample) {

      await PlayData(ms, _volume, progressPositions, playFromSample);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayDataWait(Stream ms) {

      await PlayDataWait(ms, _volume);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms, decimal volume) {

      await PlayData(ms, volume, new int[] { });
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms, decimal volume, int[] progressPositions) {

      await PlayData(ms, volume, progressPositions, 0);
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayData(Stream ms, decimal volume, int[] progressPositions, int playFromSample) {

      await Stop();

      if (!_ezb.IsConnected)
        return;

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4) {

        _ezb.Log(false, "This feature is only available for EZ-B v4");

        return;
      }

      if (_ms != null)
        _ms.Dispose();

      _ms = new MemoryStream();

      ms.CopyTo(_ms);

      byte[] bTmp = new byte[PREBUFFER_SIZE];

      _ms.Write(bTmp, 0, bTmp.Length);

      _playFromSample = playFromSample;

      _ms.Position = playFromSample;

      _progressPositions = progressPositions;

      Volume = volume;

      _threadAudio.RunWorkerAsync();
    }

    /// <summary>
    /// Stream raw audio data to the EZ-B v4's speakers.
    /// 0 is silent, 100 is normal, 200 is 2x gain, 300 is 3x gain, etc.
    /// The audio must be RAW 8 Bit at 18 KHZ Sample Rate
    /// </summary>
    public async Task PlayDataWait(Stream ms, decimal volume) {

      await Stop();

      if (!_ezb.IsConnected)
        return;

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4) {

        _ezb.Log(false, "This feature is only available for EZ-B v4");

        return;
      }

      if (_ms != null)
        _ms.Dispose();

      _ms = new MemoryStream();

      ms.CopyTo(_ms);

      byte[] bTmp = new byte[PREBUFFER_SIZE];

      _ms.Write(bTmp, 0, bTmp.Length);

      _ms.Position = 0;

      Volume = volume;

      await _threadAudio_DoWork(null, null);
    }

    private async Task _threadProgress_DoWork(Object sender, DoWorkEventArgs e) {

      int lastSamplePos = _playFromSample;
      TimeSpan offset = TimeSpan.FromSeconds(_playFromSample / AUDIO_SAMPLE_BITRATE);

      do {

        TimeSpan currentPos = _sw.Elapsed + offset;

        double currentSamplePos = ((currentPos.TotalMilliseconds / 1000) * (double)AUDIO_SAMPLE_BITRATE);

        foreach (int ppos in _progressPositions) {

          if (lastSamplePos < ppos && currentSamplePos >= ppos && OnPlayProgress != null) {

            lastSamplePos = ppos;

            OnPlayProgress(ppos);

            break;
          }
        }

        if (OnPlayTime != null)
          OnPlayTime(currentPos);

        if (_threadAudio == null || !_sw.IsRunning)
          return;

        await Task.Delay(50);

      } while (_threadAudio.IsBusy && _sw.IsRunning);
    }

    private async Task _threadAudio_DoWork(Object sender, DoWorkEventArgs e) {

      try {

        if (OnStartPlaying != null)
          OnStartPlaying();

        await _ezb.sendCommand(EZB.CommandEnum.CmdSoundStreamCmd, (byte)EZB.CmdSoundv4Enum.CmdSoundInitStop);

        bool playing = false;

        int position = 0;

        _sw.Restart();

        _threadProgress.RunWorkerAsync();

        do {

          byte[] bTmp = new byte[PACKET_SIZE];

          int bytesRead = _ms.Read(bTmp, 0, PACKET_SIZE);

          position += bytesRead;

          byte[] bArray = new byte[bytesRead];

          bool isClipping = false;

          int highest = 0;
          int lowest = 255;
          int average = 0;
          long total = 0;
          decimal volumeMultiplier = Volume / 100m;

          for (int x = 0; x < bytesRead; x++) {

            decimal newVal = (decimal)bTmp[x];

            if (newVal > 128)
              newVal = Math.Max(128, 128 + ((newVal - 128) * volumeMultiplier));
            else if (newVal < 128)
              newVal = Math.Min(128, 128 - ((128 - newVal) * volumeMultiplier));

            if (newVal > 255) {

              newVal = 255;

              isClipping = true;
            } else if (newVal < 0) {

              newVal = 0;

              isClipping = true;
            }

            highest = Math.Max(highest, (int)newVal);
            lowest = Math.Min(lowest, (int)newVal);
            total += (int)newVal;

            bArray[x] = (byte)newVal;
          }

          average = (int)(total / bytesRead);

          List<byte> dataTmp = new List<byte>();
          dataTmp.Add((byte)EZB.CmdSoundv4Enum.CmdSoundLoad);
          dataTmp.AddRange(BitConverter.GetBytes((UInt16)bArray.Length));
          dataTmp.AddRange(bArray);

          await _ezb.sendCommand(EZB.CommandEnum.CmdSoundStreamCmd, dataTmp.ToArray());

          if (!playing && position > PREBUFFER_SIZE) {

            await _ezb.sendCommand(EZB.CommandEnum.CmdSoundStreamCmd, (byte)EZB.CmdSoundv4Enum.CmdSoundPlay);

            playing = true;
          }

          if (OnAudioDataChanged != null)
            OnAudioDataChanged(dataTmp.ToArray(), lowest, highest, average);

          if (OnClippingStatus != null)
            OnClippingStatus(isClipping);

          float runtime = (float)_sw.ElapsedMilliseconds;

          float shouldSend = ((runtime * AUDIO_SAMPLE_BITRATE) / 1000) + PREBUFFER_SIZE;

          float difference = position - shouldSend;

          if (difference > 0) {

            float delay = (difference / AUDIO_SAMPLE_BITRATE) * 1000;

            await Task.Delay((int)delay);
          }

        } while (position < _ms.Length && _ezb.IsConnected && !_threadAudio.CancellationPending);
      } catch (Exception ex) {

        _ezb.Log(false, "Error Streaming Audio: {0}", ex);
      }
    }

    private async void _threadAudio_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e) {

      _sw.Stop();

      await _ezb.sendCommand(EZB.CommandEnum.CmdSoundStreamCmd, (byte)EZB.CmdSoundv4Enum.CmdSoundInitStop);

      if (OnClippingStatus != null)
        OnClippingStatus(false);

      if (OnStopPlaying != null)
        OnStopPlaying();
    }

    /// <summary>
    /// Stop the audio which is being played
    /// </summary>
    public async Task Stop() {

      await _threadAudio.CancelWorker();
    }

    /// <summary>
    /// Dispose of the AutoPositioner
    /// </summary>
    protected async override void DisposeOverride() {

      OnStartPlaying = null;
      OnStopPlaying = null;

      await _threadProgress.CancelWorker();

      await _threadAudio.CancelWorker();
    }
  }
}
