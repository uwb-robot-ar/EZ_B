using System;
using System.Collections.Generic;

namespace EZ_B {

  public class Dynamixel {

    EZB _ezb;

    protected internal Dynamixel(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Return the data packet that will release a servo with the specified ID
    /// </summary>
    public static byte[] ReleaseServo(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data

      buffer.Add(24); // Torque Enable

      buffer.Add(0x00);

      return CreateDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Returns the data packet that will  move a servo with the specified id to the position
    /// </summary>
    public static byte[] MoveServoCmd(byte id, int position) {

      if (position > 1023)
        position = 1023;

      if (position < 0)
        position = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data

      buffer.Add(30); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)position));

      return CreateDynamixelCommand(id, buffer.ToArray());
    }

    /// <summary>
    /// Return a packet that will set the speed of the servo with the id to the speed
    /// </summary>
    public static byte[] ServoSpeed(byte id, int speed) {

      if (speed > 1023)
        speed = 1023;

      if (speed < 0)
        speed = 0;

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data

      buffer.Add(34); // Goal Position

      buffer.AddRange(BitConverter.GetBytes((UInt16)speed));

      return CreateDynamixelCommand(id, buffer.ToArray());
    }

    internal static byte[] CreateDynamixelCommand(byte id, byte[] buffer) {

      if (buffer.Length > 255)
        throw new Exception("AX-12 Command cannot be longer than 255 bytes");

      List<byte> bList      = new List<byte>();
      byte       dataLength = (byte)(buffer.Length + 1);

      bList.Add(0xff);
      bList.Add(0xff);
      bList.Add(id);
      bList.Add(dataLength);

      int checksum = id;

      checksum += dataLength;

      for (int x = 0; x < buffer.Length; x++) {

        checksum += buffer[x];

        bList.Add(buffer[x]);
      }

      bList.Add((byte)(0xff - (checksum % 256)));

      return bList.ToArray();
    }

    /// <summary>
    /// Send the dynamixel data packet to the ez-b b4
    /// </summary>
    public void SendCommandToEZB(byte[] data) {

      List<byte> cmd = new List<byte>();

      cmd.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetAX12Servo);
      cmd.Add((byte)data.Length);
      cmd.AddRange(data);

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, cmd.ToArray());
    }

    /// <summary>
    /// Change the LED status of the dynamixel servo
    /// </summary>
    public void LED(Servo.ServoPortEnum servo, bool status) {

      if (servo < Servo.ServoPortEnum.AX0 || servo > Servo.ServoPortEnum.AX50)
        throw new Exception("Servo out of range for Dynamixel");

      byte id = (byte)(servo - Servo.ServoPortEnum.AX0);

      LED(id, status);
    }

    /// <summary>
    /// Change the LED status of the specified servo
    /// </summary>
    public void LED(byte id, bool status) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data
      buffer.Add(25); // LED
      buffer.Add(status ? (byte)1 : (byte)0);

      SendCommandToEZB(CreateDynamixelCommand(id, buffer.ToArray()));
    }

    public void DisableAllAlarms(Servo.ServoPortEnum servo) {

      byte id = (byte)(servo - Servo.ServoPortEnum.AX0);

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data
      buffer.Add(0x12);
      buffer.Add(254);

      SendCommandToEZB(CreateDynamixelCommand(id, buffer.ToArray()));
    }

    public static byte [] GetDisableStatusPacketCmd(byte id) {

      List<byte> buffer = new List<byte>();

      buffer.Add(3);  // Write data
      buffer.Add(0x10);
      buffer.Add(0);

      return CreateDynamixelCommand(id, buffer.ToArray());        
    }

    public void DisableStatusPacket(Servo.ServoPortEnum servo) {

      byte id = (byte)(servo - Servo.ServoPortEnum.AX0);

      SendCommandToEZB(GetDisableStatusPacketCmd(id));
    }

    public void DisableStatusPacket() {

      for (byte x = 0; x < 50; x++)
        SendCommandToEZB(GetDisableStatusPacketCmd(x));
    }

    public void ChangeID(Servo.ServoPortEnum source, Servo.ServoPortEnum destination) {

      byte       souceID       = (byte)(source - Servo.ServoPortEnum.AX0);
      byte       destinationID = (byte)(destination - Servo.ServoPortEnum.AX0);
      List<byte> buffer        = new List<byte>();

      buffer.Add(3);  // Write data
      buffer.Add(3);  // Change ID
      buffer.Add(destinationID);

      SendCommandToEZB(CreateDynamixelCommand(souceID, buffer.ToArray()));
    }
  }
}
