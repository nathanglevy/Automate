using System;
using System.Globalization;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Handlers.RequirementsHandler;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Model.CellComponents;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestRequirementsPackageHandler
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var handler = new RequirementsHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestCanHandleCorrectArgs_ExpectTrue()
        {
            var handler = new RequirementsHandler();
            Assert.IsTrue(handler.CanHandle(new RequirementsPackage(null)));
        }

        [TestMethod]
        public void TestCannotHandleInCorrectArgs_ExpectFalse()
        {
            var handler = new RequirementsHandler();
            Assert.IsFalse(handler.CanHandle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCanHandleWithNull_ExpectAnException()
        {
            var handler = new RequirementsHandler();
            handler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleIncorrectArgs_ExpectArgumentException()
        {
            var handler = new RequirementsHandler();
            handler.Handle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid()), null);
        }

        [TestMethod]
        public void
            TestHandlePickUpRequirmentWhenTwoIdleMovablesAndNoDeliveryNeeded_expectTaskWithAssigneToClosetAndDelivertoStorage()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));

            // Create two Movables
            var movabe430 = gameWorld.CreateMovable(new Coordinate(6, 3, 0), MovableType.SimpleRobot);
            var ironOreStack = movabe430.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);
            movabe430.ComponentStackGroup.MaxSize = 50;
            ironOreStack.StackMax = 50;

            var movabe880 = gameWorld.CreateMovable(new Coordinate(8, 8, 0), MovableType.SimpleRobot);
            var ironOreStack2 = movabe880.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);
            movabe430.ComponentStackGroup.MaxSize = 170;
            ironOreStack2.StackMax = 170;

            // Create a Storage
            var storage = gameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1),
                StructureType.Storage);
            storage.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);
            ironFactory.CurrentJob.JobRequirements.AddRequirement(
                new ComponentPickupRequirement(Component.IronOre, 200));
            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.IronOre, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 200);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            // Test Check

            // check that we have a single TASK
            var actionIndex = 0;
            Assert.AreEqual(2, handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe430.Guid, taskContainer.TargetTask.AssignedToGuid);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.PickupTask, taskContainer.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(4, 4, 0), taskContainer.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(50, taskContainer.TargetTask.GetCurrentAction().Amount);

            // check that we have a single TASK
            actionIndex = 1;
            Assert.AreEqual(2, handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer2 = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer2.TargetTask.IsAssigned);
            Assert.AreEqual(movabe880.Guid, taskContainer2.TargetTask.AssignedToGuid);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.PickupTask, taskContainer2.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(4, 4, 0), taskContainer2.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(150, taskContainer2.TargetTask.GetCurrentAction().Amount);
        }


        [TestMethod]
        public void TestDeliverRequirmentWhenOnlyIdleMovableWithRequiredComponent_ExpectDeliveryTaskAssignedToMovable()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create two Movables
            var movabe850 = gameWorld.CreateMovable(new Coordinate(8, 5, 0), MovableType.SimpleRobot);
            var ironOreStack = movabe850.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 50);
            movabe850.ComponentStackGroup.MaxSize = 50;
            ironOreStack.StackMax = 50;

            var movabe100 = gameWorld.CreateMovable(new Coordinate(1, 0, 0), MovableType.SimpleRobot);
            var ironOreStack2 = movabe100.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 170);
            movabe850.ComponentStackGroup.MaxSize = 170;
            ironOreStack2.StackMax = 170;

            // Create a Storage
            //var storage = gameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.Storage);
            //storage.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);
            ironFactory.CurrentJob.JobRequirements.AddRequirement(
                new ComponentPickupRequirement(Component.IronOre, 200));
            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.IronOre, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 200);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            // Test Check

            // check that we have a single TASK
            var actionIndex = 0;
            Assert.AreEqual(2, handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe850.Guid, taskContainer.TargetTask.AssignedToGuid);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.DeliveryTask, taskContainer.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(4, 4, 0), taskContainer.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(50, taskContainer.TargetTask.GetCurrentAction().Amount);

            // check that we have a single TASK
            actionIndex = 1;
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer2 = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer2.TargetTask.IsAssigned);
            Assert.AreEqual(movabe100.Guid, taskContainer2.TargetTask.AssignedToGuid);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.DeliveryTask, taskContainer2.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(4, 4, 0), taskContainer2.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(150, taskContainer2.TargetTask.GetCurrentAction().Amount);
        }


        //        [TestMethod]
        public void TestHandlePickUpRequirmentWhenTwoIdleMovablesBut_expectTaskWithAssigneWithPickUpActionAsOutput()
        {
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movabe430 = gameWorld.CreateMovable(new Coordinate(4, 3, 0), MovableType.SimpleRobot);
            var ironOre = movabe430.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);
            ironOre.StackMax = 30;

            var movabe880 = gameWorld.CreateMovable(new Coordinate(8, 8, 0), MovableType.SimpleRobot);

            var cellItem = new CellItem(gameWorld, new Coordinate(3, 3, 0));
            cellItem.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.IronOre, 200));

            var componentPickupRequirement = new ComponentPickupRequirement(Component.IronOre, 200);
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[0].Type);
            Assert.IsTrue(handlerResult.GetItems()[0] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[0] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe430.Guid, taskContainer.TargetTask.AssignedToGuid);
        }
    }
}
