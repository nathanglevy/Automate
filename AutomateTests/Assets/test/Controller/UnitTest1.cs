using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestRequirementHandler
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var handler = new RequirementHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestCanHandleCorrectArgs_ExpectTrue()
        {
            var handler = new RequirementHandler();
            Assert.IsTrue(handler.CanHandle(new RequirementWrapper(null, null)));
        }

        [TestMethod]
        public void TestCannotHandleInCorrectArgs_ExpectFalse()
        {
            var handler = new RequirementHandler();
            Assert.IsFalse(handler.CanHandle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCanHandleWithNull_ExpectAnException()
        {
            var handler = new RequirementHandler();
            handler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleIncorrectArgs_ExpectArgumentException()
        {
            var handler = new RequirementHandler();
            handler.Handle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid()),null);
        }

        [TestMethod]
        public void TestHandlePickUpRequirmentWhenTwoIdleMovables_expectTaskWithAssigneWithPickUpActionAsOutput()
        {
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movabe430 = gameWorld.CreateMovable(new Coordinate(4, 3, 0), MovableType.SimpleRobot);
            var movabe880 = gameWorld.CreateMovable(new Coordinate(8, 8, 0), MovableType.SimpleRobot);

            var cellItem = new CellItem(gameWorld, new Coordinate(3, 3, 0));
            cellItem.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.IronOre, 200));

            var componentPickupRequirement = new ComponentPickupRequirement(Component.IronOre, 200);
            var requirementWrapper = new RequirementWrapper(componentPickupRequirement,cellItem);

            var handler = new RequirementHandler();
            var handlerResult = handler.Handle(requirementWrapper,new HandlerUtils(gameWorld.Guid,null,null));

            Assert.AreEqual(1,handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal,handlerResult.GetItems()[0].Type);
            Assert.IsTrue(handlerResult.GetItems()[0] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[0] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe430.Guid,taskContainer.TargetTask.AssignedToGuid);
        }


        [TestMethod]
        public void TestHandlePickUpRequirmentWhenTwoIdleMovables_expectTaskWithAssigneWithPickUpActionAsOutput()
        {
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movabe430 = gameWorld.CreateMovable(new Coordinate(4, 3, 0), MovableType.SimpleRobot);
            var ironOre = movabe430.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);
            ironOre.StackMax = 30;

            var movabe880 = gameWorld.CreateMovable(new Coordinate(8, 8, 0), MovableType.SimpleRobot);

            var cellItem = new CellItem(gameWorld, new Coordinate(3, 3, 0));
            cellItem.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.IronOre, 200));

            var componentPickupRequirement = new ComponentPickupRequirement(Component.IronOre, 200);
            var requirementWrapper = new RequirementWrapper(componentPickupRequirement, cellItem);

            var handler = new RequirementHandler();
            var handlerResult = handler.Handle(requirementWrapper, new HandlerUtils(gameWorld.Guid, null, null));

            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.AreEqual(ActionType.Internal, handlerResult.GetItems()[0].Type);
            Assert.IsTrue(handlerResult.GetItems()[0] is TaskContainer);
            var taskContainer = handlerResult.GetItems()[0] as TaskContainer;
            Assert.IsTrue(taskContainer.TargetTask.IsAssigned);
            Assert.AreEqual(movabe430.Guid, taskContainer.TargetTask.AssignedToGuid);
        }





    }

    public class RequirementWrapper : ModelMasterAction
    {
        public ComponentPickupRequirement Requirement { get; }
        public Item HostingItem { get; }

        public RequirementWrapper(ComponentPickupRequirement requirement, Item hostingItem) : base(ActionType.Internal)
        {
            Requirement = requirement;
            HostingItem = hostingItem;
        }
    }

    public class RequirementHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException(
                    "only Args from the type RequirementWrapper can be passed to Handle Method");

            // Cast and Get Requirment
            var reqWrapper = args as RequirementWrapper;
            var req = reqWrapper.Requirement;

            // Get GameWorld
            var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            var tasks = new List<MasterAction>();
            switch (req.RequirementType)
            {
                case RequirementType.ComponentDelivery:
                    break;
                case RequirementType.ComponentPickup:
                    TaskContainer task = CreatePickUpTask(reqWrapper,gameWorld);
                    tasks.Add(task);
                    break;
                case RequirementType.Environment:
                    break;
                case RequirementType.Work:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new HandlerResult(tasks);
        }

        private TaskContainer CreatePickUpTask(RequirementWrapper req, IGameWorld gameWorld)
        {
            // create new Task
            var pickupTask = gameWorld.TaskDelegator.CreateNewTask();

            // Get/Add Component Stack Group at Dest
            var cmpntGrp = gameWorld.GetComponentStackGroupAtCoordinate(req.HostingItem.Coordinate);

            // Create Transport Action
            var pickUpTaskAction = pickupTask.AddTransportAction(TaskActionType.PickupTask, req.HostingItem.Coordinate, cmpntGrp,
                req.Requirement.Component, req.Requirement.Amount);
            
            // Link the Req to the Task
            req.Requirement.AttachAction(pickUpTaskAction);

            if (IsIdleMovablesExist(gameWorld))
            {
                gameWorld.TaskDelegator.AssignTask(FindClosetMovable(gameWorld,req.HostingItem.Coordinate),pickupTask);
            }
            return new TaskContainer(pickupTask);
        }

        private Guid FindClosetMovable(IGameWorld gameWorld,Coordinate targetDest)
        {

            var idleMovables = gameWorld.GetMovableIdList()
                .Where(p => !gameWorld.GetMovable(p).IsInMotion())
                .Select(gameWorld.GetMovable).ToList();

            var closestPath = gameWorld.GetMovementPathWithLowestCostToCoordinate(idleMovables.Select(p => p.Coordinate).ToList(), targetDest);
            var targteMovablemovable = idleMovables.First(p => p.Coordinate.Equals(closestPath.GetStartCoordinate()));
            return targteMovablemovable.Guid;
        }

        private bool IsIdleMovablesExist(IGameWorld gameWorld)
        {
            List<Coordinate> idleMovables = new List<Coordinate>();
            foreach (var movableId in gameWorld.GetMovableIdList())
            {
                if (!gameWorld.GetMovable(movableId).IsInMotion())
                    return true;
            }
            return false;
        }


        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("Args is null, cannot determine if Handler should be activated, dying");
            return args is RequirementWrapper;
        }
    }
}
