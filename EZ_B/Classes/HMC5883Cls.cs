using System;

namespace EZ_B.Classes {

  public class HMC5883Cls {

    public int X = 0;
    public int Y = 0;
    public int Z = 0;

    /// <summary>
    /// Degrees are assuming compass is flat with no tilt using X and Y for easy calculation
    /// heading = atan2(x, y) / 0.0174532925
    /// </summary>
    public int Heading {
      get {
        return (int)(Math.Atan2(X, Y) / 0.0174532925) + 180;
      }
    }
  }
}
