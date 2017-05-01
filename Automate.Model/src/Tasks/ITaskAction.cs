using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public interface ITaskAction
    {
        Guid Guid { get; }
        Guid MasterTask { get; }
        Coordinate TaskLocation { get; }
        TaskActionType TaskActionType { get; }
        int Amount { get; }
        event CompletedEventHandler Completed;
        void OnCompleted();
    }
}