using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class CarriedMovableDirectRoute : TransportScenarioProvider, ITransportScenarioProvider
    {

        public override ScenarioTask CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Coordinate Destination, IGameWorld gameWorld)
        {
            throw  new NotImplementedException();
        }

        public override ScenarioTask CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Boundary boundery, IGameWorld gameWorld)
        {
            var allIdleMovables = gameWorld.GetMovableList().Where(m => !m.IsInMotion());

            var componentType = requirement.Component;

            // GetAllMovables Has Such Component
            var idleMovablesHasComponent = allIdleMovables
                .Where(m => m.ComponentStackGroup.IsContainingComponentStack(componentType) && // has the needed component 
                            m.ComponentStackGroup.GetComponentStack(componentType).CurrentAmount > 0 && // has some amount of needed component
                            !gameWorld.TaskDelegator.HasDelegatedTasks(m.Guid)) // has no tasks which already delegated
                .ToList();

            

            // In case of no match, returns the higest cost ever
            if (idleMovablesHasComponent.Count == 0)
                return new ScenarioTask(new ScenarioCost(float.PositiveInfinity),null,null);

            // Get the Min Path & ScenarioCost between all of them
            var idleWithComponentStartPositions = idleMovablesHasComponent.Select(p => p.Coordinate).ToList();

            // Call GameWorldGuid To Get the Min Path/ScenarioCost
            var lowestCost =
                gameWorld.GetMovementPathWithLowestCostToBoundary((List<Coordinate>) idleWithComponentStartPositions, boundery,false);

            // Get the movable located at this position
            var selectedMovable = idleMovablesHasComponent.First(m => m.Coordinate.Equals(lowestCost.GetStartCoordinate()));

            // Create New ScenarioTask
            var deliverTask = gameWorld.TaskDelegator.CreateNewTask();

            // Assign Movable
            gameWorld.TaskDelegator.AssignTask(selectedMovable.Guid, deliverTask);

            // Create Delivery ScenarioTask
            var deliveryAmount = selectedMovable.ComponentStackGroup.GetComponentStack(componentType).CurrentAmount;
            if (requirement.RequirementRemainingToDelegate < deliveryAmount)
            {
                deliveryAmount = requirement.RequirementRemainingToDelegate;
            }
            var deliverAction = deliverTask.AddTransportAction(TaskActionType.DeliveryTask, lowestCost.GetEndCoordinate(),
                gameWorld.GetComponentStackGroupAtCoordinate(boundery.topLeft),componentType,deliveryAmount
               );

            // Update the Target Structure for Future use
            var structure = gameWorld.GetStructureAtCoordinate(boundery.topLeft);
           

            // return the ScenarioCost & the ScenarioTask
            var cost =  new ScenarioCost(lowestCost.TotalCost);
            var scenarioTask = new ScenarioTask(cost, new TaskContainer(deliverTask), delegate()
            {
                // in case of Scenrario Execution - the following should be done
                selectedMovable.ComponentStackGroup.GetComponentStack(componentType).AssignOutgoingAmount(selectedMovable.Guid,deliveryAmount);
                structure.ComponentStackGroup.GetComponentStack(componentType).AssignIncomingAmount(selectedMovable.Guid,deliveryAmount);

                // attach the Requirement to the Action
                requirement.AttachAction(deliverAction);
            });
            return scenarioTask;
        }

    }
}