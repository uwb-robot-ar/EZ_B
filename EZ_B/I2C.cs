using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// http://www.kerrywong.com/2012/07/08/msp-exp430g2-i2c-master-examples/

namespace EZ_B {

  public class I2C {

    EZB _ezb;

    protected internal I2C(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Write binary to the specified 7 bit address.
    /// Example: WriteBinary(0x1D, 0, 0, 1, 1, 1, 0, 1, 0);
    /// </summary>
    public void WriteBinary(byte deviceAddress, byte b7, byte b6, byte b5, byte b4, byte b3, byte b2, byte b1, byte b0) {

      byte b = (byte)((b0) + (b1 * 2) + (b2 * 4) + (b3 * 8) + (b4 * 16) + (b5 * 32) + (b6 * 64) + (b7 * 128));

      Write(deviceAddress, new byte[] { b });
    }

    /// <summary>
    /// Write data to the specified device 7 bit address.
    /// Example: Write(0x1D, new byte [] { 127, 64 } );
    /// </summary>
    public async void Write(byte deviceAddress, params byte[] data) {

      if (data.Length == 0)
        throw new Exception("I2C Data for Write cannot be empty");

      if (data.Length > 255)
        throw new Exception("Can not send more than 255 bytes over I2C");

      byte writeAddress8Bit;

      if (deviceAddress >= 128)
        writeAddress8Bit = deviceAddress;
      else
        writeAddress8Bit = (byte)(deviceAddress << 1);

      List<byte> bl = new List<byte>();
      bl.Add(writeAddress8Bit);
      bl.Add((byte)data.Length);
      bl.AddRange(data);

      await _ezb.sendCommand(0, EZB.CommandEnum.CmdI2CWrite, bl.ToArray());
    }

    /// <summary>
    /// Read data from the specified i2c device address. 
    /// Example: byte [] ret = Read(Auto, 0x1D, 1);
    /// </summary>
    public async Task<byte[]> Read(byte deviceAddress, byte expectedBytesReturn) {

      byte readAddress8Bit;

      if (deviceAddress >= 128)
        readAddress8Bit = (byte)((deviceAddress) | 1);
      else
        readAddress8Bit = (byte)((deviceAddress << 1) | 1);

      return await _ezb.sendCommand(expectedBytesReturn, EZB.CommandEnum.CmdI2CRead, new byte[] { readAddress8Bit, expectedBytesReturn });
    }

    /// <summary>
    /// Set the clock speed of the i2c interface
    /// </summary>
    public void SetClockSpeed(UInt32 rate) {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _ezb.Log(false, "Setting i2c rate: {0}", rate);

      List<Byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv4Enum.CmdV4I2CClockSpeed);
      b.AddRange(BitConverter.GetBytes(rate));

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, b.ToArray());
    }
  }
}
