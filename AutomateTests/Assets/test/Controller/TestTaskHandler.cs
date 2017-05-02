using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.TaskActionHandler;
using Automate.Controller.Handlers.TaskHandler;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTaskHandler
    {
        private bool _handleTaskActionFired;
        private bool _taskIsCompleteFired;
        private AutoResetEvent _onCompletefireSync = new AutoResetEvent(false);
        private AutoResetEvent _handleMimicSync = new AutoResetEvent(false);

        [TestMethod]
        public void CreateNew_ShouldPass()
        {
            var taskHandler = new TaskHandler();
            Assert.IsNotNull(taskHandler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            var taskHandler = new TaskHandler();
            Assert.IsTrue(taskHandler.CanHandle(new TaskContainer(null)));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            var taskHandler = new TaskHandler();
            Assert.IsFalse(taskHandler.CanHandle(new MoveAction(null,null,Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNull_ExpectException()
        {
            var taskHandler = new TaskHandler();
            taskHandler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleWithIncorrectArgs_ExpectException()
        {
            var taskHandler = new TaskHandler();
            taskHandler.Handle(new MoveAction(null, null, Guid.Empty), null);
        }

        //[TestMethod]
        public void TestHandle_ExpectOnComplete()
        {
            var taskHandler = new TaskHandler();

            // build model, targetTask and Action
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            var cmpntGrp = gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(0, 1, 0));
            newTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(0, 1, 0),cmpntGrp,Component.IronOre,  100);

            // Handle the TargetTask
            // Expects --> to Handle Current Action
            var taskContainer = new TaskContainer(newTask) {OnCompleteDelegate = TaskIsCompleted };
            var handlerResult = taskHandler.Handle(taskContainer,new HandlerUtils(gameWorldItem.Guid,HandleMimic,null));


            //_handleMimicSync.WaitOne(300);
            //Assert.IsTrue(_handleTaskActionFired);

            _onCompletefireSync.WaitOne();
            Assert.IsTrue(_taskIsCompleteFired);
        }


        private void TaskIsCompleted(ControllerNotificationArgs args)
        {
            _taskIsCompleteFired = true;
            _onCompletefireSync.Set();
        }

        private IList<ThreadInfo> HandleMimic(IObserverArgs args)
        {
            _handleTaskActionFired = args is TaskActionContainer;
            if (_handleTaskActionFired)
            {
                var taskActionContainer = args as TaskActionContainer;
                //taskActionContainer.TargetAction.
            }
            _handleMimicSync.Set();
            return null;
        }
    }
}
