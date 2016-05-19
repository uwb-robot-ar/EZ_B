using System;

namespace EZ_B {

  public class SureDualAxisCompass {

    EZB _ezb;

    public byte Address7Bit = 0x30;
    public Classes.CompassDirection CompassData = new Classes.CompassDirection(false);

    private object _lock = new object();
    private DateTime _lastUpdateTime = DateTime.Now;

    /// <summary>
    /// To prevent requests from flooding the communication channel, this limit prevents too many calls. Best to leave it alone.
    /// </summary>
    public int MinPoolTimeMS = 25;

    protected internal SureDualAxisCompass(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Init the coil. Should be called as init one time
    /// </summary>
    public void SetCoil() {

      if (!_ezb.IsConnected)
        return;

      lock (_lock) {

        _ezb.I2C.Write(Address7Bit, new byte[] { 0x00, 0x02 });
      }
    }

    /// <summary>
    /// Set the offset of the compass degrees for custom alignment
    /// </summary>
    public int Offset {
      get {
        return CompassData.Offset;
      }
      set {
        CompassData.Offset = value;
      }
    }

    /// <summary>
    /// Reset the Compass Coil
    /// </summary>
    public void ResetCoil() {

      if (!_ezb.IsConnected)
        return;

      lock (_lock) {

        _ezb.I2C.Write(Address7Bit, new byte[] { 0x00, 0x04 });
      }
    }

    /// <summary>
    /// Updates CompassData object with the current magnetic co-ordinates of the DC-SS503 Compass Module
    /// </summary>
    /// <returns></returns>
    public async void Update() {

      if (MinPoolTimeMS > 0 && _lastUpdateTime.AddMilliseconds(MinPoolTimeMS) > DateTime.Now)
        return;

      if (!_ezb.IsConnected) {

        CompassData.Connected = false;
      } else {

        _ezb.I2C.Write(Address7Bit, new byte[] { 0x00, 0x01 });

        byte[] bArray = await _ezb.I2C.Read(Address7Bit, 5);

        CompassData.X = BitConverter.ToInt16(new byte[] { bArray[2], bArray[1] }, 0);
        CompassData.Y = BitConverter.ToInt16(new byte[] { bArray[4], bArray[3] }, 0);
        CompassData.Connected = true;
      }
    }
  }
}
