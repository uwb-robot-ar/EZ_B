using System;
using System.Collections.Generic;

namespace EZ_B.Classes {

  public class RGBAnimatorAction {

    public string                    Title   = string.Empty;
    public bool                      Repeats = false;
    public string                    GUID    = Guid.NewGuid().ToString();
    public RGBAnimatorActionFrame [] Frames  = new RGBAnimatorActionFrame[] {};

    public RGBAnimatorAction() {
    }

    public RGBAnimatorAction(string title, string guid, bool repeats, RGBAnimatorActionFrame [] frames ) {

      Title = title;
      Repeats = repeats;
      GUID = guid;
      Frames = frames;
    }

    public RGBAnimatorAction(string title, bool repeats, RGBAnimatorActionFrame [] frames ) {

      Title = title;
      Repeats = repeats;
      Frames = frames;
    }

    public void AddFrame(RGBAnimatorActionFrame frame ) {

      List<RGBAnimatorActionFrame> frames = new List<RGBAnimatorActionFrame>(Frames);

      frames.Add(frame);

      Frames = frames.ToArray();
    }

    public void RemoveFrameAtIndex(int index) {

      List<RGBAnimatorActionFrame> frames = new List<RGBAnimatorActionFrame>(Frames);

      frames.RemoveAt(index);

      Frames = frames.ToArray();
    }

    public void RemoveFrameByGUID(string guid) {

      List<RGBAnimatorActionFrame> frames = new List<RGBAnimatorActionFrame>(Frames);

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
