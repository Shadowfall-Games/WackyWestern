using Player.Hand;
using UnityEngine;

namespace Grabbing
{
    public abstract class GrabbedObject : MonoBehaviour
    {
        [Header("JointSettings")]
        [SerializeField] private float _jointForce = 1000;
        [SerializeField] private float _jointDamping = 10;
        
        private HandContact _leftHandContact, _rightHandContact;
        private ConfigurableJoint _leftHandConfigurableJoint, _rightHandConfigurableJoint;
        
        public virtual void Grab(HandContact handContact, bool isLeftHand)
        {
            if (isLeftHand) _leftHandContact = handContact;
            else _rightHandContact = handContact; 
            
            if (isLeftHand)
            {
                _leftHandConfigurableJoint = gameObject.AddComponent<ConfigurableJoint>();
                _leftHandConfigurableJoint.connectedBody = _leftHandContact.GetRigidBody();
                ConfigurateJoint(_leftHandConfigurableJoint, ConfigurableJointMotion.Locked);
            }
            else
            {
                _rightHandConfigurableJoint = gameObject.AddComponent<ConfigurableJoint>();
                _rightHandConfigurableJoint.connectedBody = _rightHandContact.GetRigidBody();
                ConfigurateJoint(_rightHandConfigurableJoint, ConfigurableJointMotion.Locked);
            }
        }

        public virtual void Drop()
        {
            if (_leftHandContact != null && !_leftHandContact.HasJoint()) { _leftHandContact = null; Destroy(_leftHandConfigurableJoint); }
            if (_rightHandContact != null && !_rightHandContact.HasJoint()) { _rightHandContact = null; Destroy(_rightHandConfigurableJoint); }
        }
        
        private void ConfigurateJoint(ConfigurableJoint configurableJoint, ConfigurableJointMotion motion)
        {
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
    }
}