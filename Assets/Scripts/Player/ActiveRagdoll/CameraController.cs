using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Object To Follow")]
    [SerializeField] private Transform _objectToFollow;

    [Header("Follow Properties")]
    [SerializeField] private float _distance = 5;
    [SerializeField] private float _distanceOffset = 0.11f;
    [SerializeField] private float _smoothness = 0.07f;

    [Header("Rotation Properties")]
    [SerializeField] private float _rotateSpeed = 3;

    [SerializeField] private float _minAngle = -65;
    [SerializeField] private float _maxAngle = 20;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask _ignoreLayer;

    private Camera _camera;
    private float _currentX = 0.0f;
    private float _currentY = 0.0f;
    private Quaternion _rotation;
    private Vector3 _direction;

    private InputSystem _inputSystem;

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _camera = Camera.main;
    }


    private void Update()
    {
        _currentX = _currentX + _inputSystem.Player.Look.ReadValue<Vector2>().x * _rotateSpeed;
        _currentY = _currentY + _inputSystem.Player.Look.ReadValue<Vector2>().y * _rotateSpeed;

        _currentY = Mathf.Clamp(_currentY, _minAngle, _maxAngle);
    }


    private void FixedUpdate()
    {
        _direction = new Vector3(0, 0, -_distance);
        _rotation = Quaternion.Euler(-_currentY, _currentX, 0);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, _objectToFollow.position + _rotation * _direction, _smoothness);
        if (Physics.Linecast(_objectToFollow.position, _camera.transform.position, out RaycastHit hit, ~_ignoreLayer)) _camera.transform.position = hit.point + _camera.transform.forward * _distanceOffset;
        _camera.transform.LookAt(_objectToFollow.position);
    }

}