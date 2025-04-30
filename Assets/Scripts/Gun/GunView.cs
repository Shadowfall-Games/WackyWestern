using System;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace Gun
{
    [Serializable]
    public class GunView
    {
        [SerializeField] private float _vfxDuration = 0.5f;
        [SerializeField] private VisualEffect _impactExplosion;
        
        private Gun _gun;
        
        public void Init(Gun gun)
        {
            _gun = gun;

            _gun.OnHit += SpawnVFX;
        }

        public void OnDestroy()
        {
            _gun.OnHit -= SpawnVFX;
        }
        private void SpawnVFX(Vector3 position)
        {
            var vfxInstance = Object.Instantiate(_impactExplosion, position, Quaternion.identity);

            VisualEffect vfx = vfxInstance.GetComponent<VisualEffect>();
            Object.Destroy(vfxInstance.gameObject, _vfxDuration);
        }
    }
}