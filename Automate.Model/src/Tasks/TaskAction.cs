using System;
using Automate.Model.MapModelComponents;
using UnityEngine;

namespace Automate.Model.Tasks
{
    public class TaskAction : ITaskAction
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Guid MasterTask { get; }
        public Coordinate TaskLocation { get; }
        public TaskActionType TaskActionType { get; internal set; }
        public int Amount { get; }
        public event CompletedEventHandler Completed;

        internal TaskAction(Guid masterTask, Coordinate taskLocationCoordinate, int amount)
        {
            MasterTask = masterTask;
            TaskLocation = taskLocationCoordinate;
            TaskActionType = TaskActionType.GenericTask;
            Amount = amount;
        }

        public virtual void OnCompleted()
        {
            Completed?.Invoke(this, new TaskActionEventArgs(Amount));
        }

    }
}