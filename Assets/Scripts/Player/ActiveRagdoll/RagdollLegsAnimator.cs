using UnityEngine;

public class RagdollLegsAnimator : MonoBehaviour
{
    #region AccessableFields
    [SerializeField] private GameObject[] _playerParts;
    [SerializeField] private ConfigurableJoint[] _jointParts;

    [Space(3)]
    [Header("IsWalking")]
    [SerializeField] private bool _walkForward;
    [SerializeField] private bool _walkBackward;

    [Space(3)]
    [Header("StepValues")]
    [SerializeField] private float _timeStep = 0.4f;
    [SerializeField] private float _legsHeight = 0.8f;

    [Space(3)]
    [Header("AnimationCurves")]
    [Header("Forward")]
    [SerializeField] private AnimationCurve _upperLegCurveForward;
    [SerializeField] private AnimationCurve _lowerLegCurveForward;

    [Space(3)]
    [Header("Backward")]
    [SerializeField] private AnimationCurve _upperLegCurveBackward;
    [SerializeField] private AnimationCurve _lowerLegCurveBackward;
    #endregion

    #region PrivateFields
    private float _rightStepTime, _leftStepTime;
    private bool _stepRight, _stepLeft, _flagLegRight, _flagLegLeft;
    private Quaternion _startRightUpperLegRotation, _startRightLowerLegRotation, _startLeftUpperLegRotation, _startLeftLowerLegRotation;
    #endregion

    private void Awake()
    {
        _startRightUpperLegRotation = _jointParts[4].targetRotation;
        _startRightLowerLegRotation = _jointParts[5].targetRotation;
        _startLeftUpperLegRotation = _jointParts[7].targetRotation; 
        _startLeftLowerLegRotation = _jointParts[8].targetRotation;
    }

    private void FixedUpdate()
    {
        LegsMoving();
    }

