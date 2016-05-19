using System;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_B {

  public class HMC5883 {

    EZB _ezb;

    public HMC5883(EZB ezb) {

      _ezb = ezb;
    }

    public void Init() {

      _ezb.I2C.Write(0x1E, new byte[] { 0x02, 0x00 });
    }

    public async Task<Classes.HMC5883Cls> GetData() {

      _ezb.I2C.Write(0x1E, new byte[] { 0x03 });

      byte[] tmp = await _ezb.I2C.Read(0x1E, 6);

      byte[] everything = tmp.Reverse().ToArray();

      Classes.HMC5883Cls cls = new Classes.HMC5883Cls();

      cls.X = BitConverter.ToInt16(everything, 4);
      cls.Z = BitConverter.ToInt16(everything, 2);
      cls.Y = BitConverter.ToInt16(everything, 0);
      
      return cls;
    }
  }
}
