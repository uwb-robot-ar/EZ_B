using System;
using System.Text;

namespace EZ_B.Classes {

  public class RecordingEvent {

    public RecordingEvent() {
    }

    public RecordingEvent(byte[] cmdData, int ms) {

      CmdData = cmdData;
      MS = ms;
    }

    public string GetCmdArgs {

      get {

        StringBuilder sb = new StringBuilder();

        foreach (byte b in CmdData)
          sb.AppendFormat("{0} ", b);

        return sb.ToString();
      }
    }

    public byte []              CmdData = new byte[] { };
    public int                  MS      = 0;
  }
}
