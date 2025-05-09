using System;
using UnityEngine;

namespace HealthSystem
{
    public interface IHealthSystem
    {
        event Action<int> Recovered;
        event Action<int> Damaged;
        event Action<Vector3> Died;
        event Action OnRevival;
        
        void ApplyDamage(int damage, Vector3 hitPoint);

        void Recovery(int health);

        void Revival();
    }
}