using System;
using System.Threading.Tasks;

namespace EZ_B {

  public class HC_SR04 {

    EZB _ezb;

    private volatile DateTime[] lastRequest = new DateTime[25];
    private volatile int[] lastValue = new int[25];

    public static readonly int MAX_VALUE = 255;

    /// <summary>
    /// To prevent ADC requests from flooding the communication channel, this limit prevents too many calls. Best to leave it alone.
    /// </summary>
    public int MinPoolTimeMS = 200;

    protected internal HC_SR04(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    ///  Get the value received from the HC-SR04 Ping Sensor
    /// </summary>
    public async Task<int> GetValue(Digital.DigitalPortEnum triggerPort, Digital.DigitalPortEnum echoPort) {

      int index = (int)triggerPort;

      if (lastRequest[index].AddMilliseconds(MinPoolTimeMS) > DateTime.Now)
        return lastValue[index];

      byte retVal = (await _ezb.sendCommand(1, EZB.CommandEnum.CmdHC_SR04 + (byte)triggerPort, (byte)echoPort))[0];

      lastValue[index] = retVal;
      lastRequest[index] = DateTime.Now;

      return retVal;
    }
  }
}
