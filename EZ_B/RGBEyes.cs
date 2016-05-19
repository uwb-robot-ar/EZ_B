using System;
using System.Collections.Generic;

namespace EZ_B {

  public class RGBEyes {

    EZB _ezb;

    /// <summary>
    /// Default I2C Address of the RGB Eyes Module (0xa0)
    /// </summary>
    public static readonly byte I2C_ADDRESS = 0xa0;

    /// <summary>
    /// The maximum brightness that can be sent to the RGB LED (7)
    /// </summary>
    public static readonly byte BRIGHTNESS_MAX = 7;

    /// <summary>
    /// The minimium brightness that can be sent to the RGB LED (0)
    /// </summary>
    public static readonly byte BRIGHTNESS_MIN = 0;

    /// <summary>
    /// The number of RGB LEDs is referenced by the index
    /// </summary>
    public static readonly byte INDEX_MAX = 18;

    private byte [] _reds   = new byte[] { };
    private byte [] _greens = new byte[] { };
    private byte [] _blues  = new byte[] { };

    internal RGBEyes(EZB ezb) {

      _ezb = ezb;

      List<byte> b = new List<byte>();

      for (int x = 0; x < INDEX_MAX; x++)
        b.Add(0);

      _reds = b.ToArray();
      _greens = b.ToArray();
      _blues = b.ToArray();
    }

    /// <summary>
    /// Change the I2C address of the device. Will send the command to the default address.
    /// </summary>
    /// <param name="NewI2CAddress"></param>
    public void ChangeI2CAddress(byte NewI2CAddress) {

      ChangeI2CAddress(I2C_ADDRESS, NewI2CAddress);
    }

    /// <summary>
    /// Change the I2C address of the device.
    /// </summary>
    public void ChangeI2CAddress(byte CurrentI2CAddress, byte NewI2CAddress) {

      _ezb.I2C.Write(CurrentI2CAddress, new byte[] { 30 << 3, NewI2CAddress });
    }

    /// <summary>
    /// Set all of the LED's to the specific color. Sends the command to the default I2C address
    /// </summary>
    public void SetAllColor(byte r, byte g, byte b) {

      SetAllColor(I2C_ADDRESS, r, g, b);
    }

    /// <summary>
    /// Sets all of the LED's to the specific color.
    /// </summary>
    public void SetAllColor(byte I2CAddress, byte r, byte g, byte b) {

      List<byte> i = new List<byte>();

      for (byte x = 0; x < 18; x++)
        i.Add(x);

      SetColor(I2CAddress, i.ToArray(), r, g, b);
    }

    /// <summary>
    /// Set the color of the specified index. Sends the command to the default I2C address
    /// </summary>
    public void SetColor(byte index, byte r, byte g, byte b) {

      SetColor(I2C_ADDRESS, new byte[] { index }, r, g, b);
    }

    /// <summary>
    /// Sets the color of the specified index.
    /// </summary>
    public void SetColor(byte I2CAddress, byte index, byte r, byte g, byte b) {

      SetColor(I2CAddress, new byte[] { index }, r, g, b);
    }

    /// <summary>
    /// Sets the color of the specified indexes within the array. Sends the command to the default I2C Addres
    /// </summary>
    public void SetColor(byte[] indexes, byte r, byte g, byte b) {

      SetColor(I2C_ADDRESS, indexes, r, g, b);
    }

    /// <summary>
    /// Sets the color of the specified indexes within the array.
    /// </summary>
    public void SetColor(byte I2CAddress, byte[] indexes, byte r, byte g, byte b) {

      if (r > BRIGHTNESS_MAX)
        r = BRIGHTNESS_MAX;

      if (g > BRIGHTNESS_MAX)
        g = BRIGHTNESS_MAX;

      if (b > BRIGHTNESS_MAX)
        b = BRIGHTNESS_MAX;

      List<byte> data = new List<byte>();

      foreach (byte index in indexes) {

        if (index > 17)
          throw new Exception(string.Format("Index out of range for RGB Eyes ({0})", index));

        _reds[index] = r;
        _greens[index] = g;
        _blues[index] = b;

        int packet0 = (index << 3) | r;

        int packet1 = (g << 4) | b;

        data.Add((byte)packet0);
        data.Add((byte)packet1);
      }

      _ezb.I2C.Write(I2CAddress, data.ToArray());
    }

    public struct RGBDef {

      public int Index;
      public byte Red;
      public byte Green;
      public byte Blue;
    }

    public void SetColor(RGBDef[] defs) {

      SetColor(I2C_ADDRESS, defs);
    }

    public void SetColor(byte I2CAddress, RGBDef[] defs) {

      List<byte> data = new List<byte>();

      foreach (RGBDef def in defs) {

        byte r = Math.Min(BRIGHTNESS_MAX, def.Red);
        byte g = Math.Min(BRIGHTNESS_MAX, def.Green);
        byte b = Math.Min(BRIGHTNESS_MAX, def.Blue);

        _reds[def.Index] = r;
        _greens[def.Index] = g;
        _blues[def.Index] = b;

        int packet0 = (def.Index << 3) | r;

        int packet1 = (g << 4) | b;

        data.Add((byte)packet0);
        data.Add((byte)packet1);
      }

      _ezb.I2C.Write(I2CAddress, data.ToArray());
    }

    public byte GetRed(byte index) {

      return _reds[index];
    }

    public byte GetGreen(byte index) {

      return _greens[index];
    }

    public byte GetBlue(byte index) {

      return _blues[index];
    }
  }
}
