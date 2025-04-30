using Player.ActiveRagdoll;
using UnityEngine;

namespace Player.Hand
{
    public class HandHandler : MonoBehaviour
    {
        [SerializeField] private bool _isLeftHand;
        
        [SerializeField] private HandContact _handContact;
        [SerializeField] private HandRotation _handRotation;
        
        private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
        private ConfigurableJoint _configurableJoint;
        private Rigidbody _rigidbody;
        private CameraController _cameraController;
        
        private void Start()
        {
            _activeRagdoll = FindAnyObjectByType<ActiveRagdoll.ActiveRagdoll>();
            _configurableJoint = GetComponent<ConfigurableJoint>();
            _rigidbody = GetComponent<Rigidbody>();
            
            if (Camera.main != null) _cameraController = Camera.main.GetComponent<CameraController>();
            
            _handContact.Init(_activeRagdoll, _rigidbody, _isLeftHand);
            _handRotation.Init(_activeRagdoll, _configurableJoint, _cameraController, _isLeftHand);

            _handContact.ObjectPickedUp += () => _handRotation.SetHaveObjectInHand(true);
            _handContact.ObjectDroppedOut += () => _handRotation.SetHaveObjectInHand(true);
            _handContact.ObjectDroppedOut += _handRotation.ResetRotation;
        }

        private void OnEnable() => _handContact.OnEnable();

        private void OnDisable()
        {
            _handContact.OnDisable();
            _handRotation.OnDisable();
        }
        
        private void OnDestroy()
        {
            _handContact.ObjectPickedUp -= () => _handRotation.SetHaveObjectInHand(true);
            _handContact.ObjectDroppedOut -= () => _handRotation.SetHaveObjectInHand(true);
            _handContact.ObjectDroppedOut -= _handRotation.ResetRotation;
        }

        private void Update()
        {
            _handContact.Update();
            _handRotation.Update();
        }

        private void OnCollisionEnter(Collision collision)
        {
            _handContact.OnCollisionEnter(collision, gameObject);
        }
    }
}