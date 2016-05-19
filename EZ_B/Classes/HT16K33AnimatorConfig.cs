using System;
using System.Collections.Generic;

namespace EZ_B.Classes {

  public class HT16K33AnimatorConfig {

    public HT16K33AnimatorAction  [] Actions = new HT16K33AnimatorAction[] { };

    public HT16K33AnimatorConfig() {

    }

    public HT16K33AnimatorActionFrame GetFrameByGUID(string GUID) {

      foreach (HT16K33AnimatorAction action in Actions)
        foreach (HT16K33AnimatorActionFrame frame in action.Frames)
          if (frame.GUID == GUID)
            return frame;

      throw new Exception("Cannot find Frame with GUID: " + GUID);
    }

    public HT16K33AnimatorAction GetActionByGUID(string GUID) {

      foreach (HT16K33AnimatorAction action in Actions)
        if (action.GUID == GUID)
          return action;

      throw new Exception("Cannot find Action with GUID: " + GUID);
    }

    public HT16K33AnimatorAction GetActionByTitle(string title) {

      foreach (HT16K33AnimatorAction action in Actions)
        if (action.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
          return action;

      throw new Exception("Cannot find Action with title: " + title);
    }

    public void RemoveActionByGUID(string guid) {

      List<HT16K33AnimatorAction> actions = new List<HT16K33AnimatorAction>(Actions);

      for (int x=0; x < Actions.Length; x++)
        if (Actions[x].GUID == guid) 
          actions.RemoveAt(x);

      Actions = actions.ToArray();
    }

    public string AddAction(HT16K33AnimatorAction action) {

      List<HT16K33AnimatorAction> actions = new List<HT16K33AnimatorAction>(Actions);

      actions.Add(action);
      
      Actions = actions.ToArray();

      return action.GUID;
    }
  }
}
