using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZ_B {

  public class EZBv4Manager {

    EZ_B.EZB _ezb;

    internal EZBv4Manager(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Disable or Enable the battery monitor for the EZ-B v4. If the battery monitor is disabled, the EZ-B will continue to operate I/O if the voltage is low.
    /// You can also adjust the lowest voltage value to one decimal place.
    /// </summary>
    public void SetLipoBatteryProtection(bool value, decimal lowestVoltage) {

      SetLipoBatteryLowestVoltage(lowestVoltage);

      SetLipoBatteryProtectionState(value);
    }

    /// <summary>
    /// Sets the lowest voltage that the EZ-B will operate with for the battery monitor. This is useful to Lipo batteries. 
    /// This feature is enabled by default on the EZ-B v4.
    /// </summary>
    /// <param name="lowestVoltage"></param>
    public void SetLipoBatteryLowestVoltage(decimal lowestVoltage) {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _ezb.Log(false, "Setting battery monitor voltage: {0}", lowestVoltage);

      UInt16 voltage = (UInt16)(lowestVoltage / 0.003862434m);

      List<Byte> b = new List<byte>();

      b.Add((byte)EZB.CmdEZBv4Enum.CmdV4SetBatteryMonitorVoltage);
      b.AddRange(BitConverter.GetBytes(voltage));

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, b.ToArray());
    }

    /// <summary>
    /// Disable or Enable the battery monitor for the EZ-B v4. If the battery monitor is disabled, the EZ-B will continue to operate I/O if the voltage is low.
    /// </summary>
    public void SetLipoBatteryProtectionState(bool value) {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return;

      _ezb.Log(false, "Setting battery protection: {0}", value);

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4,
        (byte)EZB.CmdEZBv4Enum.CmdV4SetLipoBatteryProtectionState,
        value ? (byte)1 : (byte)0
      );
    }

    /// <summary>
    /// Returns the cpu core temperature in degrees celcuis
    /// </summary>
    /// <returns></returns>
    public async Task<decimal> GetCPUTemperature() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return 0;

      byte[] tmpArray = await _ezb.sendCommand(2, EZB.CommandEnum.CmdEZBv4, (byte)EZB.CmdEZBv4Enum.CmdV4GetCPUTemp);

      decimal volts = (decimal)BitConverter.ToUInt16(tmpArray, 0) * 0.000800781m;

      return volts * 32.89473684m;
    }

    /// <summary>
    /// Returns the battery voltage
    /// </summary>
    /// <returns></returns>
    public async Task<decimal> GetBatteryVoltage() {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        return 0;

      byte[] tmpArray = await _ezb.sendCommand(2, EZB.CommandEnum.CmdEZBv4, (byte)EZB.CmdEZBv4Enum.CmdV4GetBatteryVoltage);

      return (decimal)BitConverter.ToUInt16(tmpArray, 0) * 0.003862434m;
    }
  }
}
