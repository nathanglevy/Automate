using Automate.Controller.Abstracts;
using Automate.Model.GameWorldComponents;
using Automate.Model.Requirements;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class RequirementsPackage : ModelMasterAction
    {
        public ComponentPickupRequirement Requirement { get; }
        public Item HostingItem { get; }

        public RequirementsPackage(IRequirementAgent gameWorldRequirementAgent) : base(ActionType.Internal)
        {
        }
    }
}