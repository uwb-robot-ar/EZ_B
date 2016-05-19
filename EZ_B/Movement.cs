namespace EZ_B {

  public class Movement {

    private EZB _ezb;

    public delegate void OnMovementHandler(MovementDirectionEnum direction);
    public delegate void OnSpeedChangedHandler(int speedLeft, int speedRight);

    public static readonly byte MAX_SPEED = 255;
    public static readonly byte MIN_SPEED = 0;

    private byte                  _speedUserLeft    = 0;
    private byte                  _speedUserRight   = 0;
    private byte                  _speedLeft        = 0;
    private byte                  _speedRight       = 0;
    private MovementDirectionEnum _currentDirection = MovementDirectionEnum.Stop;
    private MovementTypeEnum      _movementType     = MovementTypeEnum.Unknown;

    /// <summary>
    /// Event risen when for movement action
    /// </summary>
    public event OnMovementHandler OnMovement;

    /// <summary>
    /// Event risen when for speed changed
    /// </summary>
    public event OnSpeedChangedHandler OnSpeedChanged;

    public enum MovementDirectionEnum {
      Stop,
      Forward,
      Reverse,
      Left,
      Right,
      Up,
      Down,
      RollRight,
      RollLeft
    }

    public enum MovementTypeEnum {

      Unknown,
      HBridge,
      HBridgePWM,
      ModifiedServo,
      Roomba,
      BV4113,
      ARDrone,
      BrookstoneRover,
      Sabertooth,
      RoboSapien,
      RoboQuad,
      AutoPosition,
      MIP
    }

    /// <summary>
    /// Set the type of movement type this control will use (Servo or HBridge?)
    /// </summary>
    public MovementTypeEnum MovementType {
      set {
        _movementType = value;
        SetSpeed(MAX_SPEED);
      }
      get {
        return _movementType;
      }
    }

    /// <summary>
    /// Get the current direction
    /// </summary>
    public MovementDirectionEnum GetCurrentDirection {
      get {
        return _currentDirection;
      }
    }

    /// <summary>
    /// Servo port for modified servo that acts as the left wheel (if set for Servo Type)
    /// </summary>
    public Servo.ServoPortEnum   ServoWheelLeftModifiedPort  = Servo.ServoPortEnum.D14;

    /// <summary>
    /// Servo port for modified servo that acts as the right wheel (if set for Servo Type)
    /// </summary>
    public Servo.ServoPortEnum   ServoWheelRightModifiedPort = Servo.ServoPortEnum.D13;

    /// <summary>
    /// The left wheel trigger A port of the H Bridge
    /// </summary>
    public Digital.DigitalPortEnum HBridgeLeftWheelTriggerA = Digital.DigitalPortEnum.D14;

    /// <summary>
    /// The left wheel trigger B port of the H Bridge
    /// </summary>
    public Digital.DigitalPortEnum HBridgeLeftWheelTriggerB = Digital.DigitalPortEnum.D13;

    /// <summary>
    /// The right wheel trigger A port of the H Bridge
    /// </summary>
    public Digital.DigitalPortEnum HBridgeRightWheelTriggerA = Digital.DigitalPortEnum.D12;

    /// <summary>
    /// The right wheel trigger B port of the H Bridge
    /// </summary>
    public Digital.DigitalPortEnum HBridgeRightWheelTriggerB = Digital.DigitalPortEnum.D11;

    /// <summary>
    /// The right wheel PWM
    /// </summary>
    public Digital.DigitalPortEnum HBridgeRightWheelPWM = Digital.DigitalPortEnum.D1;

    /// <summary>
    /// The lefgt wheel PWM
    /// </summary>
    public Digital.DigitalPortEnum HBridgeLeftWheelPWM = Digital.DigitalPortEnum.D2;

    /// <summary>
    /// Servos and R/C servo controllers have specified values for their speed control.
    /// Use this value to set it.
    /// </summary>
    public int ModifiedServoLeftForwardValue = Servo.SERVO_MIN;

    /// <summary>
    /// Servos and R/C servo controllers have specified values for their speed control.
    /// Use this value to set it.
    /// </summary>
    public int ModifiedServoLeftReverseValue = Servo.SERVO_MAX;

    /// <summary>
    /// Servos and R/C servo controllers have specified values for their speed control.
    /// Use this value to set it.
    /// </summary>
    public int ModifiedServoRightForwardValue = Servo.SERVO_MAX;

    /// <summary>
    /// Servos and R/C servo controllers have specified values for their speed control.
    /// Use this value to set it.
    /// </summary>
    public int ModifiedServoRightReverseValue = Servo.SERVO_MIN;

    /// <summary>
    /// For ESC - Some ESC Require a STOP value (neutral position) to be set.
    /// This does not need to be set for Modified Servos.
    /// Normally, if this is FALSE the EZ-B will simply stop sending a PWM signal. If this is set to True, the signal will send the specified Stop right and left values.
    /// </summary>
    public bool ModifiedServoUseStopValue = false;

    /// <summary>
    /// Specifieds the Stop Position for the ESC. Will not be used unless the ModifiedServoUseStopValue is set
    /// </summary>
    public int ModifiedServoRightStopValue = (Servo.SERVO_MAX - Servo.SERVO_MIN) / 2;

    /// <summary>
    /// Specifieds the Stop Position for the ESC. Will not be used unless the ModifiedServoUseStopValue is set
    /// </summary>
    public int ModifiedServoLeftStopValue = (Servo.SERVO_MAX - Servo.SERVO_MIN) / 2;

    /// <summary>
    /// Specifies the rate for moving a drone forward and reverse
    /// </summary>
    public float DroneForwardReverseRate = 0.20f;

    /// <summary>
    /// Specifies the rate for turning a drone
    /// </summary>
    public float DroneYawLeftRightRate = 0.20f;

    /// <summary>
    /// Specifies the rate for raising and lowering a drone
    /// </summary>
    public float DroneUpDownRate = 0.20f;

    /// <summary>
    /// Specifies the rate for rolling left or right
    /// </summary>
    public float DroneRollLeftRightRate = 0.20f;

    /// <summary>
    /// WowWee MIP UART Interface Port. By default, it uses the default UART 0 peripheral interface
    /// </summary>
    public int MIPUartPort = 0;

    protected internal Movement(EZB ezb) {

      _ezb = ezb;
    }

    private byte calculateSpeed(byte max, byte speed) {

      float scalar = Functions.GetScalarFromRange(max, MAX_SPEED, MIN_SPEED);

      return (byte)(scalar * speed);
    }

    /// <summary>
    /// Get the global speed
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public byte GetSpeed() {

      return _speedUserLeft;
    }

    /// <summary>
    /// Get the global speed for Left wheel
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public byte GetSpeedLeft() {

      return _speedUserLeft;
    }

    /// <summary>
    /// Get the global speed for Right wheel
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public byte GetSpeedRight() {

      return _speedUserRight;
    }

    /// <summary>
    /// Set the global speed
    /// </summary>
    public void SetSpeed(byte speed) {

      SetSpeed(speed, speed);
    }

    /// <summary>
    /// Set the global speed
    /// </summary>
    public void SetSpeed(byte speedLeft, byte speedRight) {

      setSpeedLeft(speedLeft);
      setSpeedRight(speedRight);

      if (OnSpeedChanged != null)
        OnSpeedChanged(_speedUserLeft, _speedUserRight);
    }

    /// <summary>
    /// Set the left speed
    /// </summary>
    public void SetSpeedLeft(byte speed) {

      setSpeedLeft(speed);

      if (OnSpeedChanged != null)
        OnSpeedChanged(_speedUserLeft, _speedUserRight);
    }

    /// <summary>
    /// Set the right speed
    /// </summary>
    public void SetSpeedRight(byte speed) {

      setSpeedRight(speed);

      if (OnSpeedChanged != null)
        OnSpeedChanged(_speedUserLeft, _speedUserRight);
    }

    private void setSpeedLeft(byte speed) {

      _speedUserLeft = speed;

      switch (MovementType) {
        case MovementTypeEnum.Unknown:
          break;
        case MovementTypeEnum.ModifiedServo:
          break;
        case MovementTypeEnum.HBridge:
          break;
        case MovementTypeEnum.HBridgePWM:
          _speedLeft = calculateSpeed(100, _speedUserLeft);
          _ezb.PWM.SetPWM(HBridgeLeftWheelPWM, _speedLeft);
          break;
        case MovementTypeEnum.Roomba:
          _speedLeft = calculateSpeed(250, _speedUserLeft);
          break;
        case MovementTypeEnum.BV4113:
          break;
        case MovementTypeEnum.ARDrone:
          break;
        case MovementTypeEnum.BrookstoneRover:
          _speedLeft = calculateSpeed(16, _speedUserLeft);
          break;
        case MovementTypeEnum.AutoPosition:
          break;
        case MovementTypeEnum.Sabertooth:
          break;
        case MovementTypeEnum.RoboSapien:
          break;
        case MovementTypeEnum.RoboQuad:
          break;
        case MovementTypeEnum.MIP:
          _speedLeft = calculateSpeed(0x80 - 0x61, _speedUserLeft);
          break;
      }
    }

    private void setSpeedRight(byte speed) {

      _speedUserRight = speed;

      switch (MovementType) {
        case MovementTypeEnum.Unknown:
          break;
        case MovementTypeEnum.ModifiedServo:
          break;
        case MovementTypeEnum.HBridge:
          break;
        case MovementTypeEnum.HBridgePWM:
          _speedRight = calculateSpeed(100, _speedUserRight);
          _ezb.PWM.SetPWM(HBridgeRightWheelPWM, _speedRight);
          break;
        case MovementTypeEnum.Roomba:
          break;
        case MovementTypeEnum.BV4113:
          break;
        case MovementTypeEnum.ARDrone:
          break;
        case MovementTypeEnum.BrookstoneRover:
          _speedRight = calculateSpeed(16, _speedUserRight);
          break;
        case MovementTypeEnum.AutoPosition:
          break;
        case MovementTypeEnum.Sabertooth:
          break;
        case MovementTypeEnum.RoboSapien:
          break;
        case MovementTypeEnum.RoboQuad:
          break;
        case MovementTypeEnum.MIP:
          _speedRight = calculateSpeed(0x60 - 0x41, _speedUserRight);
          break;
      }
    }

    /// <summary>
    /// Stops the robot if moving
    /// </summary>
    public void GoStop() {

      if (_currentDirection == MovementDirectionEnum.Stop && _movementType != MovementTypeEnum.AutoPosition)
        return;

      if (MovementType == MovementTypeEnum.ModifiedServo) {

        if (ModifiedServoUseStopValue) {

          _ezb.Servo.SetServoPosition(ServoWheelRightModifiedPort, ModifiedServoRightStopValue);
          _ezb.Servo.SetServoPosition(ServoWheelLeftModifiedPort, ModifiedServoLeftStopValue);
        } else {

          _ezb.Servo.ReleaseServo(ServoWheelRightModifiedPort);
          _ezb.Servo.ReleaseServo(ServoWheelLeftModifiedPort);
        }
      } else if (MovementType == MovementTypeEnum.HBridge) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.HBridgePWM) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.Roomba) {

        _ezb.Roomba.Stop();
      } else if (MovementType == MovementTypeEnum.BV4113) {

        _ezb.BV4113.Stop();
      } else if (MovementType == MovementTypeEnum.ARDrone) {

        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.BrookstoneRover) {

        _ezb.Log(true, "Brookstone Rover is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.Sabertooth) {

        _ezb.SabertoothSerial.Stop();
      } else if (MovementType == MovementTypeEnum.MIP) {

        _ezb.MIP.Stop();
      }

      _currentDirection = MovementDirectionEnum.Stop;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Moves robot forward at specified speed
    /// </summary>
    public void GoForward(byte speed) {

      GoForward(speed, speed);
    }

    /// <summary>
    /// Moves robot forward at specified speed
    /// </summary>
    public void GoForward(byte speedLeft, byte speedRight) {

      SetSpeed(speedLeft, speedRight);

      GoForward();
    }

    /// <summary>
    /// Moves robot forward
    /// </summary>
    public void GoForward() {

      if (_currentDirection == MovementDirectionEnum.Forward)
        return;

      if (MovementType == MovementTypeEnum.ModifiedServo) {

        _ezb.Servo.SetServoPosition(ServoWheelRightModifiedPort, ModifiedServoRightForwardValue);
        _ezb.Servo.SetServoPosition(ServoWheelLeftModifiedPort, ModifiedServoLeftForwardValue);
      } else if (MovementType == MovementTypeEnum.HBridge) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.HBridgePWM) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.Roomba) {

        _ezb.Roomba.Forward(_speedLeft);
      } else if (MovementType == MovementTypeEnum.BV4113) {

        _ezb.BV4113.Forward();
      } else if (MovementType == MovementTypeEnum.ARDrone) {

        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.BrookstoneRover) {

        _ezb.Log(true, "Brookstone Rover Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.Sabertooth) {

        _ezb.SabertoothSerial.Forward();
      } else if (MovementType == MovementTypeEnum.MIP) {

        _ezb.MIP.Forward(_speedLeft);
      }

      _currentDirection = MovementDirectionEnum.Forward;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Moves robot backward at specified speed
    /// </summary>
    public void GoReverse(byte speed) {

      GoReverse(speed, speed);
    }

    /// <summary>
    /// Moves robot backward at specified speed
    /// </summary>
    public void GoReverse(byte speedLeft, byte speedRight) {

      SetSpeed(speedLeft, speedRight);

      GoReverse();
    }

    /// <summary>
    /// Moves robot backward
    /// </summary>
    public void GoReverse() {

      if (_currentDirection == MovementDirectionEnum.Reverse)
        return;

      if (MovementType == MovementTypeEnum.ModifiedServo) {

        _ezb.Servo.SetServoPosition(ServoWheelRightModifiedPort, ModifiedServoRightReverseValue);
        _ezb.Servo.SetServoPosition(ServoWheelLeftModifiedPort, ModifiedServoLeftReverseValue);
      } else if (MovementType == MovementTypeEnum.HBridge) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, true);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, true);
      } else if (MovementType == MovementTypeEnum.HBridgePWM) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, true);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, true);
      } else if (MovementType == MovementTypeEnum.Roomba) {

        _ezb.Roomba.Reverse(_speedLeft);
      } else if (MovementType == MovementTypeEnum.BV4113) {

        _ezb.BV4113.Reverse();
      } else if (MovementType == MovementTypeEnum.ARDrone) {

        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.BrookstoneRover) {

        _ezb.Log(true, "Brookstone Rover Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.Sabertooth) {

        _ezb.SabertoothSerial.Reverse();
      } else if (MovementType == MovementTypeEnum.MIP) {

        _ezb.MIP.Reverse(_speedLeft);
      }

      _currentDirection = MovementDirectionEnum.Reverse;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Turns robot left at specified speed
    /// </summary>
    public void GoLeft(byte speed) {

      SetSpeed(speed, speed);

      GoLeft();
    }

    /// <summary>
    /// Turns robot left at specified speed
    /// </summary>
    public void GoLeft(byte speedLeft, byte speedRight) {

      SetSpeed(speedLeft, speedRight);

      GoLeft();
    }

    /// <summary>
    /// Turns robot left
    /// </summary>
    public void GoLeft() {

      if (_currentDirection == MovementDirectionEnum.Left)
        return;

      if (MovementType == MovementTypeEnum.ModifiedServo) {

        _ezb.Servo.SetServoPosition(ServoWheelRightModifiedPort, ModifiedServoRightForwardValue);
        _ezb.Servo.SetServoPosition(ServoWheelLeftModifiedPort, ModifiedServoLeftReverseValue);
      } else if (MovementType == MovementTypeEnum.HBridge) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, true);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.HBridgePWM) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, true);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, false);
      } else if (MovementType == MovementTypeEnum.Roomba) {

        _ezb.Roomba.Left(_speedLeft);
      } else if (MovementType == MovementTypeEnum.BV4113) {

        _ezb.BV4113.Left();
      } else if (MovementType == MovementTypeEnum.ARDrone) {

        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.BrookstoneRover) {

        _ezb.Log(true, "Brookstone Rover is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.Sabertooth) {

        _ezb.SabertoothSerial.Left();
      } else if (MovementType == MovementTypeEnum.MIP) {

        _ezb.MIP.Left(_speedLeft);
      }

      _currentDirection = MovementDirectionEnum.Left;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Turns robot right at specified speed
    /// </summary>
    public void GoRight(byte speed) {

      SetSpeed(speed, speed);

      GoRight();
    }

    /// <summary>
    /// Turns robot right at specified speed
    /// </summary>
    public void GoRight(byte speedLeft, byte speedRight) {

      SetSpeed(speedLeft, speedRight);

      GoRight();
    }

    /// <summary>
    /// Turns robot right
    /// </summary>
    public void GoRight() {

      if (_currentDirection == MovementDirectionEnum.Right)
        return;

      if (MovementType == MovementTypeEnum.ModifiedServo) {

        _ezb.Servo.SetServoPosition(ServoWheelRightModifiedPort, ModifiedServoRightReverseValue);
        _ezb.Servo.SetServoPosition(ServoWheelLeftModifiedPort, ModifiedServoLeftForwardValue);
      } else if (MovementType == MovementTypeEnum.HBridge) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, true);
      } else if (MovementType == MovementTypeEnum.HBridgePWM) {

        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerA, true);
        _ezb.Digital.SetDigitalPort(HBridgeLeftWheelTriggerB, false);

        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerA, false);
        _ezb.Digital.SetDigitalPort(HBridgeRightWheelTriggerB, true);
      } else if (MovementType == MovementTypeEnum.Roomba) {

        _ezb.Roomba.Right(_speedLeft);
      } else if (MovementType == MovementTypeEnum.BV4113) {

        _ezb.BV4113.Right();
      } else if (MovementType == MovementTypeEnum.ARDrone) {

        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.BrookstoneRover) {

        _ezb.Log(true, "Brookstone Rover is not supported in Mobile");
      } else if (MovementType == MovementTypeEnum.Sabertooth) {

        _ezb.SabertoothSerial.Right();
      } else if (MovementType == MovementTypeEnum.MIP) {

        _ezb.MIP.Right(_speedLeft);
      }

      _currentDirection = MovementDirectionEnum.Right;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Robot Goes Up (Drone flying robots)
    /// </summary>
    public void GoUp() {

      if (_currentDirection == MovementDirectionEnum.Up)
        return;

      if (MovementType == MovementTypeEnum.ARDrone)
        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");

      _currentDirection = MovementDirectionEnum.Up;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Robot Goes Down (Drone flying robots)
    /// </summary>
    public void GoDown() {

      if (_currentDirection == MovementDirectionEnum.Down)
        return;

      if (MovementType == MovementTypeEnum.ARDrone)
        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");

      _currentDirection = MovementDirectionEnum.Down;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Robot Rolls Right (Drone flying robots)
    /// </summary>
    public void GoRollRight() {

      if (_currentDirection == MovementDirectionEnum.RollRight)
        return;

      if (MovementType == MovementTypeEnum.ARDrone)
        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");

      _currentDirection = MovementDirectionEnum.RollRight;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }

    /// <summary>
    /// Robot Rolls Left (Drone flying robots)
    /// </summary>
    public void GoRollLeft() {

      if (_currentDirection == MovementDirectionEnum.RollLeft)
        return;

      if (MovementType == MovementTypeEnum.ARDrone)
        _ezb.Log(true, "AR Drone Movement is not supported in Mobile");

      _currentDirection = MovementDirectionEnum.RollLeft;

      if (OnMovement != null)
        OnMovement(_currentDirection);
    }
  }
}
