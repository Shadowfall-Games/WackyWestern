using UnityEngine;

namespace Player.Punching
{
    [RequireComponent(typeof(ActiveRagdoll.ActiveRagdoll))]
    public class ImpactContact : MonoBehaviour
    {
        private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
        
        private void Start() => _activeRagdoll = GetComponent<ActiveRagdoll.ActiveRagdoll>();

        private void OnCollisionEnter(Collision col)
        {
            if (_activeRagdoll.CanBeKnockoutByImpact() && col.relativeVelocity.magnitude > _activeRagdoll.RequiredForceToBeKo()) _activeRagdoll.ActivateRagdoll();
        }
    }
}