    private void LegsMoving()
    {
        if (_walkForward)
        {
            if (_playerParts[6].transform.position.z < _playerParts[9].transform.position.z && !_stepLeft && !_flagLegRight)
            {
                _stepRight = true;
                _flagLegRight = true;
                _flagLegLeft = true;
            }
            if (_playerParts[6].transform.position.z > _playerParts[9].transform.position.z && !_stepRight && !_flagLegLeft)
            {
                _stepLeft = true;
                _flagLegLeft = true;
                _flagLegRight = true;
            }

            if (_stepRight)
            {
                _rightStepTime += Time.fixedDeltaTime;

                float t = Mathf.Clamp01(_rightStepTime / _timeStep);

                float upperLegRotationValue = _upperLegCurveForward.Evaluate(t) * -0.06f * _legsHeight;
                float lowerLegRotationValue = _lowerLegCurveForward.Evaluate(t) * 0.08f * _legsHeight * 2;

                _jointParts[4].targetRotation = new Quaternion(_jointParts[4].targetRotation.x + upperLegRotationValue, _jointParts[4].targetRotation.y, _jointParts[4].targetRotation.z, _jointParts[4].targetRotation.w);
                _jointParts[5].targetRotation = new Quaternion(_jointParts[5].targetRotation.x + lowerLegRotationValue, _jointParts[5].targetRotation.y, _jointParts[5].targetRotation.z, _jointParts[5].targetRotation.w);

                if (_rightStepTime > _timeStep)
                {
                    _rightStepTime = 0;
                    _stepRight = false;

                    _stepLeft = true;
                }
            }
            else
            {
                _jointParts[4].targetRotation = Quaternion.Lerp(_jointParts[4].targetRotation, new Quaternion(_startRightUpperLegRotation.x + 0.3f, _jointParts[4].targetRotation.y, _jointParts[4].targetRotation.z, _jointParts[4].targetRotation.w), 8 * Time.fixedDeltaTime);
                _jointParts[5].targetRotation = Quaternion.Lerp(_jointParts[5].targetRotation, _startRightLowerLegRotation, 17 * Time.fixedDeltaTime);
            }

            if (_stepLeft)
            {
                _leftStepTime += Time.fixedDeltaTime;

                float t = Mathf.Clamp01(_leftStepTime / _timeStep);

                float upperLegRotationValue = _upperLegCurveForward.Evaluate(t) * -0.06f * _legsHeight;
                float lowerLegRotationValue = _lowerLegCurveForward.Evaluate(t) * 0.08f * _legsHeight * 2;

                _jointParts[7].targetRotation = new Quaternion(_jointParts[7].targetRotation.x + upperLegRotationValue, _jointParts[7].targetRotation.y, _jointParts[7].targetRotation.z, _jointParts[7].targetRotation.w);
                _jointParts[8].targetRotation = new Quaternion(_jointParts[8].targetRotation.x + lowerLegRotationValue, _jointParts[8].targetRotation.y, _jointParts[8].targetRotation.z, _jointParts[8].targetRotation.w);

                _jointParts[4].targetRotation = new Quaternion(_jointParts[4].targetRotation.x - 0.02f * _legsHeight / 2, _jointParts[4].targetRotation.y, _jointParts[4].targetRotation.z, _jointParts[4].targetRotation.w);

                if (_leftStepTime > _timeStep)
                {
                    _leftStepTime = 0;
                    _stepLeft = false;

                    _stepRight = true;
                }
            }
            else
            {
                _jointParts[7].targetRotation = Quaternion.Lerp(_jointParts[7].targetRotation, new Quaternion(_startLeftUpperLegRotation.x + 0.3f, _jointParts[7].targetRotation.y, _jointParts[7].targetRotation.z, _jointParts[7].targetRotation.w), 8 * Time.fixedDeltaTime);
                _jointParts[8].targetRotation = Quaternion.Lerp(_jointParts[8].targetRotation, _startLeftLowerLegRotation, 17 * Time.fixedDeltaTime);
            }
        }

        if (_walkBackward)
        {
            if (_playerParts[6].transform.position.z > _playerParts[9].transform.position.z && !_stepLeft && !_flagLegRight)
            {
                _stepRight = true;
                _flagLegRight = true;
                _flagLegLeft = true;
            }
            if (_playerParts[6].transform.position.z < _playerParts[9].transform.position.z && !_stepRight && !_flagLegLeft)
            {
                _stepLeft = true;
                _flagLegLeft = true;
                _flagLegRight = true;
            }

            if (_stepRight)
            {
                _rightStepTime += Time.fixedDeltaTime;

                _jointParts[4].targetRotation = new Quaternion(_jointParts[4].targetRotation.x - 0.00f * _legsHeight, _jointParts[4].targetRotation.y, _jointParts[4].targetRotation.z, _jointParts[4].targetRotation.w);
                _jointParts[5].targetRotation = new Quaternion(_jointParts[5].targetRotation.x - 0.06f * _legsHeight * 2, _jointParts[5].targetRotation.y, _jointParts[5].targetRotation.z, _jointParts[5].targetRotation.w);

                _jointParts[7].targetRotation = new Quaternion(_jointParts[7].targetRotation.x + 0.02f * _legsHeight / 2, _jointParts[7].targetRotation.y, _jointParts[7].targetRotation.z, _jointParts[7].targetRotation.w);

                if (_rightStepTime > _timeStep)
                {
                    _rightStepTime = 0;
                    _stepRight = false;

                    _stepLeft = true;
                }
            }
            else
            {
                _jointParts[4].targetRotation = Quaternion.Lerp(_jointParts[4].targetRotation, _startRightUpperLegRotation, (8f) * Time.fixedDeltaTime);
                _jointParts[5].targetRotation = Quaternion.Lerp(_jointParts[5].targetRotation, _startRightLowerLegRotation, (17f) * Time.fixedDeltaTime);
            }

            if (_stepLeft)
            {
                _leftStepTime += Time.fixedDeltaTime;

                _jointParts[7].targetRotation = new Quaternion(_jointParts[7].targetRotation.x - 0.00f * _legsHeight, _jointParts[7].targetRotation.y, _jointParts[7].targetRotation.z, _jointParts[7].targetRotation.w);
                _jointParts[8].targetRotation = new Quaternion(_jointParts[8].targetRotation.x - 0.06f * _legsHeight * 2, _jointParts[8].targetRotation.y, _jointParts[8].targetRotation.z, _jointParts[8].targetRotation.w);

                _jointParts[4].targetRotation = new Quaternion(_jointParts[4].targetRotation.x + 0.02f * _legsHeight / 2, _jointParts[4].targetRotation.y, _jointParts[4].targetRotation.z, _jointParts[4].targetRotation.w);

                if (_leftStepTime > _timeStep)
                {
                    _leftStepTime = 0;
                    _stepLeft = false;

                    _stepRight = true;
                }
            }
            else
            {
                _jointParts[7].targetRotation = Quaternion.Lerp(_jointParts[7].targetRotation, _startLeftUpperLegRotation, (8) * Time.fixedDeltaTime);
                _jointParts[8].targetRotation = Quaternion.Lerp(_jointParts[8].targetRotation, _startLeftLowerLegRotation, (17) * Time.fixedDeltaTime);
            }
        }
    }
}