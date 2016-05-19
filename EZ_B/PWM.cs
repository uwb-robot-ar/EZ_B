namespace EZ_B {

  public class PWM {

    EZB _ezb;

    private volatile int []      _pwmPositions    = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private volatile bool []     _pwmReleased     = { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };

    /// <summary>
    ///  The maximum value for a PWM (100)
    /// </summary>
    public static readonly int PWM_MAX = 100;

    /// <summary>
    ///  The minimum value of a PWM (0)
    /// </summary>
    public static readonly int PWM_MIN = 0;

    protected internal PWM(EZB ezb) {

      _ezb = ezb;
    }

    /// <summary>
    /// Set the PWM Speed. The speed can be between PWM_MIN and PWM_MAX
    /// </summary>
    public void SetPWM(Digital.DigitalPortEnum pwmPort, int speed) {

      if (!_ezb.IsConnected)
        return;

      if (speed > PWM_MAX)
        speed = PWM_MAX;
      else if (speed < PWM_MIN)
        speed = PWM_MIN;

      if (speed == 0) {

        StopPWM(pwmPort);

        return;
      }

      _pwmPositions[(int)pwmPort] = speed;
      _pwmReleased[(int)pwmPort] = false;

      _ezb.sendCommand(EZB.CommandEnum.CmdSetPWMSpeed + (byte)pwmPort, new byte[] { (byte)speed });
    }

    /// <summary>
    /// Get the PWM
    /// </summary>
    public int GetPWM(Digital.DigitalPortEnum pwmPort) {

      return _pwmPositions[(int)pwmPort];
    }

    /// <summary>
    /// Stop PWM.
    /// </summary>
    public void StopPWM(Digital.DigitalPortEnum pwmPort) {

      _pwmPositions[(int)pwmPort] = 0;
      _pwmReleased[(int)pwmPort] = true;

      _ezb.sendCommand(EZB.CommandEnum.CmdSetPWMSpeed + (byte)pwmPort, new byte[] { (byte)0 });
    }

    /// <summary>
    /// Return true if the specified pwm port is in a stopped state
    /// </summary>
    public bool IsPWMStopped(Digital.DigitalPortEnum pwmPort) {

      return _pwmReleased[(int)pwmPort];
    }
  }
}
