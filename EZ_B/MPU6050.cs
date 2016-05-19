using System;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_B {

  public class MPU6050 {

    EZB _ezb;

    public byte DeviceAddress = 0x68;

    public MPU6050(EZB ezb) {

      _ezb = ezb;
    }

    public void Init() {

      _ezb.I2C.Write(DeviceAddress, new byte[] { 0x6b, 0x00 });
    }

    public async Task<Classes.MPU6050Cls> GetData() {

      _ezb.I2C.Write(DeviceAddress, new byte[] { 0x3B });

      byte[] tmp = await _ezb.I2C.Read(DeviceAddress, 14);
      byte[] everything = tmp.Reverse().ToArray();

      Classes.MPU6050Cls cls = new Classes.MPU6050Cls();

      cls.AccelX = BitConverter.ToInt16(everything, 12);
      cls.AccelY = BitConverter.ToInt16(everything, 10);
      cls.AccelZ = BitConverter.ToInt16(everything, 8);
      cls.TmpC = Convert.ToInt16((BitConverter.ToUInt16(everything, 6) / 340) + 36.53) / 10;
      cls.GyroX = BitConverter.ToInt16(everything, 4);
      cls.GyroY = BitConverter.ToInt16(everything, 2);
      cls.GyroZ = BitConverter.ToInt16(everything, 0);

      return cls;
    }
  }
}
