using System;
using Player.ActiveRagdoll;
using UnityEngine;

namespace Player.Hand
{
    [Serializable]
    public class HandRotation
    {
        [SerializeField] private float _rotationSpeed = 0.6f;
        [SerializeField] private float _rotationLimit = 90;
        
        private bool _isLeftHand;
        private bool _haveObjectInHand;
        
        private Vector2 _mouseDelta;
        private Quaternion _defaultRotation;
        
        private InputSystem _inputSystem;
        private CameraController _cameraController;
        private ConfigurableJoint _configurableJoint;
        private ActiveRagdoll.ActiveRagdoll _activeRagdoll;

        public void Init(ActiveRagdoll.ActiveRagdoll activeRagdoll, ConfigurableJoint configurableJoint, CameraController cameraController, bool isLeftHand)
        {
            _activeRagdoll = activeRagdoll;
            _configurableJoint = configurableJoint;
            _cameraController = cameraController;
            _isLeftHand = isLeftHand;
            
            _defaultRotation = _configurableJoint.targetRotation;

            _configurableJoint.angularYLimit = new SoftJointLimit { limit = _rotationLimit };
            _configurableJoint.angularZLimit = new SoftJointLimit { limit = _rotationLimit };
            
            _inputSystem = new InputSystem();
            _inputSystem.Enable();
        }

        public void OnDisable()
        {
            _inputSystem.Disable();
        }
        
        public void Update()
        {
            _mouseDelta = _inputSystem.Player.Look.ReadValue<Vector2>();
            
            if (_haveObjectInHand) Rotate();
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
        
        public void ResetRotation() => _configurableJoint.targetRotation = _defaultRotation;
        
        public void SetHaveObjectInHand(bool value) => _haveObjectInHand = value;
    }
}
