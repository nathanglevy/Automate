using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestTaskHandler
    {
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

        [TestMethod]
        public void TestHandle_ExpectOnComplete()
        {
            var taskHandler = new TaskHandler();

            // build model, targetTask and Action
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            newTask.AddAction(TaskActionType.PickupTask, new Coordinate(0, 1, 0), 100);

            // Handle the TargetTask
            // Expects --> to Handle Current Action
            var taskContainer = new TaskContainer(newTask);
            var handlerResult = taskHandler.Handle(taskContainer,new HandlerUtils(gameWorldItem.Guid,HandleMimic,null));


        }

        private IList<ThreadInfo> HandleMimic(IObserverArgs args)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskContainer : IObserverArgs
    {
        public Task TargetTask { get; }

        public TaskContainer(Task targetTask)
        {
            TargetTask = targetTask;
        }


        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;


    }

    public class TaskHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("Args is not expected, Handler Expects TaskContainer");

            return null;

        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is TaskContainer;
        }
    }
}
