using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZ_B.Classes {

  public class ServoItem {

    public Servo.ServoPortEnum Port;
    public int Position;

    public ServoItem(Servo.ServoPortEnum port, int position) {

      Port = port;
      Position = position;
    }
  }
}
