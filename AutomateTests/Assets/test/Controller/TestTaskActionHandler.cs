using System;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTaskActionHandler
    {

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
            Assert.IsTrue(taskHandler.CanHandle(new TaskActionContainer(null)));
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
            var taskAction = newTask.AddAction(TaskActionType.PickupTask, new Coordinate(3, 3, 0), 30);

            var taskActionHandler = new TaskActionHandler();
            var handlerResult = taskActionHandler.Handle(new TaskActionContainer(taskAction), new HandlerUtils(gameWorldItem.Guid));

            Assert.AreEqual(1,handlerResult.GetItems().Count);
            Assert.IsTrue(condition: handlerResult.GetItems()[0] is GoAndPickUpTaskHandler);
        }
    }
}
