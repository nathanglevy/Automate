using Automate.Model.Components;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public class ComponentDeliveryRequirement : ComponentTransportRequirement, IRequirement {
        public override RequirementType RequirementType { get; } = RequirementType.ComponentDelivery;
        public ComponentDeliveryRequirement(Component component, int amount) : base(component, amount) { }

        public override bool CanAttachToAction(ITaskAction taskAction)
        {
            return (taskAction.TaskActionType == TaskActionType.DeliveryTask);
        }
    }
}