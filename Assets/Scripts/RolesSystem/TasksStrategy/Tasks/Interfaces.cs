using System;

namespace RolesSystem.TasksStrategy.Tasks
{
    public interface ITask
    {
        event Action TaskCompleted;

        void CompleteTask();
    }

    public interface ISheriffTask : ITask { }
    
    public interface IMinerTask : ITask { }
    
    public interface IBarmanTask : ITask { }
    
    public interface IShepherdTask : ITask { }
    
    public interface IBanditTask : ITask { }
    
    public interface IPriestTask : ITask { }
    
    public interface IGunsmithTask : ITask { }
    
    public interface IBankerTask : ITask   { }
    
    public interface ITradesmanTask : ITask   { }
    
    public interface IHunterTask : ITask   { }
    
    public interface IGravediggerTask : ITask   { }
    
    public interface ITownsmanTask : ITask   { }
}