using System;
using System.Threading.Tasks;

namespace EZ_B {

  public class HT16K33Animator : DisposableBase {

    EZB _ezb = null;
    EZBGWorker _tf;
    HT16K33 _ht = null;

    public delegate void OnCompleteHandler();
    public delegate void OnStartActionHandler(Classes.HT16K33AnimatorAction action);

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
    public HT16K33Animator(EZB ezb, string name) {

      Name = name;

      _ezb = ezb;

      _ht = new HT16K33(ezb);

      _tf = new EZBGWorker(string.Format("HT16K33 Animator for {0}", name));
      _tf.DoWork += _tf_DoWork;
      _tf.RunWorkerCompleted += _tf_RunWorkerCompleted;
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

    private void startWorker(Classes.HT16K33AnimatorAction action) {

      _tf.RunWorkerAsync(action);
    }

    private void _tf_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e) {

      if (OnComplete != null)
        OnComplete();
    }

    private async Task _tf_DoWork(Object sender, DoWorkEventArgs e) {

      Classes.HT16K33AnimatorAction action = (Classes.HT16K33AnimatorAction)e.Argument;

      try {

        do {

          foreach (Classes.HT16K33AnimatorActionFrame frame in action.Frames) {

            if (!_ezb.IsConnected)
              return;

            _ht.UpdateLEDs(frame.Matrix);

            await Task.Delay(frame.PauseTimeMS);

            if (_tf.CancellationPending)
              return;
          }
        } while (action.Repeats && !_tf.CancellationPending);
      } catch (Exception ex) {

        _ezb.Log(false, "Error in thread worker of HT16K33 Animator: {0}", ex);
      }
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
    public void ExecAction(Classes.HT16K33AnimatorAction action) {

      if (OnStartAction != null)
        OnStartAction(action);

      startWorker(action);
    }
  }
}

