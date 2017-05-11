using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public interface ITransportScenarioProvider
    {
        DeliveryCost CalcScenarioCost(RequirementJob requirmentJob, IRequirement requirement, Coordinate Destination,
            IGameWorld gameWorld);
    }
}