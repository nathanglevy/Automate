using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using AutomateTests.Model.GameWorldComponents;
using AutomateTests.Model.GameWorldInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestPickUpTaskHandler
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            Handler<ObserverArgs> handler=new PickUpTaskHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestCreatePickUpTaskAndHandleIt_ExpectMoveAndPickUpActions()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            newTask.AddAction(TaskActionType.PickupTask,new Coordinate(0,0,0),100);
            var currentAction = newTask.GetCurrentAction();

            var taskActionHandler = new TaskActionHandler();
            var handlerResult = taskActionHandler.Handle(new TaskActionWrapper(currentAction),new HandlerUtils(gameWorldItem.Guid,null,null));

            Assert.IsNotNull(handlerResult);
            Assert.AreEqual(1,handlerResult.GetItems());

        }
    }

    public class TaskActionWrapper : ObserverArgs
    {
        public TaskAction TargetAction { get; }

        public TaskActionWrapper(TaskAction taskAction)
        {
            TargetAction = taskAction;
        }
    }

    public class TaskActionHandler : Handler<ObserverArgs>
    {

        public override bool CanHandle(ObserverArgs args)
        {
            throw new NotImplementedException();
        }

        public override IHandlerResult<MasterAction> Handle(ObserverArgs args, IHandlerUtils utils)
        {
            throw new NotImplementedException();
        }

    }

    public class PickUpTaskHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
        public override bool CanHandle(ObserverArgs args)
        {
            throw new NotImplementedException();
        }

        public override IHandlerResult<MasterAction> Handle(ObserverArgs args, IHandlerUtils utils)
        {
            throw new NotImplementedException();
        }

    }
}
