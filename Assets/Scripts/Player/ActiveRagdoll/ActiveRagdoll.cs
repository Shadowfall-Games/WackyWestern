using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.ActiveRagdoll
{
    public class ActiveRagdoll : MonoBehaviour
    {
        [SerializeField]
        private GameObject
            _root, _body, _head,
            _upperRightArm, _lowerRightArm,
            _upperLeftArm, _lowerLeftArm,
            _upperRightLeg, _lowerRightLeg,
            _upperLeftLeg, _lowerLeftLeg,
            _rightFoot, _leftFoot;

        [SerializeField] private Rigidbody _rightHand, _leftHand;

        [SerializeField] private Transform _centerOfMass;

        [Header("Input on this player")]
        [SerializeField] private bool _useControls = true;

        [Header("The Layer Only This Player Is On")]
        [SerializeField] private LayerMask _thisPlayerLayer;

        [Header("Movement Properties")]
        [SerializeField] private bool _forwardIsCameraDirection = true;
        [SerializeField] private float _moveSpeed = 8;
        [SerializeField] private float _turnSpeed = 6;
        [SerializeField] private float _jumpForce = 10;

        [Header("Balance Properties")]
        [SerializeField] private bool _autoGetUpWhenPossible = true;
        [SerializeField] private bool _useStepPrediction = true;
        [SerializeField] private float _balanceHeight = 2.5f;
        [SerializeField] private float _balanceStrength = 5000;
        [SerializeField] private float _coreStrength = 1000;
        [SerializeField] private float _limbStrength = 450;

        [Header("Walking Animation Properties")]
        [SerializeField] private float _stepDuration = 0.2f;
        [SerializeField] private float _stepHeight = 1;
        [SerializeField] private float _feetMountForce = 150;

        [Header("Reach Properties")]
        [SerializeField] private float _reachSensitivity = 100;
        [SerializeField] private float _maxReachValue = 0.3f;
        [SerializeField] private float _minReachValue = 0.6f;
        [SerializeField] private float _armReachStiffness = 700;

        [Header("Actions")]
        [SerializeField] private bool _canBeKnockoutByImpact = true;
        [FormerlySerializedAs("_requiredForceToBeKO")] [SerializeField] private float _requiredForceToBeKo = 40;
        [SerializeField] private bool _canPunch = true;
        [SerializeField] private float _punchForce = 25;

        private float
            _timer, _stepRightTimer, _stepLeftTimer,
            _mouseYAxisArms, _mouseXAxisArms, _mouseYAxisBody;

        private bool
            _walkForward, _walkBackward,
            _stepRight, _stepLeft, _alertLegRight,
            _alertLegLeft, _balanced = true, _gettingUp,
            _resetPose, _isRagdoll, _isKeyDown, _moveAxisUsed,
            _jumpAxisUsed, _reachLeftAxisUsed, _reachRightAxisUsed,
            _isRightPunchButtonPressed, _isLeftPunchButtonPressed, _canRotate = true;

        private bool
            _jumping, _isJumping, _inAir,
            _punchingRight, _punchingLeft;

        private Camera _camera;
        private Vector3 _direction;
        private Vector3 _centerOfMassPoint;

        private GameObject[] _playerParts;

        private JointDrive
            _balanceOn, _poseOn, _coreStiffness, _reachStiffness, _driveOff;

        private Quaternion
            _bodyTarget,
            _upperRightArmTarget, _lowerRightArmTarget,
            _upperLeftArmTarget, _lowerLeftArmTarget,
            _upperRightLegTarget, _lowerRightLegTarget,
            _upperLeftLegTarget, _lowerLeftLegTarget;

        private Vector2 _move;
        private InputSystem _inputSystem;

        [Header("Player Editor Debug Mode")]
        [SerializeField] private bool _editorDebugMode;

        private void OnEnable()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Player.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.Player.Disable();
        }

        private void Awake() => PlayerSetup();

        private void Update()
        {
            _move = _inputSystem.Player.Movement.ReadValue<Vector2>();
            _isLeftPunchButtonPressed = _inputSystem.Player.PunchLeft.ReadValue<float>() > 0.1f;
            _isRightPunchButtonPressed = _inputSystem.Player.PunchRight.ReadValue<float>() > 0.1f;

            if (_balanced && _useStepPrediction)
            {
                StepPrediction();
                CenterOfMass();
            }

            if (!_useStepPrediction)
            {
                ResetWalkCycle();
            }
        }

        private void FixedUpdate()
        {
            Walking();

            if (_useControls && !_inAir)
            {
                PlayerMovement();

                if (_canPunch)
                {
                    PlayerPunch();
                }
            }

            if (_useControls)
            {
                PlayerReach();
            }

            if (_useControls)
            {
                PlayerRotation();
                ResetPlayerPose();

                PlayerGetUpJumping();
            }

            GroundCheck();
            CenterOfMass();
        }

        private JointDrive SetupDrive(float strength)
        {
            var jointDrive = new JointDrive();
            jointDrive.positionSpring = strength;
            jointDrive.positionDamper = 0;
            jointDrive.maximumForce = Mathf.Infinity;

            return jointDrive;
        }

        private void PlayerSetup()
        {
            _camera = Camera.main;

            _balanceOn = SetupDrive(_balanceStrength);
            _poseOn = SetupDrive(_limbStrength);
            _coreStiffness = SetupDrive(_coreStrength);
            _reachStiffness = SetupDrive(_armReachStiffness);
            _driveOff = SetupDrive(25);

            _playerParts = new GameObject[]
            {
                //array index numbers
			
                _root, //0
                _body, //1
                _head, //2
                _upperRightArm, //3
                _lowerRightArm, //4
                _upperLeftArm, //5
                _lowerLeftArm, //6
                _upperRightLeg, //7
                _lowerRightLeg, //8
                _upperLeftLeg, //9
                _lowerLeftLeg, //10
                _rightFoot, //11
                _leftFoot //12
            };

            _bodyTarget = _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation;
            _upperRightArmTarget = _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation;
            _lowerRightArmTarget = _playerParts[4].GetComponent<ConfigurableJoint>().targetRotation;
            _upperLeftArmTarget = _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation;
            _lowerLeftArmTarget = _playerParts[6].GetComponent<ConfigurableJoint>().targetRotation;
            _upperRightLegTarget = _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation;
            _lowerRightLegTarget = _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation;
            _upperLeftLegTarget = _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation;
            _lowerLeftLegTarget = _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation;
        }

        private void GroundCheck()
        {
            Ray ray = new Ray(_playerParts[0].transform.position, -_playerParts[0].transform.up);

            //Balance when ground is detected
            if (Physics.Raycast(ray, out _, _balanceHeight, 1 << LayerMask.NameToLayer("Ground")) && !_inAir && !_isJumping && !_reachRightAxisUsed && !_reachLeftAxisUsed)
            {
                if (!_balanced && _playerParts[0].GetComponent<Rigidbody>().linearVelocity.magnitude < 1f)
                {
                    if (_autoGetUpWhenPossible)
                    {
                        _balanced = true;
                    }
                }
            }

            //Fall over when ground is not detected
            else if (!Physics.Raycast(ray, out _, _balanceHeight, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (_balanced)
                {
                    _balanced = false;
                }
            }

            //Balance on/off
            if (_balanced && _isRagdoll) DeactivateRagdoll();
            else if (!_balanced && !_isRagdoll) ActivateRagdoll();
        }

        private void StepPrediction()
        {
            //Reset variables when balanced
            if (!_walkForward && !_walkBackward)
            {
                _stepRight = false;
                _stepLeft = false;
                _stepRightTimer = 0;
                _stepLeftTimer = 0;
                _alertLegRight = false;
                _alertLegLeft = false;
            }

            //Check direction to walk when off balance
            //Backwards

            if (_centerOfMass.position.z < _playerParts[11].transform.position.z && _centerOfMass.position.z < _playerParts[12].transform.position.z)
            {
                _walkBackward = true;
            }
            else
            {
                if (!_isKeyDown)
                {
                    _walkBackward = false;
                }
            }

            //Forward
            if (_centerOfMass.position.z > _playerParts[11].transform.position.z && _centerOfMass.position.z < _playerParts[12].transform.position.z)
            {
                _walkForward = true;
            }
            else
            {
                if (!_isKeyDown)
                {
                    _walkForward = false;
                }
            }
        }

        private void ResetWalkCycle()
        {
            //Reset variables when not moving
            if (_walkForward || _walkBackward) return;
        
            _stepRight = false;
            _stepLeft = false;
            _stepRightTimer = 0;
            _stepLeftTimer = 0;
            _alertLegRight = false;
            _alertLegLeft = false;
        }

        private void SetDrives(JointDrive drive, int fromValue, int toValue)
        {
            for (var i = fromValue; i <= toValue; i++)
            {
                _playerParts[i].GetComponent<ConfigurableJoint>().angularXDrive = drive;
                _playerParts[i].GetComponent<ConfigurableJoint>().angularYZDrive = drive;
            }
        }

        private void SetPlayerState(bool walkForward, bool walkBackward, bool moveAxisUsed, bool isKeyDown, JointDrive drive)
        {
            SetPlayerState(walkForward, walkBackward, moveAxisUsed, isKeyDown);

            if (_isRagdoll)
            {
                SetDrives(drive, 7, 12);
            }
        }

        private void SetPlayerState(bool walkForward, bool walkBackward, bool moveAxisUsed, bool isKeyDown)
        {
            _walkForward = walkForward;
            _walkBackward = walkBackward;
            _moveAxisUsed = moveAxisUsed;
            _isKeyDown = isKeyDown;
        }

        private void PlayerMovement()
        {
            //Move in camera direction
            if (_move.y > 0 || _move.x != 0 && _move.y >= 0)
            {
                _direction = _playerParts[0].transform.rotation * new Vector3(_move.x, 0.0f, _move.y);
                _direction.y = 0f;
                _playerParts[0].transform.GetComponent<Rigidbody>().linearVelocity = Vector3.Lerp(_playerParts[0].transform.GetComponent<Rigidbody>().linearVelocity, (_direction * _moveSpeed) + new Vector3(0, _playerParts[0].transform.GetComponent<Rigidbody>().linearVelocity.y, 0), 0.8f);

                if (_move.x != 0 || _move.y > 0 && _balanced)
                {
                    if (!_walkForward) SetPlayerState(true, false, true, true);
                }

                else if (_move.x == 0 && _move.y == 0)
                {
                    if (_walkForward) SetPlayerState(false, false, false, false);
                }
            }

            //Move in own direction
            else
            {
                //fix this part
                if (_move.y != 0)
                {
                    var v3 = _playerParts[0].GetComponent<Rigidbody>().transform.forward * (_move.y * _moveSpeed) +
                             _playerParts[0].GetComponent<Rigidbody>().transform.right * (_move.x * _moveSpeed);
                    _playerParts[0].GetComponent<Rigidbody>().linearVelocity = v3;
                }


                switch (_move.y)
                {
                    case > 0:
                    {
                        if (!_walkForward) SetPlayerState(true, false, true, true, _poseOn);
                        break;
                    }
                    case < 0:
                    {
                        if (!_walkBackward) SetPlayerState(false, true, true, true, _poseOn);
                        break;
                    }
                    case 0:
                    {
                        if (_walkForward || _walkBackward && _moveAxisUsed) SetPlayerState(false, false, false, false, _driveOff);
                        break;
                    }
                }
            }
        }

        private void PlayerRotation()
        {
            if (_forwardIsCameraDirection)
            {
                //Turn with camera
                var lookPos = _camera.transform.forward;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation, Quaternion.Inverse(rotation), Time.deltaTime * _turnSpeed);
            }
            else
            {
                //Self Direction
                //Turn with keys
                if (_move.x != 0)
                {
                    _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation, new Quaternion(_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.x, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.y - (_move.x * _turnSpeed), _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.w), 6 * Time.fixedDeltaTime);
                }

                //reset turn upon target rotation limit
                if (_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.y < -0.98f)
                {
                    _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.x, 0.98f, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.w);
                }

                else if (_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.y > 0.98f)
                {
                    _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.x, -0.98f, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[0].GetComponent<ConfigurableJoint>().targetRotation.w);
                }
            }
        }

        private void PlayerGetUpJumping()
        {
            if (_inputSystem.Player.Jump.IsPressed())
            {
                if (!_jumpAxisUsed)
                {
                    if (_balanced && !_inAir) _jumping = true;

                    else if (!_balanced) DeactivateRagdoll();
                }

                _jumpAxisUsed = true;
            }

            else
            {
                _jumpAxisUsed = false;
            }


            if (_jumping)
            {
                _isJumping = true;

                var v3 = _playerParts[0].GetComponent<Rigidbody>().transform.up * _jumpForce;
                v3.x = _playerParts[0].GetComponent<Rigidbody>().linearVelocity.x;
                v3.z = _playerParts[0].GetComponent<Rigidbody>().linearVelocity.z;
                _playerParts[0].GetComponent<Rigidbody>().linearVelocity = v3;
            }

            if (_isJumping)
            {
                _timer = _timer + Time.fixedDeltaTime;

                if (_timer > 0.2f)
                {
                    _timer = 0.0f;
                    _jumping = false;
                    _isJumping = false;
                    _inAir = true;
                }
            }
        }

        public void PlayerLanded()
        {
            if (_inAir && !_isJumping && !_jumping)
            {
                _inAir = false;
                _resetPose = true;
            }
        }

        private void PlayerReach()
        {
            if (true && _canRotate)
            {
                if (_mouseYAxisBody <= _maxReachValue && _mouseYAxisBody >= -_minReachValue)
                {
                    _mouseYAxisBody = _mouseYAxisBody + (_inputSystem.Player.Look.ReadValue<Vector2>().y / _reachSensitivity);
                }

                else if (_mouseYAxisBody > -_maxReachValue)
                {
                    _mouseYAxisBody = _maxReachValue;
                }

                else if (_mouseYAxisBody < -_minReachValue)
                {
                    _mouseYAxisBody = -_minReachValue;
                }

                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_mouseYAxisBody, 0, 0, 1);
            }


            //Reach Left
            if (_inputSystem.Player.ReachLeft.IsPressed() && !_punchingLeft)
            {

                if (!_reachLeftAxisUsed)
                {
                    //Adjust Left Arm joint strength
                    SetDrives(_reachStiffness, 5, 6);

                    //Adjust body joint strength
                    _playerParts[1].GetComponent<ConfigurableJoint>().angularXDrive = _coreStiffness;
                    _playerParts[1].GetComponent<ConfigurableJoint>().angularYZDrive = _coreStiffness;

                    _reachLeftAxisUsed = true;
                }

                if (_mouseYAxisArms <= 1.2f && _mouseYAxisArms >= -1.2f)
                {
                    _mouseYAxisArms = _mouseYAxisArms + (_inputSystem.Player.Look.ReadValue<Vector2>().y / _reachSensitivity);
                }

                else if (_mouseYAxisArms > 1.2f)
                {
                    _mouseYAxisArms = 1.2f;
                }

                else if (_mouseYAxisArms < -1.2f)
                {
                    _mouseYAxisArms = -1.2f;
                }

                //upper  left arm pose
                _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.58f - (_mouseYAxisArms), -0.88f - (_mouseYAxisArms), -0.8f, 1);
            }

            if (!_inputSystem.Player.ReachLeft.IsPressed() && !_punchingLeft)
            {
                if (_reachLeftAxisUsed)
                {
                    if (_balanced)
                    {
                        SetDrives(_poseOn, 5, 6);

                        _playerParts[1].GetComponent<ConfigurableJoint>().angularXDrive = _poseOn;
                        _playerParts[1].GetComponent<ConfigurableJoint>().angularYZDrive = _poseOn;
                    }

                    else if (!_balanced)
                    {
                        SetDrives(_driveOff, 5, 6);
                    }

                    _resetPose = true;
                    _reachLeftAxisUsed = false;
                }
            }




            //Reach Right
            if (_inputSystem.Player.ReachRight.IsPressed() && !_punchingRight)
            {

                if (!_reachRightAxisUsed)
                {
                    //Adjust Right Arm joint strength
                    SetDrives(_reachStiffness, 3, 4);

                    //Adjust body joint strength
                    _playerParts[1].GetComponent<ConfigurableJoint>().angularXDrive = _coreStiffness;
                    _playerParts[1].GetComponent<ConfigurableJoint>().angularYZDrive = _coreStiffness;

                    _reachRightAxisUsed = true;
                }

                if (_mouseYAxisArms is <= 1.2f and >= -1.2f)
                {
                    _mouseYAxisArms += (_inputSystem.Player.Look.ReadValue<Vector2>().y / _reachSensitivity);
                }

                else if (_mouseYAxisArms > 1.2f)
                {
                    _mouseYAxisArms = 1.2f;
                }

                else if (_mouseYAxisArms < -1.2f)
                {
                    _mouseYAxisArms = -1.2f;
                }

                //upper right arm pose
                _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0.58f + (_mouseYAxisArms), 0.88f + (_mouseYAxisArms), -0.8f, 1);
            }

            if (_inputSystem.Player.ReachRight.IsPressed() || _punchingRight || !_reachRightAxisUsed) return;
        
            if (_balanced)
            {
                SetDrives(_poseOn, 3, 4);

                _playerParts[1].GetComponent<ConfigurableJoint>().angularXDrive = _poseOn;
                _playerParts[1].GetComponent<ConfigurableJoint>().angularYZDrive = _poseOn;
            }

            else if (!_balanced)
            {
                SetDrives(_driveOff, 3, 4);
            }

            _resetPose = true;
            _reachRightAxisUsed = false;

        }

        private void PlayerPunch()
        {

            //punch right
            if (!_punchingRight && _isRightPunchButtonPressed && !_reachRightAxisUsed)
            {
                _punchingRight = true;

                //Right hand punch pull back pose
                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, -0.15f, 0, 1);
                _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.62f, 0.1f, 0.02f, 1);
                _playerParts[4].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(1.31f, -0.5f, 0.5f, 1);
            }

            if (_punchingRight && !_isRightPunchButtonPressed)
            {
                _punchingRight = false;

                //Right hand punch release pose
                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, 0.15f, 0, 1);
                _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0.74f, 0.04f, 0f, 1);
                _playerParts[4].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0.2f, 0, 0, 1);

                //Right hand punch force
                _rightHand.AddForce(_playerParts[0].transform.forward * _punchForce, ForceMode.Impulse);

                _playerParts[1].GetComponent<Rigidbody>().AddForce(_playerParts[0].transform.forward * _punchForce, ForceMode.Impulse);

                StartCoroutine(DelayCoroutine());
                IEnumerator DelayCoroutine()
                {
                    yield return new WaitForSeconds(0.3f);
                    if (!_isRightPunchButtonPressed)
                    {
                        _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation = _upperRightArmTarget;
                        _playerParts[4].GetComponent<ConfigurableJoint>().targetRotation = _lowerRightArmTarget;
                    }
                }
            }


            //punch left
            if (!_punchingLeft && _isLeftPunchButtonPressed && !_reachLeftAxisUsed)
            {
                _punchingLeft = true;

                //Left hand punch pull back pose
                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, 0.15f, 0, 1);
                _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0.62f, -0.1f, 0.02f, 1);
                _playerParts[6].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-1.31f, 0.5f, 0.5f, 1);
            }

            if (_punchingLeft && !_isLeftPunchButtonPressed)
            {
                _punchingLeft = false;

                //Left hand punch release pose
                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.15f, -0.15f, 0, 1);
                _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.74f, 0.04f, 0f, 1);
                _playerParts[6].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(-0.2f, 0, 0, 1);

                //Left hand punch force
                _leftHand.AddForce(_playerParts[0].transform.forward * _punchForce, ForceMode.Impulse);

                _playerParts[1].GetComponent<Rigidbody>().AddForce(_playerParts[0].transform.forward * _punchForce, ForceMode.Impulse);

                StartCoroutine(DelayCoroutine());
                IEnumerator DelayCoroutine()
                {
                    yield return new WaitForSeconds(0.3f);
                    if (!_isLeftPunchButtonPressed)
                    {
                        _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation = _upperLeftArmTarget;
                        _playerParts[6].GetComponent<ConfigurableJoint>().targetRotation = _lowerLeftArmTarget;
                    }
                }
            }
        }

        private void Walking()
        {
            if (!_inAir)
            {
                if (_walkForward)
                {
                    //right leg
                    if (_playerParts[11].transform.position.z < _playerParts[12].transform.position.z && !_stepLeft && !_alertLegRight)
                    {
                        _stepRight = true;
                        _alertLegRight = true;
                        _alertLegLeft = true;
                    }

                    //left leg
                    if (_playerParts[11].transform.position.z > _playerParts[12].transform.position.z && !_stepRight && !_alertLegLeft)
                    {
                        _stepLeft = true;
                        _alertLegLeft = true;
                        _alertLegRight = true;
                    }
                }

                if (_walkBackward)
                {
                    //right leg
                    if (_playerParts[11].transform.position.z > _playerParts[12].transform.position.z && !_stepLeft && !_alertLegRight)
                    {
                        _stepRight = true;
                        _alertLegRight = true;
                        _alertLegLeft = true;
                    }

                    //left leg
                    if (_playerParts[11].transform.position.z < _playerParts[12].transform.position.z && !_stepRight && !_alertLegLeft)
                    {
                        _stepLeft = true;
                        _alertLegLeft = true;
                        _alertLegRight = true;
                    }
                }

                //Step right
                if (_stepRight)
                {
                    _stepRightTimer += Time.fixedDeltaTime;

                    //Right foot force down
                    _playerParts[11].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);

                    //walk simulation
                    if (_walkForward)
                    {
                        _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.x - 0.09f * _stepHeight, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.w);
                        _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.x + 0.03f * _stepHeight * 2, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.w);

                        _playerParts[9].GetComponent<ConfigurableJoint>().GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.x + 0.12f * _stepHeight / 2, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.w);
                    }

                    if (_walkBackward)
                    {
                        _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.x - 0.07f * _stepHeight, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.w);
                        _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.x + 0.03f * _stepHeight * 2, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation.w);

                        _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.x + 0.15f * _stepHeight / 2, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.w);
                    }


                    //step duration
                    if (_stepRightTimer > _stepDuration)
                    {
                        _stepRightTimer = 0;
                        _stepRight = false;

                        if (_walkForward || _walkBackward)
                        {
                            _stepLeft = true;
                        }
                    }
                }
                else
                {
                    //reset to idle
                    _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(_playerParts[7].GetComponent<ConfigurableJoint>().targetRotation, _upperRightLegTarget, (8f) * Time.fixedDeltaTime);
                    _playerParts[8].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(_playerParts[8].GetComponent<ConfigurableJoint>().targetRotation, _lowerRightLegTarget, (17f) * Time.fixedDeltaTime);

                    //feet force down
                    _playerParts[11].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);
                    _playerParts[12].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);
                }


                //Step left
                if (_stepLeft)
                {
                    _stepLeftTimer += Time.fixedDeltaTime;

                    //Left foot force down
                    _playerParts[12].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);

                    //walk simulation
                    if (_walkForward)
                    {
                        _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.x - 0.09f * _stepHeight, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.w);
                        _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.x + 0.03f * _stepHeight * 2, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.w);

                        _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.x + 0.12f * _stepHeight / 2, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.w);
                    }

                    if (_walkBackward)
                    {
                        _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.x - 0.07f * _stepHeight, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation.w);
                        _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.x + 0.03f * _stepHeight * 2, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation.w);

                        _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(_playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.x + 0.15f * _stepHeight / 2, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.y, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.z, _playerParts[7].GetComponent<ConfigurableJoint>().targetRotation.w);
                    }


                    //Step duration
                    if (_stepLeftTimer > _stepDuration)
                    {
                        _stepLeftTimer = 0;
                        _stepLeft = false;

                        if (_walkForward || _walkBackward)
                        {
                            _stepRight = true;
                        }
                    }
                }
                else
                {
                    //reset to idle
                    _playerParts[9].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(_playerParts[9].GetComponent<ConfigurableJoint>().targetRotation, _upperLeftLegTarget, (7f) * Time.fixedDeltaTime);
                    _playerParts[10].GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(_playerParts[10].GetComponent<ConfigurableJoint>().targetRotation, _lowerLeftLegTarget, (18f) * Time.fixedDeltaTime);

                    //feet force down
                    _playerParts[11].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);
                    _playerParts[12].GetComponent<Rigidbody>().AddForce(-Vector3.up * _feetMountForce * Time.deltaTime, ForceMode.Impulse);
                }
            }
        }

        public void ActivateRagdoll()
        {
            _isRagdoll = true;
            _balanced = false;

            //Root
            _playerParts[0].GetComponent<ConfigurableJoint>().angularXDrive = _driveOff;
            _playerParts[0].GetComponent<ConfigurableJoint>().angularYZDrive = _driveOff;
            //head
            _playerParts[2].GetComponent<ConfigurableJoint>().angularXDrive = _driveOff;
            _playerParts[2].GetComponent<ConfigurableJoint>().angularYZDrive = _driveOff;
            //arms
            if (!_reachRightAxisUsed)
            {
                SetDrives(_driveOff, 3, 4);
            }

            if (!_reachLeftAxisUsed)
            {
                SetDrives(_driveOff, 5, 6);
            }
            //legs
            SetDrives(_driveOff, 7, 12);
        }

        public void DeactivateRagdoll()
        {
            _isRagdoll = false;
            _balanced = true;

            //Root
            _playerParts[0].GetComponent<ConfigurableJoint>().angularXDrive = _balanceOn;
            _playerParts[0].GetComponent<ConfigurableJoint>().angularYZDrive = _balanceOn;
            //head
            _playerParts[2].GetComponent<ConfigurableJoint>().angularXDrive = _poseOn;
            _playerParts[2].GetComponent<ConfigurableJoint>().angularYZDrive = _poseOn;
            //arms
            if (!_reachRightAxisUsed)
            {
                SetDrives(_poseOn, 3, 4);
            }

            if (!_reachLeftAxisUsed)
            {
                SetDrives(_poseOn, 5, 6);
            }
            //legs
            SetDrives(_poseOn, 7, 12);

            _resetPose = true;
        }

        private void ResetPlayerPose()
        {
            if (_resetPose && !_jumping)
            {
                _playerParts[1].GetComponent<ConfigurableJoint>().targetRotation = _bodyTarget;
                _playerParts[3].GetComponent<ConfigurableJoint>().targetRotation = _upperRightArmTarget;
                _playerParts[4].GetComponent<ConfigurableJoint>().targetRotation = _lowerRightArmTarget;
                _playerParts[5].GetComponent<ConfigurableJoint>().targetRotation = _upperLeftArmTarget;
                _playerParts[6].GetComponent<ConfigurableJoint>().targetRotation = _lowerLeftArmTarget;

                _mouseYAxisArms = 0;

                _resetPose = false;
            }
        }

        private void CenterOfMass()
        {
            var finalPosition = Vector3.zero;
            var finalMass = 0f;
            for (int i = 0; i <= 12; i++)
            {
                finalPosition += _playerParts[i].GetComponent<Rigidbody>().mass * _playerParts[i].transform.position;
                finalMass += _playerParts[i].GetComponent<Rigidbody>().mass;
            }
            _centerOfMassPoint = finalPosition / finalMass;

            _centerOfMass.position = _centerOfMassPoint;
        }

        public void SetCanRotate(bool value) => _canRotate = value;

        public GameObject Root => _root;
        public LayerMask ThisPlayerLayer() => _thisPlayerLayer;
        public float RequiredForceToBeKo() => _requiredForceToBeKo;
        public bool CanRotate() => _canRotate;
        public bool IsJumping() => _isJumping;
        public bool InAir() => _inAir;
        public bool PunchingLeft() => _punchingLeft;
        public bool PunchingRight() => _punchingRight;
        public bool ReachLeftAxisUsed() => _reachLeftAxisUsed;
        public bool ReachRightAxisUsed() => _reachRightAxisUsed;
        public bool UseControls() => _useControls;
        public bool CanBeKnockoutByImpact() => _canBeKnockoutByImpact;

        private void OnDrawGizmos()
        {
            if (!_editorDebugMode) return;
            
            Debug.DrawRay(_root.transform.position, -_root.transform.up * _balanceHeight, Color.green);

            if (_useStepPrediction)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_centerOfMass.position, 0.3f);
            }
        }
    }
}