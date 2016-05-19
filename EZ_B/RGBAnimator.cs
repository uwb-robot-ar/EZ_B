using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZ_B {

  public class RGBAnimator : DisposableBase {

    EZB _ezb = null;
    EZBGWorker _tf = null;
    RGBEyes _eyes = null;

    public delegate void OnCompleteHandler();
    public delegate void OnStartActionHandler(Classes.RGBAnimatorAction action);

    /// <summary>
    /// Event risen when movement is complete
    /// </summary>
    public event OnCompleteHandler OnComplete;

    /// <summary>
    /// Event risen when an action is started
    /// </summary>
    public event OnStartActionHandler OnStartAction;

    /// <summary>
    /// Unique name for this auto position instance
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Create an instance of the AutoPosition Control
    /// </summary>
    public RGBAnimator(EZB ezb, string name) {

      Name = name;

      _ezb = ezb;

      _eyes = new RGBEyes(_ezb);

      _tf = new EZBGWorker(string.Format("RGB Animator for: {0}", name));
      _tf.DoWork += _tf_DoWork;
      _tf.RunWorkerCompleted += _tf_RunWorkerCompleted;
    }

    private void _tf_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e) {

      if (OnComplete != null)
        OnComplete();
    }


    /// <summary>
    /// Dispose of the AutoPositioner
    /// </summary>    
    protected override void DisposeOverride() {

      OnComplete = null;
      OnStartAction = null;

      Stop();
    }

    private async void stopWorker() {

      await _tf.CancelWorker();
    }

    private void startWorker(Classes.RGBAnimatorAction action) {

      _tf.RunWorkerAsync(action);
    }

    private async Task _tf_DoWork(Object sender, DoWorkEventArgs e) {

      Classes.RGBAnimatorAction action = (Classes.RGBAnimatorAction)e.Argument;

      do {

        foreach (Classes.RGBAnimatorActionFrame frame in action.Frames) {

          if (frame.UseTransition) {

            List<decimal> stepIncrementR = new List<decimal>();
            List<decimal> stepIncrementG = new List<decimal>();
            List<decimal> stepIncrementB = new List<decimal>();
            List<decimal> startingPositionR = new List<decimal>();
            List<decimal> startingPositionG = new List<decimal>();
            List<decimal> startingPositionB = new List<decimal>();

            // Get greatest position movement count
            // ---------------------------------------------------------------------------------------------------------------

            int currentPositionMin = 0;
            int toPositionMax = 0;
            for (byte x = 0; x < RGBEyes.INDEX_MAX; x++) {

              currentPositionMin = Functions.Min(currentPositionMin, _eyes.GetRed(x), _eyes.GetGreen(x), _eyes.GetBlue(x));
              toPositionMax = Functions.Max(toPositionMax, frame.Red[x], frame.Green[x], frame.Blue[x]);
            }

            int stepCount = Math.Abs(currentPositionMin - toPositionMax);

            if (stepCount == 0)
              continue;

            // Determine Step
            // Initialize startingPosition for a decimal point value of the current position
            // Decimal is used for current position for accuracy
            // ---------------------------------------------------------------------------------------------------------------
            for (byte x = 0; x < RGBEyes.INDEX_MAX; x++) {

              stepIncrementR.Add(Math.Abs(frame.Red[x] - _eyes.GetRed(x)) / (decimal)stepCount);
              stepIncrementG.Add(Math.Abs(frame.Green[x] - _eyes.GetGreen(x)) / (decimal)stepCount);
              stepIncrementB.Add(Math.Abs(frame.Blue[x] - _eyes.GetBlue(x)) / (decimal)stepCount);

              startingPositionR.Add(_eyes.GetRed(x));
              startingPositionG.Add(_eyes.GetGreen(x));
              startingPositionB.Add(_eyes.GetBlue(x));
            }

            // Execute Steps
            // ---------------------------------------------------------------------------------------------------------------
            int cnt = 0;

            while (!_tf.CancellationPending) {

              List<RGBEyes.RGBDef> defs = new List<RGBEyes.RGBDef>();

              for (byte x = 0; x < RGBEyes.INDEX_MAX; x++) {

                decimal newPositionR = 0;
                decimal newPositionG = 0;
                decimal newPositionB = 0;

                if (startingPositionR[x] < frame.Red[x])
                  newPositionR = _eyes.GetRed(x) + stepIncrementR[x];
                else
                  newPositionR = _eyes.GetRed(x) - stepIncrementR[x];

                if (startingPositionG[x] < frame.Green[x])
                  newPositionG = _eyes.GetGreen(x) + stepIncrementG[x];
                else
                  newPositionG = _eyes.GetGreen(x) - stepIncrementG[x];

                if (startingPositionB[x] < frame.Blue[x])
                  newPositionB = _eyes.GetBlue(x) + stepIncrementB[x];
                else
                  newPositionB = _eyes.GetBlue(x) - stepIncrementB[x];

                byte r = (byte)Math.Max(0, newPositionR);
                byte g = (byte)Math.Max(0, newPositionG);
                byte b = (byte)Math.Max(0, newPositionB);

                defs.Add(new RGBEyes.RGBDef() {
                  Index = x,
                  Red = r,
                  Green = g,
                  Blue = b
                });

                if (_tf.CancellationPending)
                  return;
              }

              if (!_ezb.IsConnected)
                return;

              _eyes.SetColor(defs.ToArray());

              await Task.Delay(frame.TransitionTimeMS / stepCount);

              if (cnt == stepCount)
                break;

              cnt++;
            }
          } else {

            List<RGBEyes.RGBDef> defs = new List<RGBEyes.RGBDef>();

            for (byte x = 0; x < RGBEyes.INDEX_MAX; x++) {

              defs.Add(new RGBEyes.RGBDef() {
                Index = x,
                Red = frame.Red[x],
                Green = frame.Green[x],
                Blue = frame.Blue[x]
              });
            }

            if (!_ezb.IsConnected)
              return;

            _eyes.SetColor(defs.ToArray());
          }

          await Task.Delay(frame.PauseTimeMS);

          if (_tf.CancellationPending)
            return;
        }
      } while (action.Repeats && !_tf.CancellationPending);
    }

    /// <summary>
    /// Stops the current movement. Blocks until stop is successful.
    /// </summary>
    public void Stop() {

      stopWorker();
    }

    /// <summary>
    /// Execute the Action
    /// </summary>
    public void ExecAction(Classes.RGBAnimatorAction action) {

      if (OnStartAction != null)
        OnStartAction(action);

      startWorker(action);
    }
  }
}

