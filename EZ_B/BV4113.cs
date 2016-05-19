using System;
using System.Collections.Generic;

namespace EZ_B {

  public class BV4113 {

    EZB          _ezb;
    private byte _speed = Movement.MAX_SPEED;

    protected internal BV4113(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    ///  Stop
    /// </summary>
    public void Stop() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb3)
        throw new Exception("This feature is only compatible with EZ-B v3");

      List<byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv3Enum.CmdV3BV4113);
      b.Add((byte)4);

      _ezb.sendCommand(0, EZB.CommandEnum.CmdEZBv3, b.ToArray());
    }

    /// <summary>
    ///  Move forward.
    /// </summary>
    public void Forward() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb3)
        throw new Exception("This feature is only compatible with EZ-B v3");

      List<byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv3Enum.CmdV3BV4113);
      b.Add((byte)3);

      _ezb.sendCommand(0, EZB.CommandEnum.CmdEZBv3, b.ToArray());
    }

    /// <summary> 
    ///  Move reverse.
    /// </summary>
    public void Reverse() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb3)
        throw new Exception("This feature is only compatible with EZ-B v3");

      List<byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv3Enum.CmdV3BV4113);
      b.Add((byte)1);

      _ezb.sendCommand(0, EZB.CommandEnum.CmdEZBv3, b.ToArray());
    }

    /// <summary> 
    ///  Right.
    /// </summary>
    public void Right() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb3)
        throw new Exception("This feature is only compatible with EZ-B v3");

      List<byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv3Enum.CmdV3BV4113);
      b.Add((byte)2);

      _ezb.sendCommand(0, EZB.CommandEnum.CmdEZBv3, b.ToArray());
    }

    /// <summary> 
    ///  Left.
    /// </summary>
    public void Left() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb3)
        throw new Exception("This feature is only compatible with EZ-B v3");

      List<byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv3Enum.CmdV3BV4113);
      b.Add((byte)0);

      _ezb.sendCommand(0, EZB.CommandEnum.CmdEZBv3, b.ToArray());
    }
  }
}
