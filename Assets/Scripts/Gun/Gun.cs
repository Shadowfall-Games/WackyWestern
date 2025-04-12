using System;
using HealthSystem;
using Player.ActiveRagdoll;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        [Header("Raycast start point")]
        [SerializeField] private Transform _originRay;
        
        [Header("Gun specs")]
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _recoilForce = 1;
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _rechargeSpeed = 3;
        [SerializeField] private int _maxBulletsAmount = 20;
        [SerializeField] private bool _isMachineGun;

        [Header("VFX")]
        [SerializeField] private float _vfxDuration = 0.5f;
        [SerializeField] private VisualEffect _impactExplosion;
        
        private InputSystem _inputSystem;
        private float _time;
        private int _currentBulletsAmount;
        private bool _canShoot = true;
        
        private ActiveRagdoll _currentActiveRagdoll;

        public event Action<int> OnShoot;
        public event Action OnRecharge;

        private void OnEnable() =>  _inputSystem.Gun.Recharge.performed += Recharge;

        private void Awake()
        {
            _inputSystem = new InputSystem();
            _inputSystem.Gun.Enable();
            
            _currentBulletsAmount = _maxBulletsAmount;
            _time = _rateOfFire;
        }

        private void Update()
        {
            Debug.DrawRay(_originRay.position, _originRay.forward, Color.blue);

            if (!_canShoot) return;
            
            _time += Time.deltaTime;
            if (_time < _rateOfFire) return;
            
            if (_isMachineGun)
            {
                if (_inputSystem.Gun.Shoot.IsPressed()) Shoot();
            }
            else
            {
                if (_inputSystem.Gun.Shoot.WasPerformedThisFrame()) Shoot();
            }
        }

        protected virtual void Shoot()
        {
            if (Physics.Raycast(_originRay.position, _originRay.forward, out RaycastHit hit))
            {
                var healthSystem = hit.collider.GetComponentInParent<IHealthSystem>();
                if (healthSystem != null) healthSystem.ApplyDamage(_damage);
                SpawnVFX(hit.point);
            }
            
            _currentBulletsAmount--;
            if (_currentBulletsAmount == 0) Recharge();
            _time = 0;
            
            OnShoot?.Invoke(_currentBulletsAmount);
        }

        private async void Recharge()
        {
            OnRecharge?.Invoke();

            await Awaitable.WaitForSecondsAsync(_rechargeSpeed, destroyCancellationToken);
            
            _canShoot = false;
            _canShoot = true;
            _currentBulletsAmount = _maxBulletsAmount;
            _time = _rateOfFire;
        }
        
        private void SpawnVFX(Vector3 position)
        {
            var vfxInstance = Instantiate(_impactExplosion, position, Quaternion.identity);

            VisualEffect vfx = vfxInstance.GetComponent<VisualEffect>();
            Destroy(vfxInstance.gameObject, _vfxDuration);
        }
        
        private void Recharge(InputAction.CallbackContext obj) => Recharge();

        private void OnDisable() => _inputSystem.Gun.Recharge.performed -= Recharge;
    }
}