using System;
using System.Threading.Tasks;

namespace EZ_B {

  public class Digital {

    EZB _ezb;

    /// <summary>
    /// List of Digital Ports
    /// </summary>
    [FlagsAttribute]
    public enum DigitalPortEnum {
      D0 = 0,
      D1 = 1,
      D2 = 2,
      D3 = 3,
      D4 = 4,
      D5 = 5,
      D6 = 6,
      D7 = 7,
      D8 = 8,
      D9 = 9,
      D10 = 10,
      D11 = 11,
      D12 = 12,
      D13 = 13,
      D14 = 14,
      D15 = 15,
      D16 = 16,
      D17 = 17,
      D18 = 18,
      D19 = 19,
      D20 = 20,
      D21 = 21,
      D22 = 22,
      D23 = 23
    }

    private volatile bool[] _lastValue = new bool[35];

    protected internal Digital(EZB ezb) {

      _ezb = ezb;
    }

    internal void Init() {

      for (int x = 0; x < _lastValue.Length; x++)
        _lastValue[x] = false;
    }

    /// <summary>
    ///  Set the status of a digital port. TRUE will output +5, FALSE will short to GND
    /// </summary>
    /// <returns>True if successful</returns>
    public void SetDigitalPort(DigitalPortEnum digitalPort, bool status) {

      int index = (int)digitalPort;

      _lastValue[index] = status;

      if (status)
        _ezb.sendCommand(EZB.CommandEnum.CmdSetDigitalPortOn + (byte)digitalPort);
      else
        _ezb.sendCommand(EZB.CommandEnum.CmdSetDigitalPortOff + (byte)digitalPort);
    }

    /// <summary>
    /// Does not query the EZ-B Controller. This returns the status of the port after you had SetDigitalPort().
    /// </summary>
    public bool GetLastDigitalPortSet(DigitalPortEnum digitalPort) {

      int index = (int)digitalPort;

      return _lastValue[index];
    }

    /// <summary>
    /// Toggles the status of a digital port and returns the new status
    /// </summary>
    public bool Toggle(DigitalPortEnum digitalPort) {

      int index = (int)digitalPort;

      SetDigitalPort(digitalPort, !_lastValue[index]);

      return _lastValue[index];
    }

    /// <summary>
    /// Query the status of a digital port.
    /// </summary>
    public async Task<bool> GetDigitalPort(DigitalPortEnum digitalPort) {

      int index = (int)digitalPort;

      bool retVal = ((await _ezb.sendCommand(1, EZB.CommandEnum.CmdGetDigitalPort + (byte)digitalPort))[0] == 1);

      _lastValue[index] = retVal;

      return retVal;
    }
    /// <summary>
    /// Query the status of a digital port as an Integer (0 false, 1 true)
    /// </summary>
    public async Task<int> GetDigitalPortAsInt(DigitalPortEnum digitalPort) {

      return (await GetDigitalPort(digitalPort) ? 1 : 0);
    }
  }
}
