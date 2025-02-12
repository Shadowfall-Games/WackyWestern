using UnityEngine;

public class AnimationController : MonoBehaviour
{
    /* #region AccessableFields
    [Header("TransitionSpeed")]
    [SerializeField] private float _towardsTransitionSpeed = 0.2f;
    [SerializeField] private float _backwardsTransitionSpeed = 0.1f;
    [SerializeField] private float _stopTransitionSpeed = 0.1f;

    [Header("Camera")]
    [SerializeField] private CinemachineFreeLook _camera;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("ActiveRagdoll")]
    [SerializeField] private ActiveRagdollController _activeRagdollController;
    #endregion

    #region PrivateFields
    private InputSystem _inputSystem;

    private float _mouseXSmooth;
    #endregion

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
    }

    private void Update()
    {
        if (_inputSystem.Player.S.IsPressed())
        {
            if (_inputSystem.Player.Sprint.IsPressed()) _animator.SetFloat("yAxis", -2, _backwardsTransitionSpeed, Time.deltaTime);
            else _animator.SetFloat("yAxis", -1, _backwardsTransitionSpeed, Time.deltaTime);
        }
        else if (_inputSystem.Player.W.IsPressed() || _inputSystem.Player.A.IsPressed() || _inputSystem.Player.D.IsPressed())
        {
            if (_inputSystem.Player.Sprint.IsPressed()) _animator.SetFloat("yAxis", 2, _towardsTransitionSpeed, Time.deltaTime);
            else _animator.SetFloat("yAxis", 1, _towardsTransitionSpeed, Time.deltaTime);
        }
        else if (_animator.GetFloat("yAxis") != 0)
        {
            _animator.SetFloat("yAxis", 0, _stopTransitionSpeed, Time.deltaTime);
        }

        if (_camera.m_XAxis.m_InputAxisValue != 0 && !_inputSystem.Player.Movement.IsInProgress())
        {
            _animator.SetBool("isTurning", true);

            _mouseXSmooth = Mathf.Lerp(_mouseXSmooth, _inputSystem.Player.Look.ReadValue<Vector2>().x, 4 * Time.deltaTime);
            _animator.SetFloat("Turn", _mouseXSmooth);
        }
        else if (_animator.GetBool("isTurning")) _animator.SetBool("isTurning", false);
    }

    private void OnDisable() => _inputSystem.Player.Disable(); */
}