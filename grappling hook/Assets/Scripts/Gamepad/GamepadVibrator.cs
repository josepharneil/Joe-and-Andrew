using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadVibrator : MonoBehaviour
{
    private enum VibrationPattern
    {
        Constant,
        Pulse,
        Linear
    }

    [SerializeField] private PlayerInput playerInput;
    private VibrationPattern _activeVibrationPattern;
    private float _vibrationDuration;
    private float _pulseDuration;
    private float _lowA;
    private float _lowStep;
    private float _highA;
    private float _highStep;
    private float _rumbleStep;
    private bool _isMotorActive = false;
    private Gamepad _gamepad;

    private void Start()
    {
        _gamepad = GetGamepad();
    }

    // Public Methods
    public void VibrationConstant(float low, float high, float duration)
    {
        _activeVibrationPattern = VibrationPattern.Constant;
        _lowA = low;
        _highA = high;
        _vibrationDuration = Time.time + duration;
    }

    public void VibrationPulse(float lowFrequencyMotor, float highFrequencyMotor, float burstTime, float duration)
    {
        _activeVibrationPattern = VibrationPattern.Pulse;
        _lowA = lowFrequencyMotor;
        _highA = highFrequencyMotor;
        _rumbleStep = burstTime;
        _pulseDuration = Time.time + burstTime;
        _vibrationDuration = Time.time + duration;
        _isMotorActive = true;
        _gamepad?.SetMotorSpeeds(_lowA, _highA);
    }

    public void VibrationLinear(float lowStart, float lowEnd, float highStart, float highEnd, float duration)
    {
        _activeVibrationPattern = VibrationPattern.Linear;
        _lowA = lowStart;
        _highA = highStart;
        _lowStep = (lowEnd - lowStart) / duration;
        _highStep = (highEnd - highStart) / duration;
        _vibrationDuration = Time.time + duration;
    }

    private void StopRumble()
    {
        _gamepad?.SetMotorSpeeds(0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gamepad == null)
        {
            return;
        }
        if (Time.time > _vibrationDuration)
        {
            StopRumble();
            return;
        }

        switch (_activeVibrationPattern)
        {
            case VibrationPattern.Constant:
                _gamepad.SetMotorSpeeds(_lowA, _highA);
                break;

            case VibrationPattern.Pulse:
                if (Time.time > _pulseDuration)
                {
                    _isMotorActive = !_isMotorActive;
                    _pulseDuration = Time.time + _rumbleStep;
                    if (!_isMotorActive)
                    {
                        _gamepad.SetMotorSpeeds(0, 0);
                    }
                    else
                    {
                        _gamepad.SetMotorSpeeds(_lowA, _highA);
                    }
                }
                break;
            
            case VibrationPattern.Linear:
                _gamepad.SetMotorSpeeds(_lowA, _highA);
                _lowA += (_lowStep * Time.deltaTime);
                _highA += (_highStep * Time.deltaTime);
                break;
            
            default:
                break;
        }
    }
    
    private Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => playerInput.devices.Any(d => d.deviceId == g.deviceId));

        #region Linq Query Equivalent Logic
        //Gamepad gamepad = null;
        //foreach (var g in Gamepad.all)
        //{
        //    foreach (var d in _playerInput.devices)
        //    {
        //        if(d.deviceId == g.deviceId)
        //        {
        //            gamepad = g;
        //            break;
        //        }
        //    }
        //    if(gamepad != null)
        //    {
        //        break;
        //    }
        //}
        //return gamepad;
        #endregion
    }
}
