namespace EZ_B.Classes {

  public class BlinkMColor {

    public BlinkMColor(byte red, byte green, byte blue) {

      Red = red;
      Green = green;
      Blue = blue;
    }

    public byte Red = 0;
    public byte Green = 0;
    public byte Blue = 0;

    public override string ToString() {

      return string.Format("BlinkM Current Color... R:{0}, G:{1}, B:{2}", Red, Green, Blue);
    }
  }
}
