using Player;
using UnityEngine;

public class ActiveRagdollSettings : MonoBehaviour
{
    #region AccessableFields
    [SerializeField] private ActiveRagdollController _activeRagdollController;
    [Space(5)]

    [Header("DefaultActiveRagdollPhysics")]
    [SerializeField] private int _defaultSolverIterations = 30;
    [SerializeField] private int _defaultVelSolverIterations = 15;
    [SerializeField] private float _defaultMaxAngularVelocity = 10;

    [Header("BackwardsWalkingActiveRagdollPhysics")]
    [SerializeField] private int _backwardsWalkingSolverIterations = 12;
    [SerializeField] private int _backwardsWalkingVelSolverIterations = 8;
    [SerializeField] private float _backwardsWalkingMaxAngularVelocity = 0;

    [Header("Torso")]
    [SerializeField] private Rigidbody _physicalTorso;

    public Rigidbody PhysicalTorso { get { return _physicalTorso; } }
    public Rigidbody[] Rigidbodies { get; private set; }
    #endregion

    #region PrivateFields
    private InputSystem _inputSystem;

    private bool _isDefaultIterations;
    #endregion

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
    }

    private void Awake()
    {
        _isDefaultIterations = true;

        if (Rigidbodies == null) Rigidbodies = _physicalTorso?.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in Rigidbodies)
        {
            rb.solverIterations = _defaultSolverIterations;
            rb.solverVelocityIterations = _defaultVelSolverIterations;
            rb.maxAngularVelocity = _defaultMaxAngularVelocity; ;
        }
    }

    private void Update()
    {
        if (_activeRagdollController.Movement.y == -1 && _isDefaultIterations)
        {
            _isDefaultIterations = false;
            foreach (Rigidbody rb in Rigidbodies)
            {
                rb.solverIterations = _backwardsWalkingSolverIterations;
                rb.solverVelocityIterations = _backwardsWalkingVelSolverIterations;
                rb.maxAngularVelocity = _backwardsWalkingMaxAngularVelocity;
            }
        }
        else
        {
            _isDefaultIterations = true;
            foreach (Rigidbody rb in Rigidbodies)
            {
                rb.solverIterations = _defaultSolverIterations;
                rb.solverVelocityIterations = _defaultVelSolverIterations;
                rb.maxAngularVelocity = _defaultMaxAngularVelocity; ;
            }
        }
    }

    private void OnDisable() => _inputSystem.Player.Disable();
}