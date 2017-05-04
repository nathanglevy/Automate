using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestPickUpTaskHandler
    {
        private bool _pickUpOnCompleteFired;
        private IGameWorld _gameWorldItem;
        private IHandlerResult<MasterAction> _PickUpHandlerResult;
        private AutoResetEvent _pickupHandleSync;
        private AutoResetEvent _startMoveSync;
        private IHandlerResult<MasterAction> _startMoveResult;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            Handler<IObserverArgs> handler = new GoAndPickUpTaskHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            Handler<IObserverArgs> moveHandler = new GoAndPickUpTaskHandler();
            Assert.IsTrue(moveHandler.CanHandle(new GoAndPickUpAction(new Coordinate(0, 0, 0), 100, Guid.NewGuid())));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            Handler<IObserverArgs> moveHandler = new GoAndPickUpTaskHandler();
            Assert.IsFalse(moveHandler.CanHandle(new RightClickNotification(new Coordinate(0, 0, 0))));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullArgs_ExpectException()
        {
            Handler<IObserverArgs> moveHandler = new GoAndPickUpTaskHandler();
            moveHandler.CanHandle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleWithIncorrectArgs_ExpectArgumentException()
        {
            var pickUpTaskHandler = new GoAndPickUpTaskHandler();
            pickUpTaskHandler.Handle(new RightClickNotification(new Coordinate(0, 0, 0)), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleGoAndPickUpWhenMovableNotExist_ExpectNoMovableAssignedExceptoin()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var pickUpTaskHandler = new GoAndPickUpTaskHandler();
            pickUpTaskHandler.Handle(new GoAndPickUpAction(new Coordinate(0, 0, 0), 100, Guid.Empty),
                new HandlerUtils(gameWorldItem.Guid));
        }



        [TestMethod]
        public void TestCreatePickUpTaskAndHandleIt_ExpectMoveActionThenPickUpActions()
        {
            _gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(20, 20, 1));
            var movableItem = _gameWorldItem.CreateMovable(new Coordinate(3, 1, 0), MovableType.NormalHuman);
            ComponentStack componentsAtCoordinate = _gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(0, 0, 0))
                .AddComponentStack(Component.GetComponent(ComponentType.IronOre), 0);
            componentsAtCoordinate.AddAmount(100);
          //  componentsAtCoordinate.AssignOutgoingAmount(movableItem.Guid,99);
        

            var GoAndpickUpAction = new GoAndPickUpAction(new Coordinate(0, 0, 0), 50, movableItem.Guid);
            GoAndpickUpAction.OnCompleteDelegate = PickUpOnCompleteFired;

            var pickUpTaskHandler = new GoAndPickUpTaskHandler();
            var moveActionHandler = new MoveActionHandler();
            IHandlerUtils utils = new HandlerUtils(_gameWorldItem.Guid,HandlePickupAction,null);
            _startMoveSync = new AutoResetEvent(false);
            var startMoveActionRequestResult = pickUpTaskHandler.Handle(GoAndpickUpAction, utils);

            Assert.AreEqual(1, startMoveActionRequestResult.GetItems().Count);
            Assert.AreEqual(ActionType.Movement, startMoveActionRequestResult.GetItems()[0].Type);
            var startMoveAction = startMoveActionRequestResult.GetItems()[0] as StartMoveAction;
            Assert.IsNotNull(startMoveAction);
            Assert.AreEqual(new Coordinate(0, 0, 0), startMoveAction.To);

            // expectation
            // MoveAction (3,1,0) ==> (0,0,0) <-- Start Move Action
            // MoveAction (3,1,0) ==> (2,0,0)
            // MoveAction (2,0,0) ==> (1,0,0)
            // MoveAction (1,0,0) ==> (0,0,0)
            // MoveAction (0,0,0) ==> (0,0,0)
            // PickUpAction (0,0,0) @ 100
            var startMoveActionHandler = new StartMoveActionHandler();
            var result1 = startMoveActionHandler.Handle(startMoveAction, utils);
            Assert.AreEqual(1, result1.GetItems().Count);
            Assert.AreEqual(ActionType.Movement, result1.GetItems()[0].Type);
            var moveAction0 = result1.GetItems()[0] as MoveAction;
            Assert.IsNotNull(moveAction0);
            Assert.AreEqual(new Coordinate(2, 0, 0), moveAction0.To);
            Assert.AreEqual(new Coordinate(3, 1, 0), moveAction0.CurrentCoordiate);

            //// expect 
            //var result0 = moveActionHandler.Handle(moveAction0, utils);
            //Assert.AreEqual(1, result0.GetItems().Count);
            //Assert.AreEqual(ActionType.Movement, result0.GetItems()[0].Type);
            //var moveAction1 = result0.GetItems()[0] as MoveAction;
            //Assert.IsNotNull(moveAction1);
            //Assert.AreEqual(new Coordinate(2, 0, 0), moveAction1.To);
            //Assert.AreEqual(new Coordinate(3, 1, 0), moveAction1.CurrentCoordiate);

            var result2 = moveActionHandler.Handle(moveAction0, utils);
            Assert.AreEqual(1, result2.GetItems().Count);
            Assert.AreEqual(ActionType.Movement, result2.GetItems()[0].Type);
            var moveAction2 = result2.GetItems()[0] as MoveAction;
            Assert.IsNotNull(moveAction2);
            Assert.AreEqual(new Coordinate(1, 0, 0), moveAction2.To);
            Assert.AreEqual(new Coordinate(2, 0, 0), moveAction2.CurrentCoordiate);

            var result3 = moveActionHandler.Handle(moveAction2, utils);
            Assert.AreEqual(1, result3.GetItems().Count);
            Assert.AreEqual(ActionType.Movement, result3.GetItems()[0].Type);
            var moveAction3 = result3.GetItems()[0] as MoveAction;
            Assert.IsNotNull(moveAction3);
            Assert.AreEqual(new Coordinate(0, 0, 0), moveAction3.To);
            Assert.AreEqual(new Coordinate(1, 0, 0), moveAction3.CurrentCoordiate);


            var result4 = moveActionHandler.Handle(moveAction2, utils);
            Assert.AreEqual(1, result4.GetItems().Count);
            Assert.AreEqual(ActionType.Movement, result4.GetItems()[0].Type);
            var moveAction4 = result4.GetItems()[0] as MoveAction;
            Assert.IsNotNull(moveAction4);
            Assert.AreEqual(new Coordinate(0, 0, 0), moveAction4.To);
            Assert.AreEqual(new Coordinate(0, 0, 0), moveAction4.CurrentCoordiate);

            _pickupHandleSync = new AutoResetEvent(false);
            var resultNotRelvant = moveActionHandler.Handle(moveAction4, utils);
            _pickupHandleSync.WaitOne(300);
            var result5 = _PickUpHandlerResult;
            Assert.AreEqual(50, componentsAtCoordinate.CurrentAmount);
            Assert.IsTrue(_pickUpOnCompleteFired);
            Assert.AreEqual(50,movableItem.ComponentStackGroup.GetComponentStack(GoAndpickUpAction.ComponentType).CurrentAmount);

        }

        private IList<ThreadInfo> HandlePickupAction(IObserverArgs args)
        {
            var pickUpAction = args as PickUpAction;
            if (pickUpAction != null)
            {
                var pickUpActionHandler = new PickUpActionHandler();
                _PickUpHandlerResult = pickUpActionHandler.Handle(pickUpAction, new HandlerUtils(_gameWorldItem.Guid));
                _pickupHandleSync.Set();
                return null;
            }
            var startMoveAction = args as StartMoveAction;
            if (startMoveAction != null)
            {
                var moveActionHandler = new MoveActionHandler();
                _startMoveResult = moveActionHandler.Handle(startMoveAction, new HandlerUtils(_gameWorldItem.Guid));
                _startMoveSync.Set();
            }
            return null;

        }

        private void PickUpOnCompleteFired(ControllerNotificationArgs args)
        {
            _pickUpOnCompleteFired = true;
        }
    }
}
