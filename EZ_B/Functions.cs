using System;
using System.Text;
using System.Text.RegularExpressions;

namespace EZ_B {

  public static class Functions {

    /// <summary>
    /// Displays the bit sequence of an integer value.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <returns>A string with the bit pattern representing the integer.</returns>
    /// <example>The integer value '751' would result in '00000000000000000000001011101111', can be verified with calculator.</example>
    /// <remarks>This is a method used frequently during testing.</remarks>
    public static string DisplayBitSequence(int value) {

      StringBuilder buffer = new StringBuilder();

      byte[] byteBuffer = BitConverter.GetBytes(value);

      Array.Reverse(byteBuffer);

      foreach (byte byteValue in byteBuffer) {

        for (int count = 128; count > 0; count /= 2) {

          if ((byteValue & count) != 0)
            buffer.Append("1");
          if ((byteValue & count) == 0)
            buffer.Append("0");
        }

        buffer.Append(" ");
      }

      return buffer.ToString();
    }

    /// <summary>
    /// Sets the bit in an integer value at the requested position.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <param name="position">The position at which to set the bit.</param>
    /// <returns>The integer value with the bit set.</returns>
    public static int SetBitValue(int value, int position) {
      return value |= (1 << position);
    }

    /// <summary>
    /// Clears the bit in an integer value at the requested position.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <param name="position">The position at which to clear the bit.</param>
    /// <returns>The integer value with the bit cleared.</returns>
    public static int ClearBitValue(int value, int position) {

      return value &= ~(1 << position);
    }

    /// <summary>
    /// Flips the bit in an integer value at the requested position.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <param name="position">The position at which to flip the bit.</param>
    /// <returns>The integer value with the bit flipped.</returns>
    public static int FlipBitValue(int value, int position) {

      return value ^= (1 << position);
    }

    /// <summary>
    /// Converts the string to a byte array containing the ASCII values of each char.
    /// </summary>
    /// <param name="ssid">The ssid.</param>
    /// <returns></returns>
    public static byte[] ConvertStringToByteArray(string ssid) {

      int numChars = ssid.Length;
      byte[] bytes = new byte[32];

      for (int index = 0; index < numChars; index++)
        bytes[index] = (byte)ssid[index];

      return bytes;

      //return Encoding.ASCII.GetBytes(ssid);
    }

    /// <summary>
    /// Converts the byte array to a string.
    /// </summary>
    /// <param name="buffer">The byte sequence.</param>
    /// <returns>A string representing the byte array.</returns>
    public static string ConvertByteArrayToString(byte[] buffer) {

      return ConvertByteArrayToString(buffer, buffer.Length);
    }

    /// <summary>
    /// Converts the byte array to a string.
    /// </summary>
    /// <param name="buffer">The byte sequence.</param>
    /// <param name="length">The byte count to use for conversion.</param>
    /// <returns>A string representing the byte array.</returns>
    public static string ConvertByteArrayToString(byte[] buffer, int length) {

      return Encoding.UTF8.GetString(buffer, 0, length);
    }

    /// <summary>
    /// Convert ascii object to a decimal value
    /// </summary>
    public static string ConvertToDecimal(object asciiObj) {

      StringBuilder sb = new StringBuilder();

      foreach (byte b in Encoding.UTF8.GetBytes(asciiObj.ToString()))
        sb.AppendFormat("{0} ", b);

      return sb.ToString();
    }

    /// <summary>
    /// Returns an IEnumerable of input list split into the number of specified parts
    /// </summary>
    public static T[][] Chunk<T>(this T[] arrayIn, int length) {

      bool even = arrayIn.Length % length == 0;
      int totalLength = arrayIn.Length / length;

      if (!even)
        totalLength++;

      T[][] newArray = new T[totalLength][];

      for (int i = 0; i < totalLength; ++i) {

        int allocLength = length;

        if (!even && i == totalLength - 1)
          allocLength = arrayIn.Length % length;

        newArray[i] = new T[allocLength];
        Array.Copy(arrayIn, i * length, newArray[i], 0, allocLength);
      }

      return newArray;
    }

    /// <summary>
    /// Returns true if the InObj is a byte value 
    /// </summary>
    public static bool IsByte(object InObj) {

      try {

        Convert.ToByte(InObj);

        return true;
      } catch {

        return false;
      }
    }

    /// <summary>
    /// Returns true if the InObj is a numerical value (including int and floating point)
    /// </summary>
    public static bool IsNumeric(object InObj) {

      try {

        Convert.ToDecimal(InObj);

        return true;
      } catch {

        return false;
      }
    }

    /// <summary>
    /// Returns true if the mainValue is larger than all other values
    /// </summary>
    public static bool IsLargerThan(int mainValue, params int[] values) {

      foreach (int value in values)
        if (mainValue < value)
          return false;

      return true;
    }

