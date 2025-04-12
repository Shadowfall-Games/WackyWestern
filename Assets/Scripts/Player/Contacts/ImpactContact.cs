using UnityEngine;

namespace Player.Contacts
{
    public class ImpactContact : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll.ActiveRagdoll _activeRagdoll;

        private void OnCollisionEnter(Collision col)
        {
            if (_activeRagdoll.CanBeKnockoutByImpact() && col.relativeVelocity.magnitude > _activeRagdoll.RequiredForceToBeKO()) _activeRagdoll.ActivateRagdoll();
        }
    }
}