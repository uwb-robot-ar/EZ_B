using System;

namespace EZ_B.Classes {

  public class CompassDirection {

    public int  X         = 0;
    public int  Y         = 0;
    public bool Connected = false;
    public int  Offset    = 0;

    public CompassDirection(bool connected) {

      Connected = connected;
    }

    public CompassDirection(int x, int y) {

      X = x;
      Y = y;
      Connected = true;
    }

    public double Degrees {
      get {
        double degrees = (Math.Atan2(Y, X) * (180 / Math.PI)) + 179;
        degrees = (degrees + Offset) % 360;
        return degrees;
      }
    }

    public override string ToString() {

      return string.Format("X: {0}, Y: {1:}, Degrees: {2}", X, Y, Degrees);
    }
  }
}
