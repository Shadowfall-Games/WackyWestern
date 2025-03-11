using System;
using RolesSystem.TasksStrategy.Tasks.Miner;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RolesSystem.TaskCompletionLocation
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private Transform _mineCenter;
        [SerializeField] private Transform _marketCenter;
        [SerializeField] private LayerMask _playerLayer;
        
        [SerializeField] private float _radius; 
        
        private readonly BringGold _bringGold = new();

        private bool _canTakeGold;
        private bool _canBringGold;
        private bool _haveGold;

        private void Start()
        {
            TaskDivider taskDivider = FindAnyObjectByType<TaskDivider>();
            taskDivider.AddTask(_bringGold);
        }

        private void FixedUpdate()
        {
            var colliders = Physics.OverlapSphere(_mineCenter.position, _radius, _playerLayer);
            _canTakeGold = colliders.Length != 0;
            
             colliders = Physics.OverlapSphere(_marketCenter.position, _radius, _playerLayer);
             _canBringGold = colliders.Length != 0;
        }

        private void Update()
        {
            if (!Keyboard.current.fKey.wasPressedThisFrame) return;
            
            if (_canTakeGold)
            {
                _haveGold = true;
                Debug.Log("Вы взяли золото.");
            }

            if (_canBringGold && _haveGold)
            {
                _bringGold.CompleteTask();
            }
        }
    }
}