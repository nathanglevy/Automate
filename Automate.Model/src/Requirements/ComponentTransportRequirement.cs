using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Automate.Model.Components;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public abstract class ComponentTransportRequirement : BaseRequirement, ITransportRequirement {
        public Component Component { get; }
        public abstract RequirementType RequirementType { get; }

        internal ComponentTransportRequirement(Component component, int amount)
        {
            if (amount < 0)
                throw new RequirementException("Cannot set amount to negative number");
            Amount = amount;
            Component = component;
            TotalRequirement = amount;
        }

        public bool SatisfyRequirement(int amount)
        {
            if (amount > Amount)
                throw new RequirementException("Cannot satisfy more than requirement!");
            Amount -= amount;
            return IsSatisfied;
        }

        public void AttachAction(ITaskAction taskAction)
        {
            if (!CanAttachToAction(taskAction))
                throw new TaskActionException("Cannot attatch task action to this type of requirement");
            _taskActions.Add(taskAction);
            taskAction.Completed += OnTaskCompleted;
        }

        public void DettachAction(ITaskAction taskAction)
        {
            if (!_taskActions.Contains(taskAction))
                throw new TaskActionException("Cannot detatch task because it is not attached");
            _taskActions.Remove(taskAction);
            taskAction.Completed -= OnTaskCompleted;
        }

        public void OnTaskCompleted(object sender, TaskActionEventArgs e) {
            SatisfyRequirement(e.Amount);
            DettachAction(sender as TaskAction);
        }

        public abstract bool CanAttachToAction(ITaskAction taskAction);
    }
}