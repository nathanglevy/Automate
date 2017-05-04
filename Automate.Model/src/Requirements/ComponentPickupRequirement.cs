using Automate.Model.Components;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public class ComponentPickupRequirement : ComponentTransportRequirement, IRequirement {
        public override RequirementType RequirementType { get; } = RequirementType.ComponentPickup;
        public ComponentPickupRequirement(Component component, int amount) : base(component, amount) { }

        public override bool CanAttachToAction(ITaskAction taskAction)
        {
            return (taskAction.TaskActionType == TaskActionType.PickupTask);
        }
    }
}