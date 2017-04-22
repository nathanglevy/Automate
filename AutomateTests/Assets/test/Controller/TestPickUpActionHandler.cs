using System;
using System.ComponentModel;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestPickUpActionHandler
    {
        private bool _onCompleteFired;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsNotNull(pickUpActionHandler);
        }

        [TestMethod]
        public void TestCreateNewPickUpAction_ShouldPass()
        {
            var pickUpAction = new PickUpAction(new Coordinate(0, 0, 0), 150);
            Assert.IsNotNull(pickUpAction);
            Assert.AreEqual(new Coordinate(0,0,0),pickUpAction.TargetDest);
            Assert.AreEqual(150,pickUpAction.Amount);

        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgument_ExpectTrue()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsTrue(pickUpActionHandler.CanHandle(new PickUpAction(new Coordinate(0,0,0),100)));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgument_ExpectTrue()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsFalse(pickUpActionHandler.CanHandle(new MoveAction(new Coordinate(0, 0, 0), new Coordinate(0, 0, 0),Guid.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullExpectToFail()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsFalse(pickUpActionHandler.CanHandle(null));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleNonCorrectAction_ExpectException()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            var handlerResult = pickUpActionHandler.Handle(new MockMasterAction(ActionType.AreaSelection,Guid.NewGuid().ToString()),null);
        }
            
        [TestMethod]
        public void TestHandlePickUpAction_ExpectModelToBeUpdatedAndOnCompleteToBeFired()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 2, 0), MovableType.NormalHuman);
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            gameWorldItem.TaskDelegator.AssignTask(movableItem.Guid,newTask);
            newTask.AddAction(TaskActionType.PickupTask, new Coordinate(0,0,0),100 );
            var currentAction = newTask.GetCurrentAction();

            gameWorldItem.AddComponentStack(new IronOreComponent(), new Coordinate(0, 0, 0), 0);
            var componentsAtCoordinate = gameWorldItem.GetComponentsAtCoordinate(new Coordinate(0, 0, 0));
            
            componentsAtCoordinate.AddAmount(100);

            var pickUpAction = new PickUpAction(new Coordinate(0,0,0),70 )
            {
                MasterTaskId =  newTask.Guid,
                MovableId          = movableItem.Guid,
                OnCompleteDelegate = OnCompleteSniffer,
            };

            Assert.AreEqual(componentsAtCoordinate.Amount, 100);
            var pickUpActionHandler = new PickUpActionHandler();
            var handlerResult = pickUpActionHandler.Handle(pickUpAction, new HandlerUtils(gameWorldItem.Guid));
            Assert.AreEqual(componentsAtCoordinate.Amount,30);
            Assert.IsTrue(_onCompleteFired);

        }

        private void OnCompleteSniffer(ControllerNotificationArgs args)
        {
            _onCompleteFired = true;
        }
    }
}
