using UnityEngine;

namespace Player.ActiveRagdoll
{
    public class FeetContact : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll _ragdollController;

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