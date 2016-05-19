using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_B {

  public class Recorder {

    EZB _ezb;
    bool _isRecording = false;
    List<Classes.RecordingEvent> _recordingEventList = new List<Classes.RecordingEvent>();
    DateTime _dateTime = DateTime.Now;

    public enum PlayDirectionEnum {

      Forward,
      Reverse
    }

    protected internal Recorder(EZB ezb) {

      _ezb = ezb;
    }

    public bool IsRecording {
      get {
        return _isRecording;
      }
    }

    internal void AddItem(byte[] cmdData) {

      DateTime dt = DateTime.Now;

      if (_recordingEventList.Count == 0)
        _dateTime = dt;

      int ms = (int)(dt - _dateTime).TotalMilliseconds;

      _recordingEventList.Add(new Classes.RecordingEvent(cmdData, ms));
    }

    public void StartRecording() {

      _recordingEventList.Clear();

      _isRecording = true;
    }

    public void ResumeRecording() {

      _isRecording = true;
    }

    public void StopRecording() {

      _isRecording = false;
    }

    public void ClearRecordingEvents() {

      _recordingEventList.Clear();
    }

    public int GetRecordingEventCount {
      get {
        return _recordingEventList.Count;
      }
    }

    public Classes.RecordingEvent[] GetRecordingEvents {
      get {
        return _recordingEventList.ToArray();
      }
    }

    public DateTime GetRecordingStartDate {
      get {
        return _dateTime;
      }
    }

    public async void Play() {

      await Play(_recordingEventList.ToArray(), PlayDirectionEnum.Forward);
    }

    public async void Play(PlayDirectionEnum playDirection) {

      await Play(_recordingEventList.ToArray(), playDirection);
    }

    public async Task Play(EZ_B.Classes.RecordingEvent[] recordingEvents, PlayDirectionEnum playDirection) {

      IOrderedEnumerable<Classes.RecordingEvent> recordingEventList;

      if (playDirection == PlayDirectionEnum.Forward)
        recordingEventList = from m in recordingEvents
                             orderby m.MS ascending
                             select m;
      else
        recordingEventList = from m in recordingEvents
                             orderby m.MS descending
                             select m;

      int lastMS = 0;

      foreach (EZ_B.Classes.RecordingEvent recordingEvent in recordingEventList) {

        if (lastMS == 0)
          lastMS = recordingEvent.MS;

        await Task.Delay(Math.Abs(recordingEvent.MS - lastMS));

        lastMS = recordingEvent.MS;

        DoEvent(recordingEvent);
      }
    }

    public async void DoEvent(EZ_B.Classes.RecordingEvent recordingEvent) {

      await _ezb.sendCommandData(0, recordingEvent.CmdData);
    }
  }
}
