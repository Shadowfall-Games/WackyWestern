using System;
using Player.ActiveRagdoll;
using UnityEngine;

namespace Gun
{
    public class ChangeHandlePosition : MonoBehaviour
    {
        [SerializeField] private Transform _handlePoint;

        [SerializeField] private bool _isInsideCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out HandContact handContact))
            {
                _isInsideCollider = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out HandContact handContact))
            {
                _isInsideCollider = false;
            }
        }
    }
}