    /// <summary>
    /// Returns true if the mainValue is equal to any other values
    /// </summary>
    public static bool IsEqualToo(int mainValue, params int[] values) {

      foreach (int value in values)
        if (mainValue == value)
          return true;

      return false;
    }

    public class Range {

      public decimal Left;
      public decimal Right;

      public Range() {
      }

      public Range(decimal left, decimal right) {

        Left = left;
        Right = right;
      }

      public Range(int left, int right) {

        Left = left;
        Right = right;
      }
    }

    /// <summary>
    /// Returns true if the number falls within the high and low range
    /// </summary>
    public static bool WithinRange(decimal number, Range range) {

      if (number >= range.Left && number <= range.Right)
        return true;

      return false;
    }

    /// <summary>
    /// Extension of the String.Contains but allows an array of items to check for rather than just one.
    /// </summary>
    public static bool Contains(bool ignoreCase, object inStr, params string[] containTxt) {

      if (ignoreCase) {

        foreach (string ct in containTxt)
          if (inStr.ToString().ToLower().Contains(ct.ToLower()))
            return true;
      } else {

        foreach (string ct in containTxt)
          if (inStr.ToString().Contains(ct))
            return true;
      }

      return false;
    }

    /// <summary>
    /// Returns true if the difference between Master and Compare is greater then Diff
    /// </summary>
    public static bool Diff(int diff, int master, int compare) {

      if (System.Math.Abs(master - compare) > diff)
        return true;

      return false;
    }

    /// <summary>
    /// Returns true if the difference between Master and Compare is greater then Diff
    /// </summary>
    public static bool Diff(decimal diff, decimal master, decimal compare) {

      if (System.Math.Abs(master - compare) > diff)
        return true;

      return false;
    }

    /// <summary>
    /// Returns true if the specified bit in the byte is 1. false if not. 0 is LSB, 7 is MSB
    /// </summary>
    public static bool IsBitSet(int b, int pos) {

      return (b & (1 << pos)) != 0;
    }

    /// <summary>
    /// Converts a byte to a binary string
    /// </summary>
    public static string ByteToBinaryString(byte b) {

      StringBuilder sb = new StringBuilder();

      for (int x = 7; x >= 0; x--)
        if (IsBitSet(b, x))
          sb.Append("1");
        else
          sb.Append("0");

      return sb.ToString();
    }

