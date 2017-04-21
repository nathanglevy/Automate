using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;

namespace Automate.Model.src.Tasks {
    class TransportTaskAction : TaskAction {
        public ComponentStack ComponentStack { get; }
        public TransportTaskAction(Guid masterTask, ComponentStack componentStack, Coordinate taskLocationCoordinate, TaskActionType taskActionType, int amount) : base(masterTask, taskLocationCoordinate, taskActionType, amount)
        {
            ComponentStack = componentStack;
        }
    }
}
