using System;
namespace EZ_B.Classes {

  public class MPU9150Cls {

    public int AccelX = 0;
    public int AccelY = 0;
    public int AccelZ = 0;

    public int TmpC = 0;

    public int GyroX = 0;
    public int GyroY = 0;
    public int GyroZ = 0;

    public int CompassX = 0;
    public int CompassY = 0;
    public int CompassZ = 0;

    /// <summary>
    /// Degrees are assuming compass is flat with no tilt using X and Y for easy calculation
    /// heading = atan2(x, y) / 0.0174532925
    /// </summary>
    public int Heading {
      get {

        return (int)(Math.Atan2(CompassY, CompassX) * (180 / 3.14159265) + 180);

//        return (int)(Math.Atan2(CompassX, CompassY) / 0.0174532925) + 180;
      }
    }
  }
}
