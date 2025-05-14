using UnityEngine;
using UnityEngine.VFX;

namespace Item.ExplosiveObject
{
    public class ExplosiveObjectView
    {
        private readonly float _vfxLifetime;
        private readonly float _objectLifetime;
        private readonly VisualEffect _explosionEffect;

        public ExplosiveObjectView(float vfxLifetime, float objectLifetime, VisualEffect explosionEffect)
        {
            _vfxLifetime = vfxLifetime;
            _objectLifetime = objectLifetime;
            _explosionEffect = explosionEffect;
        }

        public virtual void Explode(Vector3 explosionPosition, GameObject gameObject)
        {
            var vfxInstance = Object.Instantiate(_explosionEffect, explosionPosition, Quaternion.identity);
            
            Object.Destroy(vfxInstance.gameObject, _vfxLifetime);
            
            Object.Destroy(gameObject, _objectLifetime);
        }
    }
}