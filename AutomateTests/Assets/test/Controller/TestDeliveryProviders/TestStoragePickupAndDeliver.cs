using System.Collections.Generic;
using Automate.Controller.Handlers.RequirementsHandler;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller.TestDeliveryProviders
{
    [TestClass]
    public class TestStoragePickupAndDeliver
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var storagePickAndDeliver = new StoragePickAndDeliver();
            Assert.IsNotNull(storagePickAndDeliver);
        }

        [TestMethod]
        public void TestMovableStorageAndDeliverRequirment_ExpectCloseOneToGoPickUpandDeliver()
        {
            // Create World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create Storage
            var storage = gameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.Storage);
            storage.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 200);

            // Create Movables
            var movable = gameWorld.CreateMovable(new Coordinate(4, 1, 0), MovableType.NormalHuman);
            movable.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 0);

            // Create Factory
            var fire = gameWorld.CreateStructure(new Coordinate(17, 13, 0), new Coordinate(1, 1, 1), StructureType.LargeFire);
            fire.ComponentStackGroup.AddComponentStack(ComponentType.Wood, 0);

            // Create Delivery Requirment
            fire.CurrentJob = new RequirementJob(JobType.ItemTransport);
            fire.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.Wood, 123));
            
            // get the transport requirment
            var incompleteTransportRequirements = fire.CurrentJob.JobRequirements.GetIncompleteTransportRequirements();

            Assert.AreEqual(1,incompleteTransportRequirements.Count);

            var transportReq = incompleteTransportRequirements[0];

            // Handle
            var storagePickAndDeliver = new StoragePickAndDeliver();
            var calcScenarioCost = storagePickAndDeliver.CalcScenarioCost(fire.CurrentJob, transportReq, fire.Boundary, gameWorld);

            Assert.AreEqual(movable.Guid,calcScenarioCost.Task.TargetTask.AssignedToGuid);
            var taskActions = calcScenarioCost.Task.TargetTask.GetTaskActions();
            Assert.AreEqual(2,taskActions.Count);
            Assert.AreEqual(TaskActionType.PickupTask,taskActions[0].TaskActionType);
            Assert.AreEqual(TaskActionType.DeliveryTask,taskActions[1].TaskActionType);
        } 
    }
}
