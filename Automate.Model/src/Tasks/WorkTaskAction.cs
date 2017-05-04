using System;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Tasks
{
    public class WorkTaskAction : TaskAction {
        public Component ConsumesComponent { get; }

        public WorkTaskAction(Guid masterTask, Coordinate taskLocationCoordinate, Component component, int amount) : base(masterTask, taskLocationCoordinate, amount)
        {
            ConsumesComponent = component;
            TaskActionType = TaskActionType.WorkTask;
        }
    }
}