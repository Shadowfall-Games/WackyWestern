using System;
using Grabbing;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player.Hand
{
    public class HandContact
    { 
        private bool _hasJoint;
        private readonly bool _isLeftHand;
        
        private readonly InputSystem _inputSystem;
        private GrabbedObject _grabbedObject;
        private FixedJoint _fixedJoint;
        private readonly ActiveRagdoll.ActiveRagdoll _activeRagdoll;
        private readonly Rigidbody _rigidbody;
        
        public event Action ObjectPickedUp;
        public event Action ObjectDroppedOut;

        public HandContact(ActiveRagdoll.ActiveRagdoll activeRagdoll, Rigidbody rigidbody, bool isLeftHand, InputSystem inputSystem)
        {
            _activeRagdoll = activeRagdoll;
            _rigidbody = rigidbody;
            _isLeftHand = isLeftHand;
            _inputSystem = inputSystem;
        }
        
        public void Update()
        {
            if (!_activeRagdoll.UseControls()) return;
            
            if (_isLeftHand)
            {
                if (_hasJoint && !_inputSystem.Player.ReachLeft.IsPressed())
                {
                    DropItem(_inputSystem.Player.ReachLeft.IsPressed());
                }
            }
            else
            {
                if (_hasJoint && !_inputSystem.Player.ReachRight.IsPressed())
                {
                    DropItem(_inputSystem.Player.ReachRight.IsPressed());
                }
            }
        }

        public void OnCollisionEnter(Collision collision, GameObject gameObject)
        {
            if (!_activeRagdoll.UseControls()) return;
            
            if (_isLeftHand) GrabItem(collision, gameObject, _activeRagdoll.ReachLeftAxisUsed(), _activeRagdoll.PunchingLeft());

            if (!_isLeftHand) GrabItem(collision, gameObject, _activeRagdoll.ReachRightAxisUsed(), _activeRagdoll.PunchingRight());
        }

        private void GrabItem(Collision collision, GameObject gameObject, bool reachIsPressed, bool punchIsPressed)
        {
            var hasComponent = collision.gameObject.TryGetComponent(out GrabbedObject grabbedObject);
            
            if (collision.gameObject.layer != ~_activeRagdoll.ThisPlayerLayer() && !_hasJoint && reachIsPressed && !punchIsPressed)
            {
                _hasJoint = true;
                if (hasComponent)
                {
                    _grabbedObject = grabbedObject;
                    _grabbedObject.Grab(this, _isLeftHand);

                    ObjectPickedUp?.Invoke();
                }
                else
                {
                    _fixedJoint = gameObject.AddComponent<FixedJoint>();
                    _fixedJoint.breakForce = Mathf.Infinity;
                    _fixedJoint.connectedBody = collision.gameObject.GetComponent<Rigidbody>();
                }
            }
        }
        
        private void DropItem(bool reachIsPressed)
        {
            if (!reachIsPressed)
            {
                _hasJoint = false;
                
                if (_grabbedObject)
                {
                    _grabbedObject.Drop();
                    _grabbedObject = null;
                    
                    ObjectDroppedOut?.Invoke(); 
                }
                else
                {
                    Object.Destroy(_fixedJoint);
                    _fixedJoint = null;
                }
            }
        }
        
        public GrabbedObject GrabbedObject() => _grabbedObject;
        public Rigidbody GetRigidBody() => _rigidbody;

        public bool HasJoint() => _hasJoint;
    }
}