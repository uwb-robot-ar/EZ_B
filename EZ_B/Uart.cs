using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZ_B {

  public class Uart {

    public enum BAUD_RATE_ENUM {
      Baud_4800 = 0,
      Baud_9600 = 1,
      Baud_19200 = 2,
      Baud_38400 = 3,
      Baud_57600 = 4,
      Baud_115200 = 5,
      Baud_Custom = 6
    }

    EZB _ezb;

    protected internal Uart(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Specify the clock delay between bytes in cycles of the EZ-B's 120mhz 32 Bit ARM processor. This would only need to be used to fine tune the baudrate timing if the connected device is not very accurate or requires a diffference in timing.
    /// For example, some open-source hardware platforms use Software Serial drivers, which sometimes need a little bit of tweaking. Generally, you should never need to change these values.
    /// However, there is a Custom labelled baudrate which you can change for specific speeds. 
    /// Anyone adjusting these speeds will need a logic analyzer, such as the Saleae Logic16 or Logic32
    /// </summary>
    public void SetBaudClock(BAUD_RATE_ENUM baudRate, int clockSpeed) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This feature is only available for EZ-B v4");

      List<byte> send = new List<byte>();

      send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTClockSpeed);
      send.Add((byte)baudRate);
      send.AddRange(BitConverter.GetBytes((UInt16)clockSpeed));

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, send.ToArray());
    }

    /// <summary>
    /// Send text over serial specified serial port at baud rate
    /// </summary>
    public void SendSerial(Digital.DigitalPortEnum digitalPort, BAUD_RATE_ENUM baudRate, string text) {

      SendSerial(digitalPort, baudRate, text.Select(Convert.ToByte).ToArray());
    }

    /// <summary>
    /// Send text over serial specified serial port at baud rate
    /// </summary>
    public void SendSerial(Digital.DigitalPortEnum digitalPort, BAUD_RATE_ENUM baudRate, char[] charArray) {

      SendSerial(digitalPort, baudRate, Encoding.UTF8.GetBytes(charArray));
    }

    /// <summary>
    /// Send text over serial specified serial port at baud rate
    /// </summary>
    public void SendSerial(Digital.DigitalPortEnum digitalPort, BAUD_RATE_ENUM baudRate, byte[] byteArray) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      foreach (byte [] bArray in Functions.Chunk(byteArray, 255)) {

        List<byte> send = new List<byte>();

        send.Add((byte)baudRate);
        send.Add((byte)bArray.Length);
        send.AddRange(bArray);

        _ezb.sendCommand(EZB.CommandEnum.CmdSendSerial + (byte)digitalPort, send.ToArray());
      }
    }

    public void UARTExpansionInit(int port, UInt32 baudRate) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This feature is only available for EZ-B v4");

      List <byte> send = new List<byte>();

      if (port == 0)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion0Init);
      else if (port == 1)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion1Init);
      else if (port == 2)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion2Init);
      else
        throw new Exception("UART port is a 0, 1 or 2");

      send.AddRange(BitConverter.GetBytes(baudRate));

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, send.ToArray());
    }

    public void UARTExpansionWrite(int port, byte[] data) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This feature is only available for EZ-B v4");

      List <byte> send = new List<byte>();

      if (port == 0)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion0Write);
      else if (port == 1)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion1Write);
      else if (port == 2)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion2Write);
      else
        throw new Exception("UART port is a 0, 1 or 2");

      send.AddRange(BitConverter.GetBytes((UInt16)data.Length));
      send.AddRange(data);

      _ezb.sendCommand(EZB.CommandEnum.CmdEZBv4, send.ToArray());
    }

    public async Task<int> UARTExpansionAvailableBytes(int port) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This feature is only available for EZ-B v4");

      byte [] ret;

      if (port == 0)
        ret = await _ezb.sendCommand(2, EZB.CommandEnum.CmdEZBv4, (byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion0AvailableBytes);
      else if (port == 1)
        ret = await _ezb.sendCommand(2, EZB.CommandEnum.CmdEZBv4, (byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion1AvailableBytes);
      else if (port == 2)
        ret = await _ezb.sendCommand(2, EZB.CommandEnum.CmdEZBv4, (byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion2AvailableBytes);
      else
        throw new Exception("UART port is a 0, 1 or 2");

      return BitConverter.ToUInt16(ret, 0);
    }

    public async Task<byte[]> UARTExpansionRead(int port, int bytesToRead) {

      if (!_ezb.IsConnected)
        throw new Exception("Not connected");

      if (_ezb.EZBType != EZB.EZ_B_Type_Enum.ezb4)
        throw new Exception("This feature is only available for EZ-B v4");

      List <byte> send = new List<byte>();

      if (port == 0)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion0Read);
      else if (port == 1)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion1Read);
      else if (port == 2)
        send.Add((byte)EZB.CmdEZBv4Enum.CmdV4UARTExpansion2Read);
      else
        throw new Exception("UART port is a 0, 1 or 2");

      send.AddRange(BitConverter.GetBytes((UInt16)bytesToRead));

      return await _ezb.sendCommand(bytesToRead, EZB.CommandEnum.CmdEZBv4, send.ToArray());
    }

    public async Task<byte[]> UARTExpansionReadAvailable(int port) {

      int avail = await UARTExpansionAvailableBytes(port);

      if (avail == 0)
        return new byte[] { };

      return await UARTExpansionRead(port, avail);
    }
  }
}
