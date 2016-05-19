using System;
using System.Collections.Generic;

namespace EZ_B {

  public class ServoAX12 {
    
    EZB _ezb;

    protected internal ServoAX12(EZB ezb) {

      _ezb = ezb;
    }

    public static byte[] MoveServo(byte id, int position) {

      if (position > 1023)
        position = 1023;

      if (position < 0)
        position = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data

      buffer.Add(30); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)position));

      return GetCommand(id, buffer.ToArray());
    }

    public static byte[] ServoSpeed(byte id, int speed) {

      if (speed > 1023)
        speed = 1023;

      if (speed < 0)
        speed = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data

      buffer.Add(34); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)speed));

      return GetCommand(id, buffer.ToArray());
    }

    internal static byte[] GetCommand(byte id, byte[] buffer) {

      if (buffer.Length > 255)
        throw new Exception("AX-12 Command cannot be longer than 255 bytes");

      List<byte> bList      = new List<byte>();
      byte       dataLength = (byte)(buffer.Length + 1);

      bList.Add(0xff);
      bList.Add(0xff);
      bList.Add(id);
      bList.Add(dataLength);

      byte checksum = (byte)(id + dataLength);

      for (int x = 0; x < buffer.Length; x++) {

        checksum += buffer[x];

        bList.Add(buffer[x]);
      }

      checksum = (byte)(0xff - (checksum % 256));

      bList.Add(checksum);

      return bList.ToArray();
    }

    public void SendCommand(byte[] data) {

      List<byte> cmd = new List<byte>();

      cmd.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);
      cmd.Add((byte)data.Length);
      cmd.AddRange(data);

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, cmd.ToArray());
    }

    public void LED(byte id, bool status) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data
      buffer.Add(25); // LED
      buffer.Add(status ? (byte)1 : (byte)0);

      SendCommand(GetCommand(id, buffer.ToArray()));     
    }
  }
}
