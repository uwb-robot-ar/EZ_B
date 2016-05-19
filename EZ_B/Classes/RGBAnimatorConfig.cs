using System;
using System.Collections.Generic;

namespace EZ_B.Classes {

  public class RGBAnimatorConfig {

    public RGBAnimatorAction  [] Actions = new RGBAnimatorAction[] { };

    public RGBAnimatorConfig() {

    }

    public RGBAnimatorActionFrame GetFrameByGUID(string GUID) {

      foreach (RGBAnimatorAction action in Actions)
        foreach (RGBAnimatorActionFrame frame in action.Frames)
          if (frame.GUID == GUID)
            return frame;

      throw new Exception("Cannot find Frame with GUID: " + GUID);
    }

    public RGBAnimatorAction GetActionByGUID(string GUID) {

      foreach (RGBAnimatorAction action in Actions)
        if (action.GUID == GUID)
          return action;

      throw new Exception("Cannot find Action with GUID: " + GUID);
    }

    public RGBAnimatorAction GetActionByTitle(string title) {

      foreach (RGBAnimatorAction action in Actions)
        if (action.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
          return action;

      throw new Exception("Cannot find Action with title: " + title);
    }

    public void RemoveActionByGUID(string guid) {

      List<RGBAnimatorAction> actions = new List<RGBAnimatorAction>(Actions);

      for (int x=0; x < Actions.Length; x++)
        if (Actions[x].GUID == guid) 
          actions.RemoveAt(x);

      Actions = actions.ToArray();
    }

    public string AddAction(RGBAnimatorAction action) {

      List<RGBAnimatorAction> actions = new List<RGBAnimatorAction>(Actions);

      actions.Add(action);
      
      Actions = actions.ToArray();

      return action.GUID;
    }
  }
}
