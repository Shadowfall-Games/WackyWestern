using UnityEngine;
using HealthSystem;
using UnityEngine.VFX;

namespace Prop.ExplosiveBarrel
{
    public class ExplosiveBarrelView
    {
        private readonly float _vfxLifetime;
        private readonly float _barrelLifetime;
        private readonly VisualEffect _explosionEffect;
        
        public ExplosiveBarrelView(float vfxLifetime, float barrelLifetime, VisualEffect explosionEffect)
        {
            _vfxLifetime = vfxLifetime;
            _barrelLifetime = barrelLifetime;
            _explosionEffect = explosionEffect;
        }

        public void Explode(Vector3 explosionPosition, GameObject barrel)
        {
            var vfxInstance = Object.Instantiate(_explosionEffect, explosionPosition, Quaternion.identity);

            var vfx = vfxInstance.GetComponent<VisualEffect>();
            Object.Destroy(vfxInstance.gameObject, _vfxLifetime);
            
            Object.Destroy(barrel, _barrelLifetime);
        }
    }
}
