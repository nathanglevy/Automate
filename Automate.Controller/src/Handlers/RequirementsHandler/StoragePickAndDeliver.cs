using System.Linq;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class StoragePickAndDeliver : ITransportScenarioProvider
    {
        public DeliveryCost CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Coordinate Destination,
            IGameWorld gameWorld)
        {
            throw new System.NotImplementedException();
        }

        public DeliveryCost CalcScenarioCost(RequirementJob structureCurrentJob, ITransportRequirement requirement,
            Boundary boundary, IGameWorld gameWorld)
        {
            var componentType = requirement.Component;
            var movables = gameWorld.GetMovableList().Where(m => !m.IsInMotion() && m.ComponentStackGroup.IsContainingComponentStack(componentType));

            // In case of no movable exists -- nothing we can do
            if (!movables.Any())
            {
                return new DeliveryCost(float.PositiveInfinity, null, 0);
            }

            // LOCATE STORAGE HAS COMPONENT
            var storageWithComponent = gameWorld.GetStructuresList().Where(s => s.StructureType.Equals(StructureType.Storage) && s.ComponentStackGroup.IsContainingComponentStack(componentType));
            if (!storageWithComponent.Any())
            {
                return new DeliveryCost(float.PositiveInfinity, null, 0);
            }

            // Find the closest storage to delivery location
            // TODO: REMOVE THE addition of 1,0,0
            var storageToDestShortestPath = gameWorld.GetMovementPathWithLowestCostToBoundary(storageWithComponent.Select(s => (s.Coordinate + new Coordinate(1,0,0))).ToList(),
                boundary, false);

            var pickupStorage  = storageWithComponent.First(s => s.Coordinate.Equals(storageToDestShortestPath.GetStartCoordinate() - new Coordinate(1,0,0)));

// Find the closest movable to targetStorage
            var movableToStorageShortestPath = gameWorld.GetMovementPathWithLowestCostToBoundary(movables.Select(m => m.Coordinate).ToList(),
                pickupStorage.Boundary, false);

            // TODO: REMOVE THE addition of 1,0,0
            var selectedMovable = movables.First(m => m.Coordinate.Equals(movableToStorageShortestPath.GetStartCoordinate()));

            
            // the Cost will be the SUM of TWO PATHS FOR NOW (ASSUMING PICKUP has no cost)
            // 
            float totalCost = storageToDestShortestPath.TotalCost + movableToStorageShortestPath.TotalCost;

            // NOW will create 2 tasks to pickup and deliver

            // Create New Task
            var pickupAndDeliverTask = gameWorld.TaskDelegator.CreateNewTask();

            // Assign Movable
            gameWorld.TaskDelegator.AssignTask(selectedMovable.Guid, pickupAndDeliverTask);

            // Create Delivery Task
            // get the max amount the movable can pickup
            var pickupAmount = selectedMovable.ComponentStackGroup.GetComponentStack(componentType).RemainingAmountForIncoming;

            // in case that the movable can deliver more than needed
            if (requirement.RequirementRemainingToDelegate < pickupAmount)
            {
                pickupAmount = requirement.RequirementRemainingToDelegate;
            }
            // create the Pickup Action
            var pickupAction = pickupAndDeliverTask.AddTransportAction(TaskActionType.PickupTask, movableToStorageShortestPath.GetEndCoordinate(),
                gameWorld.GetComponentStackGroupAtCoordinate(pickupStorage.Boundary.topLeft), componentType, pickupAmount
            );

            // create the Pickup Action
            var deliverAction = pickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, storageToDestShortestPath.GetEndCoordinate(),
                gameWorld.GetComponentStackGroupAtCoordinate(boundary.topLeft), componentType, pickupAmount
            );

            // Attach action to Req
            requirement.AttachAction(deliverAction);

            // return the Cost & the Task
            return new DeliveryCost(totalCost, new TaskContainer(pickupAndDeliverTask), pickupAmount);

        }
    }
}