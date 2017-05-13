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
            TestHandlePickUpRequirementWhenTwoIdleMovablesAndNoDeliveryNeeded_expectTaskWithAssigneToClosetAndDelivertoStorage()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create two Movables
            var closeMovabe = gameWorld.CreateMovable(new Coordinate(6, 3, 0), MovableType.SimpleRobot);
            var woodMovableStack = closeMovabe.ComponentStackGroup.AddComponentStack(Component.Wood, 0);
            closeMovabe.ComponentStackGroup.MaxSize = 500;
            closeMovabe.ComponentStackGroup.GetComponentStack(Component.Wood).StackMax = 500;
            woodMovableStack.StackMax = 500;

            var farMovable = gameWorld.CreateMovable(new Coordinate(18, 8, 0), MovableType.SimpleRobot);
            var ironOreStack2 = farMovable.ComponentStackGroup.AddComponentStack(Component.Wood, 0);
            farMovable.ComponentStackGroup.MaxSize = 1200;
            ironOreStack2.StackMax = 1200;

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);

            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.Wood, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(Component.Wood, 200);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));


            // check that we have a single TASK
            var actionIndex = 0;
            Assert.AreEqual(2, handlerResult.GetItems().Count);
            Assert.AreEqual(170,
                ironFactory.ComponentStackGroup.GetComponentStack(Component.Wood).OutgoingAllocatedAmount);

            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(closeMovabe.Guid, taskContainer.TargetTask.AssignedToGuid);
            Assert.AreEqual(50,closeMovabe.ComponentStackGroup.GetComponentStack(Component.Wood).IncomingAllocatedAmount);

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
            Assert.AreEqual(farMovable.Guid, taskContainer2.TargetTask.AssignedToGuid);
            Assert.AreEqual(120, farMovable.ComponentStackGroup.GetComponentStack(Component.Wood).IncomingAllocatedAmount);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.PickupTask, taskContainer2.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(4, 4, 0), taskContainer2.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(120, taskContainer2.TargetTask.GetCurrentAction().Amount);
        }

        [TestMethod]
        public void
            TestHandlePickUpRequirementWhenNoMovablesToDoTheJob_expectNothingToHappenAndNoTasksToBeAddedAndRequirmentStillActiveForNextRound()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);

            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.Wood, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(Component.Wood, 200);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));


            // check that we have a single TASK
            Assert.AreEqual(0, handlerResult.GetItems().Count);
            Assert.AreEqual(0, ironFactory.CurrentJob.PointsOfWorkDone);
            Assert.AreEqual(200, ironFactory.CurrentJob.PointsOfWorkRemaining);
        }



        [TestMethod]
        public void TestDeliverRequirmentWhenIdleMovableWithRequiredComponent_ExpectDeliveryTaskAssignedToMovable()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create two Movables
            var movabe850 = gameWorld.CreateMovable(new Coordinate(18, 5, 0), MovableType.SimpleRobot);
            var ironOreStack = movabe850.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 50);
            movabe850.ComponentStackGroup.MaxSize = 50;
            ironOreStack.StackMax = 50;

            var movabe400 = gameWorld.CreateMovable(new Coordinate(4, 0, 0), MovableType.SimpleRobot);
            var ironOreStack2 = movabe400.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 170);
            movabe850.ComponentStackGroup.MaxSize = 170;
            ironOreStack2.StackMax = 170;

            // Create a Storage
            //var storage = gameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.Storage);
            //storage.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);

            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.IronOre, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            // Test Check

            var actionIndex = 0;
            Assert.AreEqual(2, handlerResult.GetItems().Count);
            Assert.AreEqual(200, ironFactory.ComponentStackGroup.GetComponentStack(ComponentType.IronOre).IncomingAllocatedAmount);


            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe400.Guid, taskContainer.TargetTask.AssignedToGuid);
            Assert.AreEqual(170, movabe400.ComponentStackGroup.GetComponentStack(ComponentType.IronOre).OutgoingAllocatedAmount);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.DeliveryTask, taskContainer.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(5, 4, 0), taskContainer.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(170, taskContainer.TargetTask.GetCurrentAction().Amount);


            actionIndex = 1;
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[actionIndex].Type);
            Assert.IsTrue(handlerResult.GetItems()[actionIndex] is TaskContainer);
            taskContainer = handlerResult.GetItems()[actionIndex] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe850.Guid, taskContainer.TargetTask.AssignedToGuid);
            Assert.AreEqual(30,movabe850.ComponentStackGroup.GetComponentStack(ComponentType.IronOre).OutgoingAllocatedAmount);

            // we expect 2 Sub actions
            // a. pickup Task from Dest
            // b. Deliver Task to Storage
            Assert.AreEqual(TaskActionType.DeliveryTask, taskContainer.TargetTask.GetCurrentAction().TaskActionType);
            Assert.AreEqual(new Coordinate(5, 5, 0), taskContainer.TargetTask.GetCurrentAction().TaskLocation);
            Assert.AreEqual(30, taskContainer.TargetTask.GetCurrentAction().Amount);



        }

        [TestMethod]
        public void TestDeliverRequirmentsWhenNoMovablesToDo_ExpectNothingValidRequirment()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);

            ironFactory.CurrentJob = new RequirementJob(JobType.ItemTransport);
            ironFactory.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.IronOre, 200));
            ironFactory.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            // create the Requirement package
            var requirementWrapper = new RequirementsPackage(gameWorld.RequirementAgent);

            // Create the Handler
            var handler = new RequirementsHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            // Test Check
            Assert.AreEqual(0, handlerResult.GetItems().Count);
            Assert.AreEqual(0, ironFactory.CurrentJob.PointsOfWorkDone);
            Assert.AreEqual(200, ironFactory.CurrentJob.PointsOfWorkRemaining);
        }

        [TestMethod]
        public void TestGetComponentStackOfCoordinateWhenBelongToStructure()
        {
            // Create the World
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));

            // Create a PickUp Requirement
            var ironFactory = gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(2, 2, 1),
                StructureType.Basic);
            ironFactory.ComponentStackGroup.AddComponentStack(ComponentType.Stone, 200);

            var componentStackGroup = gameWorld.GetComponentStackGroupAtCoordinate(ironFactory.Coordinate);
            Assert.IsTrue(componentStackGroup.IsContainingComponentStack(ComponentType.Stone));
        }

        [TestMethod]
        public void NOT_IMP_TestHandleIdleMovablesWithCarriedComponents_expectDeliverytoClosetStorage()
        {
            throw new NotImplementedException();
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
