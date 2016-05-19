using System;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_B {

  public class LidarLite {

    EZB _ezb;

    public LidarLite(EZB ezb) {

      _ezb = ezb;
    }

    public async Task<int> GetData() {

      _ezb.I2C.Write(0x62, 0x00, 0x04);

      await Task.Delay(50);
 
      _ezb.I2C.Write(0x62, 0x8f);

      byte[] tmp = await _ezb.I2C.Read(0x62, 2);
      byte[] data = tmp.Reverse().ToArray();

      return BitConverter.ToUInt16(data, 0);
    }
  }
}
