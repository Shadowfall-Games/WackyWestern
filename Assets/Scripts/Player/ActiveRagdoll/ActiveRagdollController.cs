using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class ActiveRagdollController : MonoBehaviour
    {
        #region AccessableFields 
        [Header("MovementValues")]
        [SerializeField] private float _speed;
        [SerializeField] private float _strafeSpeed;
        [SerializeField] private float _backwardsSpeed;
        [SerializeField] private float _sprintSpeed;

        [Header("Jump")]
        [SerializeField] private bool _isGrounded;
        [SerializeField] private LayerMask _ground;

        [SerializeField] private float _jumpForce;

        [Header("Torso")]
        [SerializeField] private Rigidbody _root;
        #endregion

        #region PrivateFields
        private Vector2 _move;
        private InputSystem _inputSystem;
        #endregion

        private void OnEnable()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Player.Jump.performed += Jump;
            _inputSystem.Player.Enable();
        }

        private void Update()
        {
            _move = _inputSystem.Player.Movement.ReadValue<Vector2>();

            _isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, _ground);
        }

        private void FixedUpdate() => Move();

        private void Move()
        {
            Vector3 moveDirection = new Vector3(_move.x, 0, _move.y);
            moveDirection = Camera.main.transform.TransformDirection(moveDirection);
            moveDirection.y = 0; 

            float currentSpeed = _speed;

            if (_move.y < 0)
            {
                currentSpeed = _backwardsSpeed;
            }
            if (_move.x != 0)
            {
                currentSpeed = _strafeSpeed;
            }

            _root.AddForce(moveDirection.normalized * currentSpeed, ForceMode.Acceleration);
        }

        private void Jump(InputAction.CallbackContext _)
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _root.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        public bool IsGrounded { get => _isGrounded; }
        public Vector2 Movement { get => _move; }

        private void OnDisable()
        {
            _inputSystem.Player.Jump.performed -= Jump;
            _inputSystem.Player.Disable();
        }
    }
}