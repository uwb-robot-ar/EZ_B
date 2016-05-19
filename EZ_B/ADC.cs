using System;
using System.Threading.Tasks;

namespace EZ_B {

  public class ADC {

    EZB _ezb;

    /// <summary>
    /// List of ADC Ports
    /// </summary>
    [FlagsAttribute]
    public enum ADCPortEnum {
      ADC0 = 0,
      ADC1 = 1,
      ADC2 = 2,
      ADC3 = 3,
      ADC4 = 4,
      ADC5 = 5,
      ADC6 = 6,
      ADC7 = 7
    }

    protected internal ADC(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    ///  Get an integer from 0-255 (8 bits) representing the relative voltage of a specified ADC port (Between 0 and 5 volts)
    /// </summary>
    public async Task<int> GetADCValue(ADCPortEnum sendSensor) {

      int index = (int)sendSensor;

      int retVal;

      if (_ezb.EZBType == EZB.EZ_B_Type_Enum.ezb4)
        retVal = (UInt16)BitConverter.ToUInt16(await _ezb.sendCommand(2, EZB.CommandEnum.CmdGetADCValue + (byte)sendSensor), 0) / 16;
      else
        retVal = (await _ezb.sendCommand(1, EZB.CommandEnum.CmdGetADCValue + (byte)sendSensor))[0];

      return retVal;
    }

    /// <summary>
    ///  Get an integer from 0-4096 (12 bits) representing the relative voltage of a specified ADC port (Between 0 and 5 volts)
    /// </summary>
    public async Task<int> GetADCValue12Bit(ADCPortEnum sendSensor) {

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This command is only available on the EZ-B v4");

      int index = (int)sendSensor;

      return (UInt16)BitConverter.ToUInt16(await _ezb.sendCommand(2, EZB.CommandEnum.CmdGetADCValue + (byte)sendSensor), 0);
    }

    /// <summary>
    /// Returns the voltage relative to the inputted value. If you want to display the Value and Voltage, you can pass the value to this function rather then executing a new command. This saves bandwidth over the line.
    /// </summary>
    public float GetADCVoltageFromValue(int adcValue) {

      float value = (float)adcValue;

      float divider = 5f / 255f;

      return divider * value;
    }

    /// <summary>
    ///  Get the voltage from 0-5v of a specified ADC port
    /// </summary>
    public async Task<float> GetADCVoltage(ADCPortEnum sendSensor) {

      return GetADCVoltageFromValue(await GetADCValue(sendSensor));
    }
  }
}
