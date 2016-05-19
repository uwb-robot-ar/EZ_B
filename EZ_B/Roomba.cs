using System.Collections.Generic;

namespace EZ_B {

  // http://hackingroomba.com/about/sample/

  public class RoombaSong {

    public enum NoteEnum {
      C1,
      Db1,
      D1,
      Eb1,
      E1,
      F1,
      Gb1,
      G1,
      Ab1,
      A1,
      Bb1,
      B1,
      C2,
      Db2,
      D2,
      Eb2,
      E2,
      F2,
      Gb2,
      G2,
      Ab2,
      A2,
      Bb2,
      B12
    }

    public RoombaSong(NoteEnum note, byte noteLength) {

      Note = note;
      NoteLength = noteLength;
    }

    public NoteEnum Note = NoteEnum.C1;
    public byte NoteLength = 20;
  }

  public class Roomba {

    EZB _ezb;
    bool _mainBrush = false;
    bool _vacuumBrush = false;
    bool _sideBrush = false;

    /// <summary>
    /// The communication port for the Roomba
    /// </summary>
    public readonly Digital.DigitalPortEnum CommunicationPort = Digital.DigitalPortEnum.D0;

    /// <summary>
    /// Some of the older roombas conflict with the SCI datasheet by iRobot Roomba.
    /// If your roomba is moving the wrong direction, set this.
    /// </summary>
    public bool UseOldProtocol = false;

    /// <summary>
    /// The baud rate for your roomba.
    /// Mostly 57600, except newer models are 115200
    /// </summary>
    public Uart.BAUD_RATE_ENUM RoombaBaudRate = Uart.BAUD_RATE_ENUM.Baud_57600;

    protected internal Roomba(EZB ezb) {

      _ezb = ezb;
    }

    public void SendInit() {

      _mainBrush = false;
      _vacuumBrush = false;
      _sideBrush = false;

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 128, 130 });
    }

    public void EnableSensors() {

      _mainBrush = false;
      _vacuumBrush = false;
      _sideBrush = false;

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 128, 130 });
    }

    public void DisableSensors() {

      _mainBrush = false;
      _vacuumBrush = false;
      _sideBrush = false;

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 128, 130, 131, 132 });
    }

    /// <summary>
    /// Power Off the Roomba
    /// </summary>
    public void PowerOff() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 133 });
    }

    /// <summary>
    /// Enable Spot Clean
    /// </summary>
    public void SpotClean() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 134 });
    }

    /// <summary>
    /// Enable Clean
    /// </summary>
    public void Clean() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 135 });
    }

    /// <summary>
    /// Turn off all brushes (motors)
    /// </summary>
    public void DisableAllBrushes() {

      _mainBrush = false;
      _sideBrush = false;
      _vacuumBrush = false;

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 138, Functions.ToByteFromBinary(0, 0, 0, 0, 0, 0, 0, 0) });
    }

    /// <summary>
    /// Control the motors
    /// </summary>
    public void SetMotorStates(bool mainBrush, bool sideBrush, bool vacuum) {

      _mainBrush = mainBrush;
      _sideBrush = sideBrush;
      _vacuumBrush = vacuum;

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 138, Functions.ToByteFromBinary(false, false, false, false, false, _mainBrush, vacuum, sideBrush) });
    }

    /// <summary>
    /// Set the state of the main brush motor
    /// </summary>
    public void SetMainBrush(bool state) {

      SetMotorStates(state, _sideBrush, _vacuumBrush);
    }

    /// <summary>
    /// Set the state of the side brush motor
    /// </summary>
    public void SetSideBrush(bool state) {

      SetMotorStates(_mainBrush, state, _vacuumBrush);
    }

    /// <summary>
    /// Set the state of the vacuum motor
    /// </summary>
    public void SetVacuum(bool state) {

      SetMotorStates(_mainBrush, _sideBrush, state);
    }

    /// <summary>
    /// Force seek docking station. Must be cleaning before you can seek dock station
    /// </summary>
    public void SeekDockingStation() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 143 });
    }

    /// <summary>
    /// Change PowerLED color and intensity
    /// The Color value is between Green and Red (1 and 255)
    /// </summary>
    public void PowerLED(byte intensity, byte color) {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 139, 63, color, intensity });
    }

    /// <summary>
    /// Stop moving
    /// </summary>
    public void Stop() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 137, 0x0, 0x0, 0x00, 0x0 });
    }

    /// <summary>
    /// Move Roomba (velocity between -200 and 200) (angle between -2000 and 2000 or -1 and 1) (straight: 32768)
    /// </summary>
    public void Drive(short velocity, short angle) {

      List<byte> send = new List<byte>();

      send.Add(137);

      velocity *= 2;

      if (velocity < -500)
        velocity = -500;

      if (velocity > 500)
        velocity = 500;

      byte bySpeedHi = (byte)(velocity >> 8);
      byte bySpeedLo = (byte)(velocity & 255);

      byte byAngleHi = (byte)((short)angle >> 8);
      byte byAngleLo = (byte)((short)angle & 255);

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 137, bySpeedHi, bySpeedLo, byAngleHi, byAngleLo });
    }

    /// <summary>
    /// Move Roomba forward. Speed is between 0 and 200
    /// </summary>
    public void Forward(byte speed) {

      if (UseOldProtocol)
        Drive(speed, -2000);
      else
        Drive(speed, 0);
    }

    /// <summary>
    /// Move Roomba Reverse. Speed is between 0 and 200
    /// </summary>
    /// <param name="speed"></param>
    public void Reverse(byte speed) {

      if (UseOldProtocol)
        Drive((short)(0 - speed), 2000);
      else
        Drive((short)(0 - speed), 0);
    }

    /// <summary>
    /// Turn Roomba right
    /// </summary>
    public void Right(byte speed) {

      if (UseOldProtocol)
        Drive((short)(0 - speed), 0);
      else
        Drive(speed, -1);
    }

    /// <summary>
    /// Turn Roomba left
    /// </summary>
    public void Left(byte speed) {

      if (UseOldProtocol)
        Drive(speed, 0);
      else
        Drive(speed, 1);
    }

    /// <summary>
    /// Enable Max Clean
    /// </summary>
    public void Max() {

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, new byte[] { 136 });
    }

    /// <summary>
    /// Play one note using the Roomba's speaker
    /// </summary>
    public void PlayTone(RoombaSong.NoteEnum note, byte length) {

      PlaySong(new RoombaSong(note, length));
    }

    /// <summary>
    /// Play a song using the Roomba's speaker.
    /// Roomba supports a maximum of 15 notes.
    /// </summary>
    public void PlaySong(params RoombaSong[] song) {

      int songLength = song.Length;

      if (songLength > 15)
        songLength = 15;

      List<byte> byteList = new List<byte>();

      byteList.Add(140);
      byteList.Add(1);
      byteList.Add((byte)songLength);

      int cnt = 0;
      foreach (RoombaSong rombaSong in song) {

        byteList.Add((byte)(60 + (byte)rombaSong.Note));
        byteList.Add(rombaSong.NoteLength);

        cnt++;

        if (cnt > 15)
          break;
      }

      byteList.Add(141);
      byteList.Add(1);

      _ezb.Uart.SendSerial(CommunicationPort, RoombaBaudRate, byteList.ToArray());
    }
  }
}
