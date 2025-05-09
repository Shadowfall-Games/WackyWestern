using HealthSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Prop.ExplosiveBarrel
{
    public class ExplosiveBarrelHandler : MonoBehaviour
    {
        [Header("Explosion settings")]
        [SerializeField] private int _explosionDamage = 100;
        [SerializeField] private float _forceRadius = 8;
        [SerializeField] private float _deadlyRadius = 4;
        [SerializeField] private float _explosionForce = 800;
        [SerializeField] private float _objectsExplosionForce = 100;
        [SerializeField] private float _upwardsForce = 2;
        
        [Header("Explosion view")]
        [SerializeField] private float _vfxLifetime = 5.5f;
        [SerializeField] private float _barrelLifetime = 1;
        [SerializeField] private VisualEffect _explosionEffect;
        
        private bool _isExploded;
        
        private ExplosiveBarrelExploder _explosiveBarrelExploder;
        private ExplosiveBarrelView _explosiveBarrelView;
        
        private IHealthSystem _barrelHealth;

        private void Start()
        {
            _barrelHealth = GetComponent<IHealthSystem>();
            
            _explosiveBarrelExploder = new ExplosiveBarrelExploder(_explosionDamage, _forceRadius, _deadlyRadius, _explosionForce, _objectsExplosionForce, _upwardsForce);
            _explosiveBarrelView = new ExplosiveBarrelView(_vfxLifetime, _barrelLifetime, _explosionEffect);
            
            _barrelHealth.Died += Explode;
        }
        
        private void OnDestroy() => _barrelHealth.Died -= Explode;

        public void Explode(Vector3 explosionPosition)
        {
            if (_isExploded) return;

            _isExploded = true;
            
            _explosiveBarrelExploder.Explode(explosionPosition);
            _explosiveBarrelView.Explode(explosionPosition, gameObject);
        }
    }
}