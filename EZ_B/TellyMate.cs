using System;

namespace EZ_B {

  public class TellyMate {

    EZB _ezb;

    public Digital.DigitalPortEnum TellyMatePort = Digital.DigitalPortEnum.D1;
    public Uart.BAUD_RATE_ENUM TellyMateBaudRate = Uart.BAUD_RATE_ENUM.Baud_57600;

    /// <summary>
    /// List of TellyMate Commands
    /// </summary>
    public enum CmdEnum {
      Cursor_Up = 65,
      Cursor_Down = 66,
      Cursor_Right = 67,
      Cursor_Left = 68,
      Clear_Screen = 69,
      Cursor_Home = 72,
      Reverse_Line_Feed = 73,
      Clear_To_End_Of_Screen = 74,
      Clear_To_End_Of_Line = 75,
      Print_Diagnostic_Information = 81,
      Clear_Line = 108,
      Clear_To_Start_Of_Line = 111,
      Line_Overflow_Enable = 118,
      Line_Overflow_Disable = 119,
      Reset_Tellymate = 122
    }

    /// <summary>
    /// List of TellyMate Font Attributes
    /// </summary>
    public enum FontAttribEnum {
      SingleWidth_SingleHeight = 48,
      DoubleWidth_SingleHeight = 49,
      SingleWidth_DoubleHeight_Top = 50,
      SingleWidth_DoubleHeight_Bottom = 51,
      DoubleWidth_DoubleHeight_Top = 52,
      DoubleWidth_DoubleHeight_Bott = 53,
      FontBank_0 = 54,
      FontBank_1 = 55,
      FontBank_2 = 56,
      FontBank_3 = 57,
      FontBank_4 = 58,
      FontBank_5 = 59,
      FontBank_6 = 60,
      FontBank_7 = 61,
      FontBank_8 = 62,
      FontBank_9 = 63,
      FontBank_10 = 64
    }

    protected internal TellyMate(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Send the text to a Tellymate on port D0 with optional carriage return
    /// </summary>
    public void SendText(string text) {

      _ezb.Uart.SendSerial(TellyMatePort, TellyMateBaudRate, text);
    }

    /// <summary>
    /// Send the text to a Tellymate on port D0 with optional carriage return
    /// </summary>
    public void SendText(string text, bool carriageReturn) {

      if (carriageReturn)
        text += Environment.NewLine;

      _ezb.Uart.SendSerial(TellyMatePort, TellyMateBaudRate, text);
    }

    /// <summary>
    /// Sent a command to the TellyMate
    /// </summary>
    public void SendCommand(CmdEnum tellyMateCmd) {

      _ezb.Uart.SendSerial(TellyMatePort, TellyMateBaudRate, new byte[] { 27, (byte)tellyMateCmd });
    }

    /// <summary>
    /// Move the cursor to specified position
    /// </summary>
    public void MoveCursor(int col, int row) {

      col = col + 32;
      row = row + 32;

      _ezb.Uart.SendSerial(TellyMatePort, TellyMateBaudRate, new byte[] { 27, (byte)'Y', (byte)row, (byte)col });
    }

    /// <summary>
    /// Set the font attribute
    /// </summary>
    public void SetFontAttrib(FontAttribEnum fontAttrib) {

      _ezb.Uart.SendSerial(TellyMatePort, TellyMateBaudRate, new byte[] { 27, (byte)'_', (byte)fontAttrib });
    }
  }
}
