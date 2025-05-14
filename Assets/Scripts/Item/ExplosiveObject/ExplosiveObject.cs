using HealthSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Item.ExplosiveObject
{
    public abstract class ExplosiveObject : MonoBehaviour
    {
        [Header("Explosion settings")]
        [SerializeField] private int _explosionDamage = 100;
        [SerializeField] private float _forceRadius = 8;
        [SerializeField] private float _deadlyRadius = 4;
        [SerializeField] private float _explosionForce = 800;
        [SerializeField] private float _objectsExplosionForce = 100;
        [SerializeField] private float _upwardsForce = 2;
        
        [Header("Explosion view")] 
        [SerializeField] protected float VFXLifetime = 5.5f;
        [SerializeField] protected float ObjectLifetime = 1;
        [SerializeField] protected VisualEffect ExplosionEffect;
        
        private bool _isExploded;
        
        private ExplosiveObjectExploder _explosiveObjectExploder;
        protected ExplosiveObjectView ExplosiveObjectView;
        
        private IHealthSystem _objectHealth;

        public virtual void Start()
        {
            _objectHealth = GetComponent<IHealthSystem>();
            
            _explosiveObjectExploder = new ExplosiveObjectExploder(_explosionDamage, _forceRadius, _deadlyRadius, _explosionForce, _objectsExplosionForce, _upwardsForce);
            ExplosiveObjectView = CreateView();
            
            _objectHealth.Died += Explode;
        }
        
        protected virtual ExplosiveObjectView CreateView()
        {
            return new ExplosiveObjectView(VFXLifetime, ObjectLifetime, ExplosionEffect);
        }
        
        private void OnDestroy() => _objectHealth.Died -= Explode;

        public void Explode(Vector3 explosionPosition)
        {
            if (_isExploded) return;

            _isExploded = true;
            
            _explosiveObjectExploder.Explode(explosionPosition);
            ExplosiveObjectView.Explode(explosionPosition, gameObject);
        }
    }
}