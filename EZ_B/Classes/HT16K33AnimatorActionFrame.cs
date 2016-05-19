using System;
using System.Xml.Serialization;

namespace EZ_B.Classes {

  public class HT16K33AnimatorActionFrame {

    public string   GUID             = Guid.NewGuid().ToString();
    public bool []  Data             = new bool[8 * 8];
    public int      PauseTimeMS      = 1000;

    [XmlIgnore]
    public bool[,] Matrix {
      get {

        bool [,] matrix = new bool[8, 8];

        int cnt = 0;

        for (int row = 0; row < 8; row++)
          for (int col = 0; col < 8; col++) {

            matrix[row, col] = Data[cnt];

            cnt++;
          }

        return matrix;
      }
      set {

        int cnt = 0;

        for (int row = 0; row < 8; row++)
          for (int col = 0; col < 8; col++) {

            Data[cnt] = value[row, col];

            cnt++;
          }
      }
    }

    public HT16K33AnimatorActionFrame() {

    }

    public HT16K33AnimatorActionFrame(string guid, int pauseTimeMS) {

      GUID = guid;
      PauseTimeMS = pauseTimeMS;
    }

    public HT16K33AnimatorActionFrame(int pauseTimeMS) {

      PauseTimeMS = pauseTimeMS;
    }

    public override string ToString() {

      return GUID;
    }
  }
}
