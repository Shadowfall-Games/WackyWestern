using System.Collections.Generic;
using HealthSystem;
using Item.ExplosiveObject.Dynamite;
using Item.ExplosiveObject.ExplosiveBarrel;
using Player.ActiveRagdoll;
using UnityEngine;

namespace Item.ExplosiveObject
{
    public class ExplosiveObjectExploder
    {
        private readonly int _explosionDamage;
        private readonly float _forceRadius;
        private readonly float _deadlyRadius;
        private readonly float _explosionForce;
        private readonly float _objectsExplosionForce;
        private readonly float _upwardsForce;

        public ExplosiveObjectExploder(int explosionDamage,float forceRadius, float deadlyRadius,float explodeForce, float objectsExplosionForce, float upwardsForce)
        {
            _explosionDamage = explosionDamage;
            _forceRadius = forceRadius;
            _deadlyRadius = deadlyRadius;
            _explosionForce = explodeForce;
            _objectsExplosionForce = objectsExplosionForce;
            _upwardsForce = upwardsForce;
        }

        public void Explode(Vector3 explosionPosition)
        {
            var hitColliders = Physics.OverlapSphere(explosionPosition, _forceRadius);

            var processedActiveRagdolls = new HashSet<ActiveRagdoll>();

            foreach (var hitCollider in hitColliders)
            {
                var distance = Vector3.Distance(explosionPosition, hitCollider.transform.position);
                
                var explosiveObject = hitCollider.GetComponentInParent<ExplosiveObject>();
                
                if (explosiveObject)
                {
                    if (distance <= _forceRadius)
                    {
                        explosiveObject?.Explode(explosiveObject.transform.position);
                    }
                }

                var activeRagdoll = hitCollider.GetComponentInParent<ActiveRagdoll>();

                if (activeRagdoll && processedActiveRagdolls.Add(activeRagdoll))
                {
                    if (distance <= _forceRadius)
                    {
                        var rootRigidbody = activeRagdoll.Root.GetComponent<Rigidbody>();
                        if (rootRigidbody != null)
                            rootRigidbody.AddExplosionForce(_explosionForce, explosionPosition, _forceRadius,
                                _upwardsForce, ForceMode.Impulse);
                    }

                    if (distance <= _deadlyRadius)
                    {
                        var healthComponent = activeRagdoll.Root.GetComponent<Health>();
                        if (healthComponent != null)
                            healthComponent.ApplyDamage(_explosionDamage, explosionPosition);
                    }

                    activeRagdoll.ActivateRagdoll();
                }
                else
                {
                    var rigidBody = hitCollider.GetComponentInParent<Rigidbody>();
                    var configurableJoint = hitCollider.GetComponentInParent<ConfigurableJoint>();

                    if (rigidBody && !configurableJoint && !explosiveObject)
                            if (distance <= _forceRadius)
                                rigidBody.AddExplosionForce(_objectsExplosionForce, explosionPosition, _forceRadius,
                                    _upwardsForce, ForceMode.Impulse);
                }
            }
        }
    }
}