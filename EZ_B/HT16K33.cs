using System.Collections.Generic;

namespace EZ_B {

  public class HT16K33 {

    EZB _ezb;

    /// <summary>
    /// Default I2C Address of the HT16K33 Module (0x70)
    /// </summary>
    public byte I2C_ADDRESS = 0x70;

    /// <summary>
    /// The maximum brightness that can be sent to the LED (15)
    /// </summary>
    public static readonly byte BRIGHTNESS_MAX = 15;

    /// <summary>
    /// The minimium brightness that can be sent to the LED (0)
    /// </summary>
    public static readonly byte BRIGHTNESS_MIN = 0;

    bool [,] _matrix = new bool[8, 8];

    public HT16K33(EZB ezb) {

      _ezb = ezb;

      for (int row = 0; row < 8; row++)
        for (int col = 0; col < 8; col++)
          _matrix[row, col] = false;
    }

    /// <summary>
    /// Initialize the HT16K33 by enabling the oscillator and setting the brightness to 15
    /// </summary>
    public void Init() {

      // init oscillator
      _ezb.I2C.Write(I2C_ADDRESS, new byte[] { 0x21 });

      // display on with no flashing
      _ezb.I2C.Write(I2C_ADDRESS, new byte[] { 0x80 | (0x00 << 1) | 0x01 });

      // set brightness
      _ezb.I2C.Write(I2C_ADDRESS, new byte[] { 0xe0 | 15 });
    }

    /// <summary>
    /// Set or Get the matrix data (8x8 array)
    /// </summary>
    public bool[,] Matrix {
      get {
        return _matrix;
      }
      set {
        _matrix = value;
      }
    }

    /// <summary>
    /// Sets all of the LED's to the specific color.
    /// </summary>
    public void SetAllStatus(bool status) {

      for (int row = 0; row < 8; row++)
        for (int col = 0; col < 8; col++)
          _matrix[row, col] = status;
    }

    /// <summary>
    /// Set the LED status in the array
    /// *Note: This will not actually change the physical LED. You must call Update() to update the array
    /// </summary>
    public void SetLED(int row, int col, bool status) {

      _matrix[row, col] = status;
    }

    /// <summary>
    /// Return the status of the LED in the array
    /// </summary>
    public bool GetLED(int row, int col) {

      return _matrix[row, col];
    }

    /// <summary>
    /// Update the LEDs with the current matrix. Also sets the current matrix to this value
    /// </summary>
    /// <param name="matrix"></param>
    public void UpdateLEDs(bool[,] matrix) {

      _matrix = matrix;

      UpdateLEDs();
    }

    /// <summary>
    /// Update the LEDs with the current matrix
    /// </summary>
    public void UpdateLEDs() {

      List<byte> list = new List<byte>();

      list.Add(0x00);

      for (int row = 0; row < 8; row++) {

        int i = 0;

        for (int col = 0; col < 8; col++) {

          int c = col;

          c--;

          if (c < 0)
            c = 7;

          if (_matrix[row, 7 - col])
            i = Functions.SetBitValue(i, c);
          else
            i = Functions.ClearBitValue(i, c);
        }

        list.Add((byte)i);
        list.Add(0x00);
      }

      _ezb.I2C.Write(I2C_ADDRESS, list.ToArray());
    }
  }
}
