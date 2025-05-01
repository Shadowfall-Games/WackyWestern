using System;
using System.Threading;
using HealthSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gun
{
    public class Gun
    {
        private readonly int _damage;
        private readonly float _rateOfFire;
        private readonly float _rechargeSpeed;
        private readonly int _maxBulletsAmount;
        private readonly bool _isMachineGun;
        private float _time;
        private int _currentBulletsAmount;
        private bool _canShoot;
        
        private readonly CancellationToken _destroyCancellationToken;

        private readonly Transform _originRay;
        private readonly InputSystem _inputSystem;
        public event Action<int> OnShot;
        public event Action<Vector3> OnHit;
        public event Action OnRecharged;

        public void OnEnable() =>  _inputSystem.Gun.Recharge.performed += Recharge;
        public void OnDisable() => _inputSystem.Gun.Recharge.performed -= Recharge;

        public Gun(CancellationToken destroyCancellationToken, Transform originRay, int damage, float rateOfFire, float rechargeSpeed, int maxBulletsAmount, bool isMachineGun)
        {
            _inputSystem = new InputSystem();
            _inputSystem.Gun.Enable();
            
            _destroyCancellationToken = destroyCancellationToken;
            _damage = damage;
            _rateOfFire = rateOfFire;
            _rechargeSpeed = rechargeSpeed;
            _maxBulletsAmount = maxBulletsAmount;
            _isMachineGun = isMachineGun;
            _originRay = originRay;

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