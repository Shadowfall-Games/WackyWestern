using System;
using Player.Hand;
using UnityEngine;

namespace Grabbing
{
    public abstract class GrabbedObject : MonoBehaviour
    {
        [Header("JointSettings")]
        [SerializeField] private float _jointForce = 1000;
        [SerializeField] private float _jointDamping = 50;

        private bool _isObjectInTwoHands;
        
        private HandContact _leftHandContact, _rightHandContact;
        private ConfigurableJoint _leftHandConfigurableJoint, _rightHandConfigurableJoint;
        
        public event Action OnGrab;
        public event Action OnDrop;
        
        public virtual void Grab(HandContact handContact, bool isLeftHand)
        {
            if (isLeftHand) _leftHandContact = handContact;
            else _rightHandContact = handContact;

            if (isLeftHand)
                ConfigurateJoint(out _leftHandConfigurableJoint, ConfigurableJointMotion.Locked, _leftHandContact);
            else
                ConfigurateJoint(out _rightHandConfigurableJoint, ConfigurableJointMotion.Locked, _rightHandContact);

            OnGrab?.Invoke();
        }

        public virtual void Drop()
        {
            if (_leftHandContact != null && !_leftHandContact.HasJoint()) { _leftHandContact = null; Destroy(_leftHandConfigurableJoint); }
            if (_rightHandContact != null && !_rightHandContact.HasJoint()) { _rightHandContact = null; Destroy(_rightHandConfigurableJoint); }
            
            OnDrop?.Invoke();
        }
        
        private void ConfigurateJoint(out ConfigurableJoint configurableJoint, ConfigurableJointMotion motion, HandContact handContact)
        {
            configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
            configurableJoint.connectedBody = handContact.GetRigidBody();
            configurableJoint.xMotion = motion;
            configurableJoint.yMotion = motion;
            configurableJoint.zMotion = motion;
            configurableJoint.rotationDriveMode = RotationDriveMode.Slerp;
            configurableJoint.slerpDrive = GetJointDrive(_jointForce, _jointDamping);
        }
        
        private JointDrive GetJointDrive (float force, float damping)
        {
            var drive = new JointDrive
            {
                mode = JointDriveMode.Position,
                positionSpring = force,
                positionDamper = damping,
                maximumForce = Mathf.Infinity
            };
            return drive;
        }
        
        public bool IsObjectInTwoHands() => _leftHandContact != null && _rightHandContact != null;
    }
}