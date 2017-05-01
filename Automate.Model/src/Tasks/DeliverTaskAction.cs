using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;

namespace Automate.Model.src.Tasks {
    class DeliverTaskAction : TaskAction, ITaskAction
    {
        public ComponentStackGroup TargetComponentStackGroup { get; }
        public Component ComponentType { get; }
        public DeliverTaskAction(Guid masterTask, ComponentStackGroup componentStackGroup, Coordinate taskLocationCoordinate, Component component, int amount) : base(masterTask, taskLocationCoordinate, amount)
        {
            TargetComponentStackGroup = componentStackGroup;
            ComponentType = component;
            TaskActionType = TaskActionType.DeliveryTask;
        }
    }
}
