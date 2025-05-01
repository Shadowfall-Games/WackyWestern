using Player.ActiveRagdoll;
using UnityEngine;

namespace Player.Hand
{
    public class HandRotation
    {
        private readonly float _rotationSpeed;

        private readonly bool _isLeftHand;
        private bool _haveObjectInHand;
        
        private Vector2 _mouseDelta;
        private readonly Quaternion _defaultRotation;
        
        private readonly InputSystem _inputSystem;
        private readonly CameraController _cameraController;
        private readonly ConfigurableJoint _configurableJoint;
        private readonly ActiveRagdoll.ActiveRagdoll _activeRagdoll;
        private readonly HandContact _handContact;

        public HandRotation(ActiveRagdoll.ActiveRagdoll activeRagdoll, ConfigurableJoint configurableJoint, CameraController cameraController, HandContact handContact, bool isLeftHand, float rotationSpeed, float rotationLimit, InputSystem inputSystem)
        {
            _activeRagdoll = activeRagdoll;
            _configurableJoint = configurableJoint;
            _cameraController = cameraController;
            _handContact = handContact;
            _isLeftHand = isLeftHand;
            _rotationSpeed = rotationSpeed;
            _inputSystem = inputSystem;

            _defaultRotation = _configurableJoint.targetRotation;

            _configurableJoint.angularYLimit = new SoftJointLimit { limit = rotationLimit };
            _configurableJoint.angularZLimit = new SoftJointLimit { limit = rotationLimit };
        }

        public void Start()
        {
            _handContact.ObjectPickedUp += ObjectPickedUp;
            _handContact.ObjectDroppedOut += ResetRotation;
        }

        public void OnDestroy()
        {
            _handContact.ObjectPickedUp -= ObjectPickedUp;
            _handContact.ObjectDroppedOut -= ResetRotation;
        }
        
        public void Update()
        {
            _mouseDelta = _inputSystem.Player.Look.ReadValue<Vector2>();
            
            if (_haveObjectInHand && !_handContact.GrabbedObject().IsObjectInTwoHands()) Rotate();
        }

        private void Rotate()
        {
            if (_isLeftHand ? _inputSystem.Player.ReachLeft.IsPressed() && _inputSystem.Player.PunchLeft.IsPressed()
                    : _inputSystem.Player.ReachRight.IsPressed() && _inputSystem.Player.PunchRight.IsPressed())
            {
                var deltaRotation = Quaternion.Euler(-(_mouseDelta.y * _rotationSpeed), 0, -(_mouseDelta.x * _rotationSpeed));
                
                if (_mouseDelta.magnitude > 0.01f)
                {
                    _configurableJoint.targetRotation = deltaRotation * _configurableJoint.targetRotation;
                    
                    if (_cameraController.CanRotate() && _activeRagdoll.CanRotate())
                    {
                        _cameraController.SetCanRotate(false);
                        _activeRagdoll.SetCanRotate(false);
                    }
                }
            }
            else if (!_cameraController.CanRotate() && !_activeRagdoll.CanRotate())
            {
                _activeRagdoll.SetCanRotate(true);
                _cameraController.SetCanRotate(true);
            }
        }
        
        private void ObjectPickedUp() => SetHaveObjectInHand(true);
        private void SetHaveObjectInHand(bool value) => _haveObjectInHand = value;
        private void ResetRotation()
        {
            _configurableJoint.targetRotation = _defaultRotation;
            SetHaveObjectInHand(false);
        }
    }
}
