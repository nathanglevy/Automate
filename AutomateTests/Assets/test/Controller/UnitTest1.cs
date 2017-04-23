using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Interfaces;
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
            Assert.IsTrue(taskHandler.CanHandle(new TaskContainer()));
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
            
        }
        

    }

    public class TaskContainer : IObserverArgs
    {
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
