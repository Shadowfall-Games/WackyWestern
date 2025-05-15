using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Player.ActiveRagdoll
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private bool _canRotate = true;
        [Space(5)]
        [Header("Object To Follow")]
        [SerializeField] private Transform _objectToFollow;

        [Header("Player Model")]
        [SerializeField] private SkinnedMeshRenderer _playerModel;

        [Header("Follow Properties")]
        [SerializeField] private float _distance = 8;
        [SerializeField] private float _distanceOffset = 0.1f;
        [SerializeField] private float _distanceToFade = 5;
        [SerializeField] private float _smoothness = 0.07f;

        [Header("Sensitivity")]
        [SerializeField] private float _sensitivityX = 2;
        [SerializeField] private float _sensitivityY = 2;
        [SerializeField] private float _sensitivityMultiplayer = 0.1f;

        [Header("Rotation Angles")]
        [SerializeField] private float _minAngle = -60;
        [SerializeField] private float _maxAngle = 20;

        [Header("Camera Collision")]
        [SerializeField] private LayerMask _ignoreLayer;

        private Camera _camera;
        private float _currentX;
        private float _currentY;
        private Quaternion _rotation;
        private Vector3 _direction;

        private InputSystem _inputSystem;

        [Inject]
        private void Construct(InputSystem inputSystem) =>  _inputSystem = inputSystem;

        private void OnDestroy()
        {
            _inputSystem.Player.Disable();
        }

        private void Start()
        {
           Cursor.lockState = CursorLockMode.Locked;

            _camera = Camera.main;
        }


        private void Update()
        {
            if (!_canRotate) return;
            
            _currentX += _inputSystem.Player.Look.ReadValue<Vector2>().x * _sensitivityX * _sensitivityMultiplayer;
            _currentY += _inputSystem.Player.Look.ReadValue<Vector2>().y * _sensitivityY * _sensitivityMultiplayer;

            _currentY = Mathf.Clamp(_currentY, _minAngle, _maxAngle);
        }


        private void FixedUpdate()
        {
            _direction = new Vector3(0, 0, -_distance);
            if (_canRotate) _rotation = Quaternion.Euler(-_currentY, _currentX, 0);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _objectToFollow.position + _rotation * _direction, _smoothness);
            if (Physics.Linecast(_objectToFollow.position, _camera.transform.position, out RaycastHit hit, ~_ignoreLayer))
            {
                var currentPosition = hit.point + _camera.transform.forward * _distanceOffset;
                _camera.transform.position = currentPosition;

                var color = _playerModel.material.color;
                color.a = (Vector3.Distance(currentPosition, _objectToFollow.position) / _distanceToFade);
                _playerModel.material.color = color;
            }
            else
            {
                var color = _playerModel.material.color;
                color.a = 1;
                _playerModel.material.color = color;
            }
            _camera.transform.LookAt(_objectToFollow.position);
        }
        
        public void SetCanRotate(bool value) => _canRotate = value;

        public bool CanRotate() => _canRotate;

    }
}