using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    [SerializeField] private Transform _targetLimb;
    [SerializeField] private bool _mirror;

    private Quaternion _startingRotation;

    private ConfigurableJoint _configurableJoint;

    private void Start()
    {
        _configurableJoint = GetComponent<ConfigurableJoint>();

        _startingRotation = Quaternion.Inverse(transform.localRotation);
    }

    private void Update()
    {
        if (!_mirror) _configurableJoint.targetRotation = _targetLimb.localRotation * _startingRotation;
        else _configurableJoint.targetRotation = Quaternion.Inverse(_targetLimb.localRotation * _startingRotation);
    }
}