    /// <summary>
    /// Converts a byte to a binary string
    /// </summary>
    public static string ByteToBinaryString(byte b, string seperator) {

      StringBuilder sb = new StringBuilder();

      for (int x = 7; x >= 0; x--) {

        if (IsBitSet(b, x))
          sb.Append("1");
        else
          sb.Append("0");

        if (x > 0)
          sb.Append(seperator);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Returns a byte from specified binary. LSB is val0. MSB is val7
    /// </summary>
    public static byte ToByteFromBinary(bool val7, bool val6, bool val5, bool val4, bool val3, bool val2, bool val1, bool val0) {

      int b = 0;

      b += (val7 ? 128 : 0);
      b += (val6 ? 64 : 0);
      b += (val5 ? 32 : 0);
      b += (val4 ? 16 : 0);
      b += (val3 ? 8 : 0);
      b += (val2 ? 4 : 0);
      b += (val1 ? 2 : 0);
      b += (val0 ? 1 : 0);

      return Convert.ToByte(b);
    }

    /// <summary>
    /// Returns a byte out of the binary. The inputs for each bit an either be a 0 or a 1. The LSB is val0. MSB is val7
    /// </summary>
    public static byte ToByteFromBinary(int val7, int val6, int val5, int val4, int val3, int val2, int val1, int val0) {

      int b = 0;

      b += (val7 == 1 ? 128 : 0);
      b += (val6 == 1 ? 64 : 0);
      b += (val5 == 1 ? 32 : 0);
      b += (val4 == 1 ? 16 : 0);
      b += (val3 == 1 ? 8 : 0);
      b += (val2 == 1 ? 4 : 0);
      b += (val1 == 1 ? 2 : 0);
      b += (val0 == 1 ? 1 : 0);

      return Convert.ToByte(b);
    }

    /// <summary>
    /// Returns a scalar. Used for converting one range into another range. (i.e. Wii Input Remote X/Y/Z to Servo Positions)
    /// </summary>
    public static float GetScalarFromRange(int containerMax, float inputMax, float inputMin) {

      return (float)containerMax / Math.Abs(inputMax - inputMin);
    }

    /// <summary>
    /// Returns a scalar. Used for converting one range into another range. (i.e. Wii Input Remote X/Y/Z to Servo Positions)
    /// </summary>
    public static float GetScalarFromRange(int containerMax, int inputMax, int inputMin) {

      return GetScalarFromRange(containerMax, (float)inputMax, (float)inputMin);
    }

    /// <summary>
    /// Returns a scalar. Used for converting one range into another range. (i.e. Wii Input Remote X/Y/Z to Servo Positions)
    /// </summary>
    public static float GetScalarFromRange(byte containerMax, byte inputMax, byte inputMin) {

      return GetScalarFromRange(containerMax, (float)inputMax, (float)inputMin);
    }

    /// <summary>
    /// Converts a Float to an IEEE754 Compliant Integer
    /// </summary>
    public static int SingleToInt32Bits(float value) {

      return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    }

    /// <summary>
    /// Returns the shortest angle between two angles (Absolute, no negatives)
    /// </summary>
    public static int GetShortestAngle(int angle1, int angle2) {

      double angleDiff = Math.Abs(angle1 - angle2) % 360;

      if (angleDiff > 180)
        angleDiff = 360 - angleDiff;

      return (int)angleDiff;
    }

    /// <summary>
    /// Get the angle of the second point relative to the first point
    /// </summary>
    public static int GetAngle(int px1, int py1, int px2, int py2) {

      return GetAngle((double)px1, (double)py1, (double)px2, (double)py2);
    }

    /// <summary>
    /// Get the angle of the second point relative to the first point
    /// </summary>
    public static int GetAngle(decimal px1, decimal py1, decimal px2, decimal py2) {

      return GetAngle((double)px1, (double)py1, (double)px2, (double)py2);
    }

    /// <summary>
    /// Get the angle of the second point relative to the first point
    /// </summary>
    public static int GetAngle(double px1, double py1, double px2, double py2) {

      // Negate X and Y values 
      double pxRes = px2 - px1;
      double pyRes = py2 - py1;
      double angle = 0.0;

      // Calculate the angle 
      if (pxRes == 0.0) {

        if (pyRes > 0.0)
          angle = System.Math.PI / 2.0;
        else
          angle = System.Math.PI * 3.0 / 2.0;
      } else if (pyRes == 0.0) {

        if (pxRes > 0.0)
          angle = 0.0;
        else
          angle = System.Math.PI;
      } else {

        if (pxRes < 0.0)
          angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;
        else if (pyRes < 0.0)
          angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
        else
          angle = System.Math.Atan(pyRes / pxRes);
      }

      // Convert to degrees 
      angle = angle * 180 / System.Math.PI;

      // Rotate degrees so 0 is pointing up (instead of to the right)
      angle = Math.Abs(angle + 90) % 360;

      return (int)angle;
    }

    /// <summary>
    /// Returns the distance between two points on a 2d vector
    /// </summary>
    public static int GetDistance(int x1, int y1, int x2, int y2) {

      double dx = x1 - x2;         // horizontal difference 
      double dy = y1 - y2;         // vertical difference 

      // Pythagoras theorem
      return (int)Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Remove all html tags
    /// </summary>
    /// <param name="inStr"></param>
    /// <returns></returns>
    public static string StripHTML(string inStr) {

      return Regex.Replace(inStr, "<.*?>", string.Empty);
    }

    /// <summary>
    /// Clamp a value to 0-255
    /// </summary>
    public static int Clamp(int i) {

      if (i < 0)
        return 0;
      if (i > 255)
        return 255;

      return i;
    }

    /// <summary>
    ///  Returns the degree X co-ordinate for a circle
    ///  i.e. Plot(DegX(10, 20), DegY(10, 20));
    /// </summary>
    public static double DegX(double deg, double diam) {

      deg = deg + 90;

      return System.Math.Cos(deg / (180 / System.Math.PI)) * diam;
    }

    /// <summary>
    ///  Returns the degree Y co-ordinate for a circle
    ///  i.e. Plot(DegX(10, 20), DegY(10, 20));
    /// </summary>
    public static double DegY(double deg, double diam) {

      deg = deg + 90;

      return System.Math.Sin(deg / (180 / System.Math.PI)) * diam;
    }

    public static int Min(params int[] vals) {

      int min = vals[0];

      foreach (byte b in vals)
        if (b < min)
          min = b;

      return min;
    }

    public static int Max(params int[] vals) {

      int max = vals[0];

      foreach (byte b in vals)
        if (b > max)
          max = b;

      return max;
    }

        public static unsafe int  IndexOf(byte[] Haystack, int offset, byte[] Needle)
        {

            if (Haystack.Length <= offset)
                return -1;

            fixed (byte* H = Haystack)

            fixed (byte* N = Needle)
            {

                int i = 0;

                for (byte* hNext = H + offset, hEnd = H + Haystack.Length; hNext < hEnd; i++, hNext++)
                {

                    bool Found = true;

                    for (byte* hInc = hNext, nInc = N, nEnd = N + Needle.Length; Found && nInc < nEnd; Found = *nInc == *hInc, nInc++, hInc++)
                        ;

                    if (Found)
                        return i;
                }

                return -1;
            }
        }
    }
}

