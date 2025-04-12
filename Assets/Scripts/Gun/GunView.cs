using System;
using Player.Contacts;
using UnityEngine;

namespace Gun
{
    public class GunView : GrabbedObject
    {
        [SerializeField] private float _jointForce = 600;
        [SerializeField] private float _jointDamping = 6;
        
        private Camera _camera;
        private HandContact _handContact;
        private ConfigurableJoint _configurableJoint;
        private Rigidbody _rigidBody;
        private Gun _gun;
        
        private void Start()
        {
            _camera = Camera.main;
            _configurableJoint = GetComponent<ConfigurableJoint>();
            _rigidBody = GetComponent<Rigidbody>();
            _gun = GetComponent<Gun>();
        }

        public override void Grab(HandContact handContact)
        {
            _handContact = handContact;
            _gun.enabled = true;
            _configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
            _configurableJoint.connectedBody = handContact.GetComponent<Rigidbody>();
            ConfigurateJoint(ConfigurableJointMotion.Locked);
        }
        
        public override void Drop()
        {
            _handContact = null;
            _gun.enabled = false;
            Destroy(_configurableJoint);
        }

        private void ConfigurateJoint(ConfigurableJointMotion motion)
        {
            _configurableJoint.xMotion = motion;
            _configurableJoint.yMotion = motion;
            _configurableJoint.zMotion = motion;
            _configurableJoint.rotationDriveMode = RotationDriveMode.Slerp;
            _configurableJoint.slerpDrive = NewJointDrive(_jointForce, _jointDamping);
        }
        
        private JointDrive NewJointDrive (float force, float damping)
        {
            var drive = new JointDrive();
            drive.mode = JointDriveMode.Position;
            drive.positionSpring = force;
            drive.positionDamper = damping;
            drive.maximumForce = Mathf.Infinity;
            return drive;
        }
    }
}