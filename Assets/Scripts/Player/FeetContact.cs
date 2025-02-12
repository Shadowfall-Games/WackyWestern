using UnityEngine;

public class FeetContact : MonoBehaviour
{
    public ActiveRagdoll _ragdollController;

    //Alert APR player when feet colliders enter ground object layer
    private void OnCollisionEnter(Collision col)
    {
        if (!_ragdollController._isJumping && _ragdollController._inAir)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _ragdollController.PlayerLanded();
            }
        }
    }
}