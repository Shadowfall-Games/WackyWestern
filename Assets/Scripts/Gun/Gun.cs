using System;
using System.Threading;
using HealthSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gun
{
    [Serializable]
    public class Gun
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
        
        private InputSystem _inputSystem;
        private float _time;
        private int _currentBulletsAmount;
        private bool _canShoot = false;
        
        private CancellationToken _destroyCancellationToken;

        public event Action<int> OnShot;
        public event Action<Vector3> OnHit;
        public event Action OnRecharged;

        public void OnEnable() =>  _inputSystem.Gun.Recharge.performed += Recharge;
        public void OnDisable() => _inputSystem.Gun.Recharge.performed -= Recharge;

        public void Init(CancellationToken destroyCancellationToken)
        {
            _inputSystem = new InputSystem();
            _inputSystem.Gun.Enable();
            
            _destroyCancellationToken = destroyCancellationToken;
            
            _currentBulletsAmount = _maxBulletsAmount;
            _time = _rateOfFire;
        }

        public void Update()
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
                OnHit?.Invoke(hit.point);
            }
            
            _currentBulletsAmount--;
            if (_currentBulletsAmount == 0) Recharge();
            _time = 0;
            
            OnShot?.Invoke(_currentBulletsAmount);
        }

        private async void Recharge()
        {
            OnRecharged?.Invoke();

            await Awaitable.WaitForSecondsAsync(_rechargeSpeed, _destroyCancellationToken);
            
            _canShoot = false;
            _canShoot = true;
            _currentBulletsAmount = _maxBulletsAmount;
            _time = _rateOfFire;
        }

        public void SetCanShoot(bool value) => _canShoot = value;
        
        private void Recharge(InputAction.CallbackContext obj) => Recharge();
    }
}