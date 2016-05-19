using System;
using System.Collections.Generic;

namespace EZ_B {

  public class MusicSynth : DisposableBase {

    EZB _ezb;

    protected internal MusicSynth(EZB ezb) {

      _ezb = ezb;
    }

    public enum SignalTypeEnum {

      Sine,
      Square,
      Triangle,
      Sawtooth,

      Pulse,
      WhiteNoise,    
      GaussNoise,	   
      DigitalNoise
    }

    private SignalTypeEnum signalType = SignalTypeEnum.Sine;

    /// <summary>
    /// Signal Type.
    /// </summary>
    public SignalTypeEnum SignalType {
      get {
        return signalType;
      }
      set {
        signalType = value;
      }
    }

    private float frequency = 1f;

    private float phase = 0f;
    /// <summary>
    /// Signal Phase.
    /// </summary>
    public float Phase {
      get {
        return phase;
      }
      set {
        phase = value;
      }
    }

    private float amplitude = 127;

    private float offset = 127;

    private float invert = 1; // Yes=-1, No=1
    /// <summary>
    /// Signal Inverted?
    /// </summary>
    public bool Invert {
      get {
        return invert == -1;
      }
      set {
        invert = value ? -1 : 1;
      }
    }

    /// <summary>
    /// Random provider for noise generator
    /// </summary>
    private Random random = new Random();

    public delegate float GetValueDelegate(float time);

    public enum NotesEnum {

      C1 = 130,
      Db1 = 138,
      D1 = 146,
      Eb1 = 155,
      E1 = 164,
      F1 = 174,
      Gb1 = 184,
      G1 = 195,
      Ab1 = 207,
      A1 = 220,
      Bb1 = 233,
      B1 = 246,

      C2 = 261,
      Db2 = 277,
      D2 = 293,
      Eb2 = 311,
      E2 = 329,
      F2 = 349,
      Gb2 = 369,
      G2 = 391,
      Ab2 = 415,
      A2 = 440,
      Bb2 = 466,
      B2 = 493,

      C3 = 523,
      Db3 = 554,
      D3 = 587,
      Eb3 = 622,
      E3 = 659,
      F3 = 698,
      Gb3 = 739,
      G3 = 783,
      Ab3 = 830,
      A3 = 880,
      Bb3 = 932,
      B3 = 987
    }

    public void PlayNoteWait(NotesEnum note, int lengthMs) {

      PlayNoteWait((float)note / 7672.90043f, lengthMs);
    }

    public void PlayNoteWait(float freq, int lengthMs) {

      frequency = freq;

      List<byte> b = new List<byte>();

      int length = Convert.ToInt16((float)EZ_B.Soundv4.AUDIO_SAMPLE_BITRATE * ((float)lengthMs / 1000f));

      for (int x=0; x < length; x++) {

        float f = getValue(x);

        b.Add((byte)f);
      }

      _ezb.SoundV4.PlayDataWait(b.ToArray());
    }

    public void PlayNote(NotesEnum note, int lengthMs) {

      PlayNote((float)note / 7672.90043f, lengthMs);
    }

    public void PlayNote(float freq, int lengthMs) {

      frequency = freq;

      List<byte> b = new List<byte>();

      int length = Convert.ToInt16((float)EZ_B.Soundv4.AUDIO_SAMPLE_BITRATE * ((float)lengthMs / 1000f));

      for (int x=0; x < length; x++) {

        float f = getValue(x);

        b.Add((byte)f);
      }

      _ezb.SoundV4.PlayData(b.ToArray());
    }

    private float getValue(float time) {

      float value = 0f;
      float t = frequency * time + phase;

      switch (signalType) { // http://en.wikipedia.org/wiki/Waveform
        case SignalTypeEnum.Sine: // sin( 2 * pi * t )
          value = (float)Math.Sin(2f * Math.PI * t);
          break;
        case SignalTypeEnum.Square: // sign( sin( 2 * pi * t ) )
          value = Math.Sign(Math.Sin(2f * Math.PI * t));
          break;
        case SignalTypeEnum.Triangle: // 2 * abs( t - 2 * floor( t / 2 ) - 1 ) - 1
          value = 1f - 4f * (float)Math.Abs(Math.Round(t - 0.25f) - (t - 0.25f));
          break;
        case SignalTypeEnum.Sawtooth: // 2 * ( t/a - floor( t/a + 1/2 ) )
          value = 2f * (t - (float)Math.Floor(t + 0.5f));
          break;

        case SignalTypeEnum.Pulse: // http://en.wikipedia.org/wiki/Pulse_wave
          value = (Math.Abs(Math.Sin(2 * Math.PI * t)) < 1.0 - 10E-3) ? (0) : (1);
          break;
        case SignalTypeEnum.WhiteNoise: // http://en.wikipedia.org/wiki/White_noise
          value = 2f * (float)random.Next(int.MaxValue) / int.MaxValue - 1f;
          break;
        case SignalTypeEnum.GaussNoise: // http://en.wikipedia.org/wiki/Gaussian_noise
          value = (float)StatisticFunction.NORMINV((float)random.Next(int.MaxValue) / int.MaxValue, 0.0, 0.4);
          break;
        case SignalTypeEnum.DigitalNoise: //Binary Bit Generators
          value = random.Next(2);
          break;
      }

      return (invert * amplitude * value + offset);
    }

    protected override void DisposeOverride() {

    }
  }

  public class StatisticFunction {

    public static double Mean(double[] values) {

      double tot = 0;
      
      foreach (double val in values)
        tot += val;

      return (tot / values.Length);
    }

    public static double StandardDeviation(double[] values) {

      return Math.Sqrt(Variance(values));
    }

    public static double Variance(double[] values) {

      double m = Mean(values);
      double result = 0;
      
      foreach (double d in values)
        result += Math.Pow((d - m), 2);

      return (result / values.Length);
    }

    public static double NORMSINV(double p) {

      double[] a = {-3.969683028665376e+01,  2.209460984245205e+02,
				-2.759285104469687e+02,  1.383577518672690e+02,
				-3.066479806614716e+01,  2.506628277459239e+00};

      double[] b = {-5.447609879822406e+01,  1.615858368580409e+02,
				-1.556989798598866e+02,  6.680131188771972e+01,
				-1.328068155288572e+01 };

      double[] c = {-7.784894002430293e-03, -3.223964580411365e-01,
				-2.400758277161838e+00, -2.549732539343734e+00,
				4.374664141464968e+00,  2.938163982698783e+00};

      double[] d = { 7.784695709041462e-03,  3.224671290700398e-01,
				2.445134137142996e+00,  3.754408661907416e+00};

      // Define break-points.
      double plow  = 0.02425;
      double phigh = 1 - plow;

      // Rational approximation for lower region:
      if (p < plow) {

        double q  = Math.Sqrt(-2 * Math.Log(p));

        return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
          ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
      }

      // Rational approximation for upper region:
      if (phigh < p) {

        double q  = Math.Sqrt(-2 * Math.Log(1 - p));
        
        return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
          ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
      }

      // Rational approximation for central region:
      {

        double q = p - 0.5;
        
        double r = q * q;
        
        return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
          (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
      }
    }

    public static double NORMINV(double probability, double mean, double standard_deviation) {

      return (NORMSINV(probability) * standard_deviation + mean);
    }

    public static double NORMINV(double probability, double[] values) {
      
      return NORMINV(probability, Mean(values), StandardDeviation(values));
    }
  }
}
