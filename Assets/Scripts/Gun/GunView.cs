using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace Gun
{
    public class GunView
    {
        private readonly float _vfxDuration;
        private readonly VisualEffect _impactExplosion;
        
        private readonly Gun _gun;
        
        public GunView(Gun gun, float vfxDuration, VisualEffect impactExplosion)
        {
            _gun = gun;
            _vfxDuration = vfxDuration;
            _impactExplosion = impactExplosion;

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