using System.Threading.Tasks;

namespace EZ_B {

  public class BV4615 {

    EZB _ezb;

    public byte Address7Bit = 0x12;

    protected internal BV4615(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Return the firmware of the device
    /// </summary>
    public async Task<string> GetFirmware() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x27, 0x1b, 0x5b, 0x3f, 0x33, 0x30, 0x62 });

      byte [] b = await _ezb.I2C.Read(Address7Bit, 2);

      return string.Format("{0}.{1}", b[0], b[1]);
    }

    /// <summary>
    /// Returns a response object with the data from the buffer
    /// </summary>
    public async Task<Classes.BV4615Response> GetData() {

      byte [] b = await _ezb.I2C.Read(Address7Bit, 2);

      bool valid = !Functions.IsBitSet(b[0], 7) && Functions.IsBitSet(b[0], 0);

      bool toggle = false;

      return new Classes.BV4615Response(toggle, valid, b[0], b[1]);
    }
  }
}
