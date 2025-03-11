using System;
using UnityEngine;

namespace RolesSystem.TasksStrategy.Tasks.Miner
{
    public class BringGold : IMinerTask
    {
        public event Action TaskCompleted;
        
        private bool _isCompleted;

        public void CompleteTask()
        {
            if (_isCompleted) return;
            
            TaskCompleted?.Invoke();
            _isCompleted = true;
            Debug.Log("Вы принесли золото!");
        }
    }
}