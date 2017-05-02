using System;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTaskActionHandler
    {
        private bool _onCompletePickup;

        [TestMethod]
        public void CreateNew_ShouldPass()
        {
            var taskHandler = new TaskActionHandler();
            Assert.IsNotNull(taskHandler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            var taskHandler = new TaskActionHandler();
            Assert.IsTrue(taskHandler.CanHandle(new TaskActionContainer(null, Guid.Empty)));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            var taskHandler = new TaskActionHandler();
            Assert.IsFalse(taskHandler.CanHandle(new MoveAction(null, null, Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNull_ExpectException()
        {
            var taskHandler = new TaskActionHandler();
            taskHandler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleWithIncorrectArgs_ExpectException()
        {
            var taskHandler = new TaskActionHandler();
            taskHandler.Handle(new MoveAction(null, null, Guid.Empty), null);
        }

        [TestMethod]
        public void TestHandlePickUp_ExpectGoAndPickUpTaskGenerated()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            ComponentStackGroup grp = gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(3, 3, 0));
            grp.AddComponentStack(ComponentType.IronOre, 0);

            var taskAction = newTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(3, 3, 0),grp,Component.IronOre , 30);
            var newGuid = Guid.NewGuid();
            gameWorldItem.TaskDelegator.AssignTask(newGuid,newTask);

            var taskActionHandler = new TaskActionHandler();
            var handlerResult = taskActionHandler.Handle(new TaskActionContainer(taskAction, newTask.Guid) {OnCompleteDelegate = OnCompleteCheck}, new HandlerUtils(gameWorldItem.Guid));

            Assert.AreEqual(1,handlerResult.GetItems().Count);

            Assert.IsTrue(condition: handlerResult.GetItems()[0] is GoAndPickUpAction);
            var goAndPickUpAction = handlerResult.GetItems()[0] as GoAndPickUpAction;
            Assert.AreEqual(newGuid,goAndPickUpAction.MovableGuid);
            Assert.AreEqual(OnCompleteCheck,goAndPickUpAction.OnCompleteDelegate);
        }

        private void OnCompleteCheck(ControllerNotificationArgs args)
        {
            _onCompletePickup = true;
        }

        [TestMethod]
        public void TestHandleDeliver_ExpectGoAndDeliverTaskGenerated()
        {

            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            var cmpntGrp = gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(3, 3, 0));
            var taskAction = newTask.AddTransportAction(TaskActionType.DeliveryTask, new Coordinate(3, 3, 0),cmpntGrp,Component.IronOre,  30);
            var newGuid = Guid.NewGuid();
            gameWorldItem.TaskDelegator.AssignTask(newGuid, newTask);

            var taskActionHandler = new TaskActionHandler();
            var handlerResult = taskActionHandler.Handle(new TaskActionContainer(taskAction, newTask.Guid) {OnCompleteDelegate = OnCompleteCheck}, new HandlerUtils(gameWorldItem.Guid));

            Assert.AreEqual(1, handlerResult.GetItems().Count);

            Assert.IsTrue(condition: handlerResult.GetItems()[0] is GoAndDeliverAction);
            var goAndDeliverAction = handlerResult.GetItems()[0] as GoAndDeliverAction;
            Assert.AreEqual(newGuid, goAndDeliverAction.MovableGuid);
            Assert.AreEqual(OnCompleteCheck, goAndDeliverAction.OnCompleteDelegate);

        }

    }
}
