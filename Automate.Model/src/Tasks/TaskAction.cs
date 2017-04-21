using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public class TaskAction
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Guid MasterTask { get; }
        public Coordinate TaskLocation { get; }
        public TaskActionType TaskActionType { get; }
        public int Amount { get; internal set; }

        internal TaskAction(Guid masterTask, Coordinate taskLocationCoordinate, TaskActionType taskActionType, int amount)
        {
            MasterTask = masterTask;
            TaskLocation = taskLocationCoordinate;
            TaskActionType = taskActionType;
            Amount = amount;
        }
    }
}