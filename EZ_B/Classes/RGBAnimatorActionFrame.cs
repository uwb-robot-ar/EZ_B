using System;

namespace EZ_B.Classes {

  public class RGBAnimatorActionFrame {

    public string   GUID             = Guid.NewGuid().ToString();
    public byte []  Red              = new byte[18] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public byte []  Green            = new byte[18] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public byte []  Blue             = new byte[18] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public bool     UseTransition    = true;
    public int      TransitionTimeMS = 1000;
    public int      PauseTimeMS      = 1000;

    public RGBAnimatorActionFrame() {

    }

    public RGBAnimatorActionFrame(string guid, bool transition, int transitionTimeMS, int pauseTimeMS) {

      GUID = guid;
      UseTransition = transition;
      TransitionTimeMS = transitionTimeMS;
      PauseTimeMS = pauseTimeMS;
    }

    public RGBAnimatorActionFrame(bool transition, int transitionTimeMS, int pauseTimeMS) {

      UseTransition = transition;
      TransitionTimeMS = transitionTimeMS;
      PauseTimeMS = pauseTimeMS;
    }

    public override string ToString() {

      return GUID;      
    }
  }
}
