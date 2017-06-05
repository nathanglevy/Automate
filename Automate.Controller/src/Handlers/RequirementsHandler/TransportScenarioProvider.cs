using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public abstract class TransportScenarioProvider : ITransportScenarioProvider
    {


        // INTERFACE METHODS SHOULD BE IMPLMENTED BY THE CHILD
        public abstract ScenarioTask CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Coordinate Destination, IGameWorld gameWorld);

        public abstract ScenarioTask CalcScenarioCost(RequirementJob structureCurrentJob, ITransportRequirement requirement, Boundary structureBoundary, IGameWorld gameWorld);




    }
}