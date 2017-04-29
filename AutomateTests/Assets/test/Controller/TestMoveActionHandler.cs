using System;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestMoveActionHandler
    {
        private bool _onMoveCompleteWasTriggered;

        [TestMethod]
        public void CreateNew_ShouldPass()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            Assert.IsNotNull(moveHandler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            Assert.IsTrue(moveHandler.CanHandle(new MoveAction(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), Guid.NewGuid())));
        }
        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            Assert.IsFalse(moveHandler.CanHandle(new RightClickNotification(new Coordinate(0, 0, 0))));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgsEspeciallStartMoveArgs_ExpectFalse()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            Assert.IsFalse(moveHandler.CanHandle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid())));
        }



        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullArgs_ExpectException()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            moveHandler.CanHandle(null);
        }

        [TestMethod]
        public void TestHandleMoveActionWhenPossible_ExpectToGetNewMoveCommandAsPartOfResult()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            Handler<IObserverArgs> startMoveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid,null,null);

            var handlerResult0 = startMoveHandler.Handle(new StartMoveAction(new Coordinate(4, 4, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult0.GetItems().Count);
            Assert.IsNotNull(handlerResult0.GetItems()[0] as MoveAction);
            var moveAction0 = handlerResult0.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction0.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction0.To);


            var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(3, 3, 0), movable.Coordinate, movable.Guid),utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MoveAction);
            var moveAction = handlerResult.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(2,2,0),moveAction.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(3,3,0),moveAction.To);

        }




     //   [TestMethod]
        public void TestHandleMoveActionToNonLegalLocation_ExpectToGetMoveErrorActionAsPartOfResult()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(4, 2, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.FastHuman);

            // Call the Model To Calculate a Path
            var sucess = movable.IssueMoveCommand(new Coordinate(3, 0, 0));
            Assert.IsTrue(sucess);

            // 3,0
            // 2,0 2,1
            //
            // 0.0
            gameWorld.CreateStructure(new Coordinate(2, 0, 0), new Coordinate(1, 1, 1), StructureType.Basic);
            gameWorld.CreateStructure(new Coordinate(2, 1, 0), new Coordinate(1, 1, 1), StructureType.Basic);


            //TODO: TALK WITH NAPH ON THIS CAPABILITY
            throw new NotImplementedException("NEED CAPABILITY TO CHECK TRUE/FALSE WHEN DO NEXT FOR MOVABLE");

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(1, 1, 0), new Coordinate(2, 2, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MovableErrorAction);
            var errorAction = handlerResult.GetItems()[0] as MovableErrorAction;
            Assert.AreEqual(ErrorType.NoPathForDestination, errorAction.Type);
            Assert.AreEqual(new Coordinate(2, 2, 0), errorAction.CurrentCoordinate);

        }

        [TestMethod]
        public void TestOnCompleteEvent_ExpectEventToBeFired()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(4, 2, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.FastHuman);

            // Call the Model To Calculate a Path
            var sucess = movable.IssueMoveCommand(new Coordinate(2, 0, 0));
            movable.StartTransitionToNext();
            Assert.IsTrue(sucess);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            var moveAction = new MoveAction(new Coordinate(0, 0, 0), new Coordinate(1, 0, 0), movable.Guid);

            moveAction.OnCompleteDelegate = OnMoveComplete;
            var handlerResult = moveHandler.Handle(moveAction, utils);

            var moveAction2 = handlerResult.GetItems()[0];
            var handlerResult2 = moveHandler.Handle(moveAction2, utils);

            var moveAction3 = handlerResult.GetItems()[0];
            var handlerResult3 = moveHandler.Handle(moveAction3, utils);

            // it means Delegate passed through all move commands and Fired after last one
            Assert.IsTrue(_onMoveCompleteWasTriggered);

        }

        private void OnMoveComplete(ControllerNotificationArgs args)
        {
            _onMoveCompleteWasTriggered = true;
        }
    }
}
