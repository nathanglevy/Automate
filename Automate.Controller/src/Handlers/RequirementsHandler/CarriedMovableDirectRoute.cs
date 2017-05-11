using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class CarriedMovableDirectRoute : ITransportScenarioProvider
    {
        public DeliveryCost CalcScenarioCost(RequirementJob requirmentJob, IRequirement requirement,
            Coordinate Destination, IGameWorld gameWorld)
        {
            var allIdleMovables = gameWorld.GetMovableList().Where(m => !m.IsInMotion());

            //TODO: Add While till all points delegated or got to maximum
            // TODO: CHANGE IT WHEN CompoenntType will be added to requirment
            var componentType = Component.IronOre;

            // GetAllMovables Has Such Component
            var idleMovablesHasComponent = allIdleMovables
                .Where(m => m.ComponentStackGroup.IsContainingComponentStack(componentType) &&
                            m.ComponentStackGroup.GetComponentStack(componentType).CurrentAmount > 0)
                .ToList();

            // In case of no match, returns the higest cost ever
            if (idleMovablesHasComponent.Count == 0)
                return new DeliveryCost(float.PositiveInfinity,null);

            // Get the Min Path & Cost between all of them
            var idleWithComponentStartPositions = idleMovablesHasComponent.Select(p => p.Coordinate).ToList();

            // Call Model To Get the Min Path/Cost
            var lowestCost =
                gameWorld.GetMovementPathWithLowestCostToCoordinate((List<Coordinate>) idleWithComponentStartPositions, Destination);

            // Get the movable located at this position
            var bestMovable = idleMovablesHasComponent.First(m => m.Coordinate.Equals(lowestCost.GetStartCoordinate()));

            // Create New Task
            var deliverTask = gameWorld.TaskDelegator.CreateNewTask();

            // Assign Movable
            gameWorld.TaskDelegator.AssignTask(bestMovable.Guid, deliverTask);

            // Create Delivery Task
            var deliverAction = deliverTask.AddTransportAction(TaskActionType.DeliveryTask, Destination,
                gameWorld.GetComponentStackGroupAtCoordinate(Destination + new Coordinate(1,0,0)),componentType,
                bestMovable.ComponentStackGroup.GetComponentStack(componentType).CurrentAmount);

            // Attach action to Req
            requirement.AttachAction(deliverAction);

            // return the Cost & the Task
            return new DeliveryCost(lowestCost.TotalCost, new TaskContainer(deliverTask));
        }
    }
}