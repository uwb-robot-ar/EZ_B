using System.Threading.Tasks;

namespace EZ_B {

  public class MMA7455 {

    EZB _ezb;

    public byte Address7Bit = 0x1D;

    public enum SensitivityEnum {

      Sensitivity_8g,
      Sensitivity_2g,
      Sensitivity_4g
    }

    protected internal MMA7455(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Return the firmware of the device
    /// </summary>
    public string WhoAmI() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x0f });

      return _ezb.I2C.Read(Address7Bit, 1).ToString();
    }

    /// <summary>
    /// Send initialization
    /// </summary>
    public void Init(SensitivityEnum sensitivity) {

      if (sensitivity == SensitivityEnum.Sensitivity_2g)
        _ezb.I2C.Write(Address7Bit, new byte[] { 0x16, Functions.ToByteFromBinary(0, 0, 0, 0, 0, 1, 0, 1) });
      else if (sensitivity == SensitivityEnum.Sensitivity_4g)
        _ezb.I2C.Write(Address7Bit, new byte[] { 0x16, Functions.ToByteFromBinary(0, 0, 0, 0, 1, 0, 0, 1) });
      else if (sensitivity == SensitivityEnum.Sensitivity_8g)
        _ezb.I2C.Write(Address7Bit, new byte[] { 0x16, Functions.ToByteFromBinary(0, 0, 0, 0, 0, 0, 0, 1) });
    }

    /// <summary>
    /// Return the current configuration
    /// </summary>
    public async Task<byte> GetMode() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x16 });

      return (await _ezb.I2C.Read(Address7Bit, 1))[0];
    }

    /// <summary>
    /// Get X
    /// </summary>
    public async Task<byte> GetX() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x06 });

      return (byte)((await _ezb.I2C.Read(Address7Bit, 1))[0] + 128);
    }

    /// <summary>
    /// Get Y
    /// </summary>
    public async Task<byte> GetY() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x07 });

      return (byte)((await _ezb.I2C.Read(Address7Bit, 1))[0] + 128);
    }

    /// <summary>
    /// Get Z
    /// </summary>
    public async Task<byte> GetZ() {

      _ezb.I2C.Write(Address7Bit, new byte[] { 0x08 });

      return (byte)((await _ezb.I2C.Read(Address7Bit, 1))[0] + 128);
    }
  }
}
