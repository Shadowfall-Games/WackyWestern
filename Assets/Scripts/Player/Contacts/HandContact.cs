using System;
using UnityEngine;

namespace Player.Contacts
{
    public class HandContact : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll.ActiveRagdoll _player;

        [SerializeField] private bool Left;
        [SerializeField] private bool hasJoint;

        private InputSystem _inputSystem;
        private GrabbedObject _grabbedObject;
        private FixedJoint _fixedJoint;
        
        public event Action<Transform> GunPickedUp;
        public event Action<Transform> GunDroppedOut;

        private void OnEnable()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Player.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.Player.Disable();
        }
        
        private void Update()
        {
            if (_player.UseConrols())
            {
                if (Left)
                {
                    if (hasJoint && !_inputSystem.Player.ReachLeft.IsPressed())
                    {
                        hasJoint = false;
                        DropItem(_inputSystem.Player.ReachLeft.IsPressed());
                    }
                }

                if (!Left)
                {
                    if (hasJoint && !_inputSystem.Player.ReachRight.IsPressed())
                    {
                        hasJoint = false;
                        DropItem(_inputSystem.Player.ReachRight.IsPressed());
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collider)
        {
            if (!_player.UseConrols()) return;
            
            if (Left) GrabItem(collider, _inputSystem.Player.ReachLeft.IsPressed(), _player.PunchingLeft());

            if (!Left) GrabItem(collider, _inputSystem.Player.ReachRight.IsPressed(), _player.PunchingRight());
        }

        private void GrabItem(Collision collider, bool reachIsPressed, bool punchIsPressed)
        {
            if (collider.gameObject.TryGetComponent(out GrabbedObject grabbedObject) && collider.gameObject.layer != _player.ThisPlayerLayer() && !hasJoint)
            {
                if (reachIsPressed && !hasJoint && !punchIsPressed)
                {
                    hasJoint = true;

                    _grabbedObject = grabbedObject;
                    _grabbedObject.Grab(this);
                    if (grabbedObject.gameObject.TryGetComponent(out Gun.Gun gun))
                    {
                        _grabbedObject.gameObject.layer = LayerMask.NameToLayer("Arms");
                        //GunPickedUp?.Invoke(grabbedObject.transform);
                    }
                    else
                    {
                        _fixedJoint = gameObject.AddComponent<FixedJoint>();
                        _fixedJoint.breakForce = Mathf.Infinity;
                        _fixedJoint.connectedBody = grabbedObject.GetComponent<Rigidbody>();
                    }
                }
            }
        }
        
        private void DropItem(bool reachIsPressed)
        {
            if (_grabbedObject&& reachIsPressed)
            {
                hasJoint = false;

                _grabbedObject.gameObject.layer = LayerMask.NameToLayer("Default");
                if (_grabbedObject.gameObject.TryGetComponent(out Gun.Gun gun))
                {
                    _grabbedObject.gameObject.layer = LayerMask.NameToLayer("Arms");
                    _grabbedObject.Drop();
                    //GunDroppedOut?.Invoke(grabbedObject.transform);
                }
                else
                {
                    _fixedJoint.breakForce = 0;
                }
            }
        }
    }
}