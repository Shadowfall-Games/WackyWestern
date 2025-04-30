using UnityEngine;

namespace Player.Contacts
{
    public class FeetContact : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll.ActiveRagdoll _ragdollController;

        private void OnCollisionEnter(Collision col)
        {
            if (_ragdollController.IsJumping() || !_ragdollController.InAir()) return;
            
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _ragdollController.PlayerLanded();
            }
        }
    }
}