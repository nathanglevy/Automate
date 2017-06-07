//using System;
//using System.Linq;
//using Automate.Controller.Handlers.RequirementsHandler;
//using Automate.Controller.Handlers.TaskHandler;
//using Automate.Model.Components;
//using Automate.Model.GameWorldComponents;
//using Automate.Model.Jobs;
//using Automate.Model.MapModelComponents;
//using Automate.Model.Movables;
//using Automate.Model.Requirements;
//using Automate.Model.StructureComponents;
//using Automate.Model.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//
//namespace AutomateTests.Assets.test.Controller.TestDeliveryProviders
//{
//    [TestClass]
//    public class TestPickUpFromNonStorageAndDeliver
//    {
//
//        [TestMethod]
//        public void TestCreateNew_ShouldPass()
//        {
//            var dynamicPickupAndDeliver = new DynamicPickupAndDeliver();
//            Assert.IsNotNull(dynamicPickupAndDeliver);
//        }
//
//        [TestMethod]
//        public void TestPickUpFromAStructureWantsToGetRidOfComponentAndDeliverToTarget()
//        {
//
//            // Create World
//            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
//
//            // Create Storage
//            var constructionArea = gameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1),
//                StructureType.Construction);
//            constructionArea.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 200);
//
//
//            // Create Movables
//            var movable = gameWorld.CreateMovable(new Coordinate(4, 1, 0), MovableType.NormalHuman);
//            movable.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 0);
//
//            // Create Factory
//            var fire = gameWorld.CreateStructure(new Coordinate(17, 13, 0), new Coordinate(1, 1, 1),
//                StructureType.LargeFire);
//            fire.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 0);
//
//            // Create Delivery Requirment
//            fire.CurrentJob = new RequirementJob(JobType.ItemTransport);
//            fire.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.Wood, 123));
//
//            // get the transport requirment
//            var incompleteTransportRequirements = fire.CurrentJob.JobRequirements.GetIncompleteTransportRequirements();
//
//            Assert.AreEqual(1, incompleteTransportRequirements.Count);
//
//            var transportReq = incompleteTransportRequirements[0];
//
//            // Handle
//            var dynamicPickupAndDeliver = new DynamicPickupAndDeliver();
//            var calcScenarioCost =
//                dynamicPickupAndDeliver.CalcScenarioCost(fire.CurrentJob, transportReq, fire.Boundary, gameWorld);
//
//            Assert.AreEqual(movable.Guid, calcScenarioCost.Task.TargetTask.AssignedToGuid);
//            var taskActions = calcScenarioCost.Task.TargetTask.GetTaskActions();
//            Assert.AreEqual(2, taskActions.Count);
//            Assert.AreEqual(TaskActionType.PickupTask, taskActions[0].TaskActionType);
//            Assert.AreEqual(TaskActionType.DeliveryTask, taskActions[1].TaskActionType);
//        }
//    }
//
//    public class DynamicPickupAndDeliver: TransportScenarioProvider
//    {
//        public override ScenarioTask CalcScenarioCost(RequirementJob requirmentJob, ITransportRequirement requirement, Coordinate Destination, IGameWorld gameWorld)
//        {
//            throw new NotImplementedException();
//        }
//
//        public override ScenarioTask CalcScenarioCost(RequirementJob structureCurrentJob, ITransportRequirement requirement, Boundary boundary, IGameWorld gameWorld)
//        {
//            var componentType = requirement.Component;
//            var movables = gameWorld.GetMovableList().Where(m => !m.IsInMotion() && m.ComponentStackGroup.IsContainingComponentStack(componentType));
//
//            // In case of no movable exists -- nothing we can do
//            if (!movables.Any())
//            {
//                return new ScenarioCost(float.PositiveInfinity, null, 0);
//            }
//
//            // LOCATE STORAGE HAS COMPONENT
//            var transportReqs = gameWorld.RequirementAgent.GetStructuresWithJobInProgress()
//                .Where(s => s.ComponentStackGroup.IsContainingComponentStack(componentType) && s.HasActiveJob && s.CurrentJob.JobType.Equals(JobType.ItemTransport));
//
//            var pickupRequirments = transportReqs.Where(t => t.CurrentJob.JobRequirements.GetAllRequirements()
//                .Any(j => !j.IsSatisfied && j.RequirementType == RequirementType.ComponentPickup));
//
////            pickupRequirments.GetEnumerator().Current.CurrentJob.
//            var storageWithComponent = gameWorld.GetStructuresList().Where(s => s.StructureType.Equals(StructureType.Storage) && s.ComponentStackGroup.IsContainingComponentStack(componentType));
//            if (!storageWithComponent.Any())   
//            {
//                return new ScenarioCost(float.PositiveInfinity, null, 0);
//            }
//
//            // Find the closest storage to delivery location
//            // TODO: REMOVE THE addition of 1,0,0
//            var storageToDestShortestPath = gameWorld.GetMovementPathWithLowestCostToBoundary(storageWithComponent.Select(s => (s.Coordinate + new Coordinate(1, 0, 0))).ToList(),
//                boundary, false);
//
//            var pickupStorage = storageWithComponent.First(s => s.Coordinate.Equals(storageToDestShortestPath.GetStartCoordinate() - new Coordinate(1, 0, 0)));
//
//            // Find the closest movable to targetStorage
//            var movableToStorageShortestPath = gameWorld.GetMovementPathWithLowestCostToBoundary(movables.Select(m => m.Coordinate).ToList(),
//                pickupStorage.Boundary, false);
//
//            // TODO: REMOVE THE addition of 1,0,0
//            var selectedMovable = movables.First(m => m.Coordinate.Equals(movableToStorageShortestPath.GetStartCoordinate()));
//
//
//            // the ScenarioCost will be the SUM of TWO PATHS FOR NOW (ASSUMING PICKUP has no cost)
//            // 
//            float totalCost = storageToDestShortestPath.TotalCost + movableToStorageShortestPath.TotalCost;
//
//            // NOW will create 2 tasks to pickup and deliver
//
//            // Create New ScenarioTask
//            var pickupAndDeliverTask = gameWorld.TaskDelegator.CreateNewTask();
//
//            // Assign Movable
//            gameWorld.TaskDelegator.AssignTask(selectedMovable.Guid, pickupAndDeliverTask);
//
//            // Create Delivery ScenarioTask
//            // get the max amount the movable can pickup
//            var pickupAmount = selectedMovable.ComponentStackGroup.GetComponentStack(componentType).RemainingAmountForIncoming;
//
//            // in case that the movable can deliver more than needed
//            if (requirement.RequirementRemainingToDelegate < pickupAmount)
//            {
//                pickupAmount = requirement.RequirementRemainingToDelegate;
//            }
//            // create the Pickup Action
//            var pickupAction = pickupAndDeliverTask.AddTransportAction(TaskActionType.PickupTask, movableToStorageShortestPath.GetEndCoordinate(),
//                gameWorld.GetComponentStackGroupAtCoordinate(pickupStorage.Boundary.topLeft), componentType, pickupAmount
//            );
//
//            // create the Pickup Action
//            var deliverAction = pickupAndDeliverTask.AddTransportAction(TaskActionType.DeliveryTask, storageToDestShortestPath.GetEndCoordinate(),
//                gameWorld.GetComponentStackGroupAtCoordinate(boundary.topLeft), componentType, pickupAmount
//            );
//
//            // Attach action to Req
//            requirement.AttachAction(deliverAction);
//
//            // return the ScenarioCost & the ScenarioTask
//            return new ScenarioCost(totalCost, new TaskContainer(pickupAndDeliverTask), pickupAmount);
//
//        }
//
// 
//    }
//}
