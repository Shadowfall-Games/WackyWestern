using System.Collections.Generic;
using RolesSystem.TasksStrategy.Tasks;
using UnityEngine;

namespace RolesSystem
{
    public class TaskDivider : MonoBehaviour
    {
        private List<IBarmanTask> _barmanTasks = new();
        private List<IMinerTask> _minerTasks = new();

        public void AddTask(IBarmanTask task)
        {
            _barmanTasks.Add(task);
        }
        
        public void AddTask(IMinerTask task)
        {
            _minerTasks.Add(task);
        }
    }
}