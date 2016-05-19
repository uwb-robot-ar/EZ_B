using System;
using System.Collections.Generic;

namespace EZ_B.Classes {

  public class HT16K33AnimatorAction {

    public string                    Title   = string.Empty;
    public bool                      Repeats = false;
    public string                    GUID    = Guid.NewGuid().ToString();
    public HT16K33AnimatorActionFrame [] Frames  = new HT16K33AnimatorActionFrame[] {};

    public HT16K33AnimatorAction() {
    }

    public HT16K33AnimatorAction(string title, string guid, bool repeats, HT16K33AnimatorActionFrame [] frames ) {

      Title = title;
      Repeats = repeats;
      GUID = guid;
      Frames = frames;
    }

    public HT16K33AnimatorAction(string title, bool repeats, HT16K33AnimatorActionFrame[] frames) {

      Title = title;
      Repeats = repeats;
      Frames = frames;
    }

    public void AddFrame(HT16K33AnimatorActionFrame frame) {

      List<HT16K33AnimatorActionFrame> frames = new List<HT16K33AnimatorActionFrame>(Frames);

      frames.Add(frame);

      Frames = frames.ToArray();
    }

    public void RemoveFrameAtIndex(int index) {

      List<HT16K33AnimatorActionFrame> frames = new List<HT16K33AnimatorActionFrame>(Frames);

      frames.RemoveAt(index);

      Frames = frames.ToArray();
    }

    public void RemoveFrameByGUID(string guid) {

      List<HT16K33AnimatorActionFrame> frames = new List<HT16K33AnimatorActionFrame>(Frames);

      for (int x=0; x < Frames.Length; x++)
        if (Frames[x].GUID == guid) {

          frames.RemoveAt(x);

          x--;
        }

      Frames = frames.ToArray();
    }

    public override string ToString() {

      return Title;
    }
  }
}
