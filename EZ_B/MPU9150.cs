using System;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_B {

  public class MPU9150 {

    EZB _ezb;

    public byte DeviceAddress = 0x68;

    public MPU9150(EZB ezb) {

      _ezb = ezb;
    }

    public void Init() {

      // Power Management boots chip into action
      _ezb.I2C.Write(DeviceAddress, 0x6B, 0x01);

      // Set SMPLRT_DIV to 0x04; this gives a 200 Hz sample rate when using the DLPF
      _ezb.I2C.Write(DeviceAddress, 0x19, 0x04);

      // 42 hz LPF
      _ezb.I2C.Write(DeviceAddress, 0x1A, 0x03);

      // full gyro mode
      _ezb.I2C.Write(DeviceAddress, 0x1B, 0x18);

      // 4g accel
      _ezb.I2C.Write(DeviceAddress, 0x1C, 0x08);

      // Interrupts pin stays high until cleared, cleared on any read, I2C bypass
      _ezb.I2C.Write(DeviceAddress, 0x37, 0x32);

      // enable master i2c mode
      _ezb.I2C.Write(DeviceAddress, 0x6A, 0x20);

      // slave 0 & 1: set delay rate for slave 0 & 1 (changing this to 0x02 makes values go wonky)
      _ezb.I2C.Write(DeviceAddress, 0x67, 0x03);

      // configure the reduced access sample rate
      _ezb.I2C.Write(DeviceAddress, 0x34, 0x1F);

      //// slave 0 & 1: set start/stop rather than restart (this was ES_WAIT but that didn't matter)
      //_ezb.I2C.Write(_ADDRESS, 0x24, 0x10);

      // slave 0: Set i2c address at slave0 at 0x0C + write bit
      _ezb.I2C.Write(DeviceAddress, 0x25, 0x8C);

      // slave 0: Set where reading at slave0 starts in the AK8975 registers
      _ezb.I2C.Write(DeviceAddress, 0x26, 0x03);

      // slave 0: enable slv0 data and specify 8 bytes to transfer
      _ezb.I2C.Write(DeviceAddress, 0x27, 0x88);

      // slave 1: set i2c address at slv1 at 0x0C
      _ezb.I2C.Write(DeviceAddress, 0x28, 0x0C);

      // slave 1: Set where writing starts
      _ezb.I2C.Write(DeviceAddress, 0x29, 0x0A);

      // slave 1: Enable writing and set read length to 1 (slave 1)
      _ezb.I2C.Write(DeviceAddress, 0x2A, 0x81);

      // data ready interupt enabled
      _ezb.I2C.Write(DeviceAddress, 0x38, 0x01);

      // slave 1: write data to compass repeated "single burst mode"
      _ezb.I2C.Write(DeviceAddress, 0x64, 0x01);
    }

    public async Task<Classes.MPU9150Cls> GetData() {

      _ezb.I2C.Write(DeviceAddress, 0x3B);

      byte[] tmp = await _ezb.I2C.Read(DeviceAddress, 20);
      byte[] everything = tmp.Reverse().ToArray();

      Classes.MPU9150Cls cls = new Classes.MPU9150Cls();

      cls.AccelX = BitConverter.ToInt16(everything, 18);
      cls.AccelY = BitConverter.ToInt16(everything, 16);
      cls.AccelZ = BitConverter.ToInt16(everything, 14);
      cls.TmpC = Convert.ToInt16((BitConverter.ToUInt16(everything, 12) / 340) + 36.53) / 10;
      cls.GyroY = BitConverter.ToInt16(everything, 10);
      cls.GyroX = BitConverter.ToInt16(everything, 8);
      cls.GyroZ = BitConverter.ToInt16(everything, 6);
      cls.CompassX = BitConverter.ToInt16(everything, 4);
      cls.CompassY = BitConverter.ToInt16(everything, 2);
      cls.CompassZ = BitConverter.ToInt16(everything, 0);

      //StringBuilder v = new StringBuilder();
      //foreach (byte b in everything) 
      //  v.AppendFormat("{0} ", (int)b);

      //_ezb.Log(false, v.ToString());

      return cls;
    }
  }
}
