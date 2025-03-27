using System;

namespace HealthSystem
{
    public interface IHealthSystem
    {
        event Action<int> Recovered;
        event Action<int> Damaged;
        event Action Died;
        event Action OnRevival;
        
        void ApplyDamage(int damage);

        void Recovery(int health);

        void Revival();
    }
}