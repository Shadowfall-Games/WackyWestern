using Grabbing;
using Player.ActiveRagdoll;
using UnityEngine;
using Zenject;

namespace Player.Hand
{
    public class HandHandler : MonoBehaviour
    {
        [SerializeField] private bool _isLeftHand;
        
        [SerializeField] private float _rotationSpeed = 0.6f;
        [SerializeField] private float _rotationLimit = 90;
        
        private HandContact _handContact;
        private HandRotation _handRotation;

        private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
        private ConfigurableJoint _configurableJoint;
        private Rigidbody _rigidbody;
        private CameraController _cameraController;
        private GrabbedObject _grabbedObject;
        private InputSystem _inputSystem;
        
        [Inject]
        private void Construct(InputSystem inputSystem) =>  _inputSystem = inputSystem;
        
        private void Start()
        {
            _activeRagdoll = FindAnyObjectByType<ActiveRagdoll.ActiveRagdoll>();
            _configurableJoint = GetComponent<ConfigurableJoint>();
            _rigidbody = GetComponent<Rigidbody>();
            
            if (Camera.main != null) _cameraController = Camera.main.GetComponent<CameraController>();
            
            _handContact = new HandContact(_activeRagdoll, _rigidbody, _isLeftHand, _inputSystem);
            _handRotation = new HandRotation(_activeRagdoll, _configurableJoint, _cameraController, _handContact, _isLeftHand, _rotationSpeed, _rotationLimit, _inputSystem);
            
            _handRotation.Start();
        }

        private void OnDestroy()
        {
            _handRotation.OnDestroy();
            _inputSystem.Disable();
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