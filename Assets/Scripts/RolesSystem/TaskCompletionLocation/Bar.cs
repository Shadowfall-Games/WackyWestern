using RolesSystem.TasksStrategy.Tasks;
using RolesSystem.TasksStrategy.Tasks.Barman;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RolesSystem.TaskCompletionLocation
{
    public class Bar : MonoBehaviour
    {
        private readonly PourBeer _pourBeer = new();
        private readonly WashGlass _washGlass = new();
        private bool _canCompleteTask;

        private void Start()
        {
            TaskDivider taskDivider = FindAnyObjectByType<TaskDivider>();
            taskDivider.AddTask(_pourBeer);
            taskDivider.AddTask(_washGlass);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Вы вошли в бар.");
            _canCompleteTask = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Вы вышли из бара.");
            _canCompleteTask = false;
        }

        private void Update()
        {
            if (!_canCompleteTask) return;
            
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                _pourBeer.CompleteTask();
            }
            else if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                _washGlass.CompleteTask();
            }
        }
    }
}