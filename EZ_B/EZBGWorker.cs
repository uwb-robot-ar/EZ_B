using System;
using System.Threading;
using System.Threading.Tasks;

namespace EZ_B {

  public class EZBGWorker {

    public event DoWorkEventHandler DoWork;
    public delegate Task DoWorkEventHandler(object sender, DoWorkEventArgs e);

    public event RunWorkerStartedEventHandler RunWorkerStarted;
    public delegate void RunWorkerStartedEventHandler(object sender);

    public event RunWorkerCompletedEventHandler RunWorkerCompleted;
    public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);

    CancellationTokenSource _token;

    Task _task;

    bool _completed = false;

    public string Name = string.Empty;

    public EZBGWorker(string name) {

      Name = name;
    }

    public async Task CancelWorker() {

      if (_token == null)
        return;

      _token.Cancel();

      while (!_completed)
        await Task.Delay(1);

      _task = null;
    }

    public bool CancellationPending {

      get {

        if (_token == null)
          return false;

        return _token.IsCancellationRequested;
      }
    }

    public bool IsBusy {
      get; set;
    }

    public void RunWorkerAsync() {

      RunWorkerAsync(null);
    }

    public async void RunWorkerAsync(object argument) {

      await CancelWorker();

      if (IsBusy)
        throw new Exception(string.Format("The worker for '{0}' is already running. This should be impossible because we try to cancel.", Name));

      if (DoWork == null)
        return;

      IsBusy = true;

      _completed = false;

      _token = new CancellationTokenSource();

      _task = Task.Factory.StartNew(async () => {

        try {

          if (RunWorkerStarted != null)
            RunWorkerStarted(this);

          var args = new DoWorkEventArgs() { Argument = argument };

          await DoWork(this, args);
        } finally {

          IsBusy = false;

          if (RunWorkerCompleted != null)
            RunWorkerCompleted(this, new RunWorkerCompletedEventArgs());

          _token.Dispose();
          _token = null;

          _completed = true;
        }
      },
      _token.Token);
    }
  }

  public class DoWorkEventArgs : EventArgs {

    public DoWorkEventArgs() {
    }

    public DoWorkEventArgs(object argument) {

      Argument = argument;
    }

    public object Argument {
      get; set;
    }

    public bool Cancel {
      get; set;
    }

    public object Result {
      get; set;
    }
  }

  public class RunWorkerCompletedEventArgs : EventArgs {

    public RunWorkerCompletedEventArgs() {
    }

    public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled) {

      Result = result;

      Error = error;

      Cancelled = cancelled;
    }

    public Exception Error {
      get; set;
    }

    public object Result {
      get; set;
    }

    public bool Cancelled {
      get; set;
    }
  }
}