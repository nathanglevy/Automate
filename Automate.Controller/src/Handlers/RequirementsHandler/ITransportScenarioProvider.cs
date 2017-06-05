using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public interface ITransportScenarioProvider
    {
        ScenarioTask CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Coordinate Destination, IGameWorld gameWorld);

        ScenarioTask CalcScenarioCost(RequirementJob structureCurrentJob, ITransportRequirement requirement, Boundary structureBoundary, IGameWorld gameWorld);
    }
}