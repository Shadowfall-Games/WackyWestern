using UnityEngine;

public class ImpactContact : MonoBehaviour
{
    [SerializeField] private ActiveRagdoll _activeRagdoll;

    private void OnCollisionEnter(Collision col)
    {
        if (_activeRagdoll.CanBeKnockoutByImpact() && col.relativeVelocity.magnitude > _activeRagdoll.RequiredForceToBeKO()) _activeRagdoll.ActivateRagdoll();
    }
}