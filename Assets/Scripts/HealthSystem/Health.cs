using System;
using UnityEngine;

namespace HealthSystem
{
    public class Health : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private int _maxHealth = 3;
        private int _currentHealth;
        private bool _isDied;

        public event Action<int> Recovered;
        public event Action<int> Damaged;
        public event Action Died;
        public event Action OnRevival;

        private void Start()
        {
            _currentHealth = _maxHealth;
            _isDied = false;
        }

        public void ApplyDamage(int damage)
        {
            if (_isDied) return;
            
            _currentHealth -= damage;
            Damaged?.Invoke(damage);
            Debug.Log("Damaged " + damage);
            
            if (_currentHealth > 0) return;
            
            Died?.Invoke();
            _isDied = true;
            Debug.Log("Died");
        }

        public void Recovery(int health)
        {
            if (_isDied) return;
            
            _currentHealth += health;
            Recovered?.Invoke(health);
            Mathf.Clamp(_currentHealth, 0, _maxHealth);
            Debug.Log("Recovery " + health);
        }

        public void Revival()
        {
            if (!_isDied) return;
            
            Start();
            OnRevival?.Invoke();
        }
    }
}