using System;
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
        public DeliveryCost CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement,
            Coordinate Destination, IGameWorld gameWorld)
        {
            throw  new NotImplementedException();
        }

        public DeliveryCost CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement,
            Boundary boundery, IGameWorld gameWorld)
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
                return new DeliveryCost(float.PositiveInfinity,null,0);

            // Get the Min Path & Cost between all of them
            var idleWithComponentStartPositions = idleMovablesHasComponent.Select(p => p.Coordinate).ToList();

            // Call Model To Get the Min Path/Cost
            var lowestCost =
                gameWorld.GetMovementPathWithLowestCostToBoundary((List<Coordinate>) idleWithComponentStartPositions, boundery,false);

            // Get the movable located at this position
            var bestMovable = idleMovablesHasComponent.First(m => m.Coordinate.Equals(lowestCost.GetStartCoordinate()));
            

            // Create New Task
            var deliverTask = gameWorld.TaskDelegator.CreateNewTask();

            // Assign Movable
            gameWorld.TaskDelegator.AssignTask(bestMovable.Guid, deliverTask);

            // Create Delivery Task
            var deliveryAmount = bestMovable.ComponentStackGroup.GetComponentStack(componentType).CurrentAmount;
            if (requirement.RequirementRemainingToDelegate < deliveryAmount)
            {
                deliveryAmount = requirement.RequirementRemainingToDelegate;
            }
            var deliverAction = deliverTask.AddTransportAction(TaskActionType.DeliveryTask, lowestCost.GetEndCoordinate(),
                gameWorld.GetComponentStackGroupAtCoordinate(boundery.topLeft),componentType,deliveryAmount
               );

            // Attach action to Req
            requirement.AttachAction(deliverAction);

            // return the Cost & the Task
            return new DeliveryCost(lowestCost.TotalCost, new TaskContainer(deliverTask),deliveryAmount);
        }
    }
}