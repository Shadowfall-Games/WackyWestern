using UnityEngine;
using System;

namespace RolesSystem.TasksStrategy.Tasks.Barman
{
    public class PourBeer : IBarmanTask
    {
        public event Action TaskCompleted;
        
        private bool _isCompleted;

        public void CompleteTask()
        {
            if (_isCompleted) return;
            
            TaskCompleted?.Invoke();
            _isCompleted = true;
            Debug.Log("Вы налили пиво!");
        }
    }
}