using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZ_B {

  public class BlinkM {

    EZB _ezb;

    protected internal BlinkM(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Stop Script with 7 bit address
    /// </summary>
    public void StopScript(byte address7Bit) {

      _ezb.I2C.Write(address7Bit, new byte[] { (byte)'o' });
    }

    /// <summary>
    /// Change the BlinkM to the specified Red/Green/Blue color
    /// </summary>
    public void ChangeToColor(byte address7Bit, byte red, byte green, byte blue) {

      List<byte> tmpArray = new List<byte>();

      tmpArray.Add((byte)'n');
      tmpArray.Add(red);
      tmpArray.Add(green);
      tmpArray.Add(blue);

      _ezb.I2C.Write(address7Bit, tmpArray.ToArray());
    }

    /// <summary>
    /// Fade the BlinkM to the specified Red/Green/Blue color
    /// </summary>
    public void FadeToColor(byte address7Bit, byte red, byte green, byte blue) {

      List<byte> tmpArray = new List<byte>();

      tmpArray.Add((byte)'c');
      tmpArray.Add(red);
      tmpArray.Add(green);
      tmpArray.Add(blue);

      _ezb.I2C.Write(address7Bit, tmpArray.ToArray());
    }

    /// <summary>
    /// Returns the current colors on the BlinkM
    /// </summary>
    public async Task<Classes.BlinkMColor> GetCurrentColor(byte address7Bit) {

      _ezb.I2C.Write(address7Bit, new byte[] { 0x67 });

      byte [] ret = await _ezb.I2C.Read(address7Bit, 3);

      Classes.BlinkMColor bmc = new EZ_B.Classes.BlinkMColor(ret[0], ret[1], ret[2]);

      return bmc;
    }
  }
}
