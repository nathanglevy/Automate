using System;
using System.ComponentModel;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndDoSomething;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;
using AutomateTests.test.Controller;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestDeliverActionHandler
    {
        private bool _onCompleteFired;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var deliverActionHandler = new DeliverActionHandler();
            Assert.IsNotNull(deliverActionHandler);
        }

        [TestMethod]
        public void TestCreateDeliverUpAction_ShouldPass()
        {
            var deliverAction = new DeliverAction(new Coordinate(0, 0, 0), new Coordinate(1,0,0), 150, Guid.NewGuid());
            Assert.IsNotNull(deliverAction);
            Assert.AreEqual(new Coordinate(0,0,0),deliverAction.TargetDest);
            Assert.AreEqual(150,deliverAction.Amount);

        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgument_ExpectTrue()
        {
            var deliverActionHandler = new DeliverActionHandler();
            Assert.IsTrue(deliverActionHandler.CanHandle(new DeliverAction(new Coordinate(0,0,0), new Coordinate(1, 0, 0), 100, Guid.NewGuid())));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgument_ExpectTrue()
        {
            var deliverActionHandler = new DeliverActionHandler();
            Assert.IsFalse(deliverActionHandler.CanHandle(new MoveAction(new Coordinate(0, 0, 0), new Coordinate(0, 0, 0),Guid.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullExpectToFail()
        {
            var deliverActionHandler = new DeliverActionHandler();
            Assert.IsFalse(deliverActionHandler.CanHandle(null));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleNonCorrectAction_ExpectException()
        {
            var deliverActionHandler = new DeliverActionHandler();
            var handlerResult = deliverActionHandler.Handle(new MockMasterAction(ActionType.AreaSelection,Guid.NewGuid().ToString()),null);
        }
            
        [TestMethod]
        public void TestHandleDeliverAction_ExpectModelToBeUpdatedAndOnCompleteToBeFired()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 2, 0), MovableType.NormalHuman);
            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            gameWorldItem.TaskDelegator.AssignTask(movableItem.Guid,newTask);
            newTask.AddAction(TaskActionType.DeliveryTask, new Coordinate(0,0,0),100 );
            var currentAction = newTask.GetCurrentAction();

            gameWorldItem.AddComponentStack(new IronOreComponent(), new Coordinate(0, 0, 0), 100);
            var componentsAtCoordinate = gameWorldItem.GetComponentsAtCoordinate(new Coordinate(0, 0, 0));
            //componentsAtCoordinate.AddAmount(100);

            componentsAtCoordinate.AssignIncomingAmount(movableItem.Guid,70);
            

            var deliverAction = new DeliverAction(new Coordinate(0,0,0), new Coordinate(1, 0, 0), 70, movableItem.Guid)
            {
                MasterTaskId =  newTask.Guid,
                MovableId          = movableItem.Guid,
                OnCompleteDelegate = OnCompleteSniffer,
            };

            Assert.AreEqual(componentsAtCoordinate.CurrentAmount, 100);
            var deliverActionHandler = new DeliverActionHandler();
            var handlerResult = deliverActionHandler.Handle(deliverAction, new HandlerUtils(gameWorldItem.Guid));
            Assert.AreEqual(componentsAtCoordinate.CurrentAmount, 170);
            Assert.IsTrue(_onCompleteFired);

        }

        [TestMethod]
        public void TestAssignedAndDeliverdNotTheSame_ExpectException()
        {
            throw new NotImplementedException();
        }

        private void OnCompleteSniffer(ControllerNotificationArgs args)
        {
            _onCompleteFired = true;
        }
    }
}
