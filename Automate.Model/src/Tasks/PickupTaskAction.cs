using System;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public class PickupTaskAction : TaskAction, ITaskAction
    {
        public ComponentStackGroup TargetComponentStackGroup { get; }
        public Component ComponentType { get; }
        public PickupTaskAction(Guid masterTask, ComponentStackGroup componentStackGroup, Coordinate taskLocationCoordinate, Component component, int amount) : base(masterTask, taskLocationCoordinate, amount) {
            TargetComponentStackGroup = componentStackGroup;
            ComponentType = component;
            TaskActionType = TaskActionType.PickupTask;
        }

    }
}