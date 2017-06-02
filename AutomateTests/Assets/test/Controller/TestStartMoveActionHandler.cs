using System;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.StructureComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestStartMoveActionHandler
    {
        private bool _onMoveCompleteWasTriggered;

        [TestMethod]
        public void CreateNew_ShouldPass()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            Assert.IsNotNull(moveHandler);
        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgs_ExpectTrue()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            Assert.IsTrue(moveHandler.CanHandle(new StartMoveAction(new Coordinate(0, 0, 0), Guid.NewGuid())));
        }
        [TestMethod]
        public void TestCanHandleWithInCorrectArgs_ExpectFalse()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            Assert.IsFalse(moveHandler.CanHandle(new MoveAction(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullArgs_ExpectException()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            moveHandler.CanHandle(null);
        }

        [TestMethod]
        public void TestStartMoveOfIdleMovable_ExpectFirstMoveActionInSched()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);

            var handlerResult0 = moveHandler.Handle(new StartMoveAction(new Coordinate(4, 4, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult0.GetItems().Count);
            Assert.IsNotNull(handlerResult0.GetItems()[0] as MoveAction);
            var moveAction0 = handlerResult0.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction0.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction0.To);
        }

        [TestMethod]
        public void TestStartMoveOfIdleMovableTowardsAStructure_ExpectFirstMoveActionInSched()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);
            gameWorld.CreateStructure(new Coordinate(4, 4, 0), new Coordinate(1, 1, 1), StructureType.SmallFire);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);

            var handlerResult0 = moveHandler.Handle(new StartMoveAction(new Coordinate(4, 4, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult0.GetItems().Count);
            Assert.IsNotNull(handlerResult0.GetItems()[0] as MoveAction);
            var moveAction0 = handlerResult0.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction0.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction0.To);
        }


        [TestMethod]
        public void TestStartMoveOfInTransitionMovable_ExpectFirstMoveActionInSched()
        {
            Handler<IObserverArgs> startMoveHandler = new StartMoveActionHandler();
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);


            // Start Route (1,1,0) --> (4,1,0)
            var handlerResult0 = startMoveHandler.Handle(new StartMoveAction(new Coordinate(4, 1, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult0.GetItems().Count);
            Assert.IsNotNull(handlerResult0.GetItems()[0] as MoveAction);
            var moveAction0 = handlerResult0.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction0.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 1, 0), moveAction0.To);

            // Change Route (4,1,0) --> (2,4,0)
            var handlerResult1 = startMoveHandler.Handle(new StartMoveAction(new Coordinate(2, 4, 0), movable.Guid), utils);
            Assert.AreEqual(0, handlerResult1.GetItems().Count);

            // Handle Move And Expect Movment to 2,2,0 instead of 3,1,0
            // Start Route (1,1,0) --> (4,1,0)
            var handlerResult2 = moveHandler.Handle(moveAction0, utils);
            Assert.AreEqual(1, handlerResult2.GetItems().Count);
            Assert.IsNotNull(handlerResult2.GetItems()[0] as MoveAction);
            var moveAction1 = handlerResult2.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(2, 1, 0), moveAction1.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(3, 2, 0), moveAction1.To);

        }


        [TestMethod]
        public void TestHandleMoveActionWhenPossible_ExpectToGetNewMoveCommandAsPartOfResult()
        {
            Handler<IObserverArgs> startmoveHandler = new StartMoveActionHandler();
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);

            var handlerResult0 = startmoveHandler.Handle(new StartMoveAction(new Coordinate(4, 4, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult0.GetItems().Count);
            Assert.IsNotNull(handlerResult0.GetItems()[0] as MoveAction);
            var moveAction0 = handlerResult0.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction0.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction0.To);


            var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(3, 3, 0), movable.Coordinate, movable.Guid), utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MoveAction);
            var moveAction = handlerResult.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(3, 3, 0), moveAction.To);

        }

      //  [TestMethod]
        public void TestHandleFirstMoveAction_ExpectFirstCoordinate()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            var handlerResult = moveHandler.Handle(new StartMoveAction(new Coordinate(4, 1, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MoveAction);
            var moveAction = handlerResult.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 1, 0), moveAction.To);

        }

       // [TestMethod]
        public void TestHandleFirstMoveActionWhenObjectInTransition_ExpectFirstCoordinate()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            var handlerResult = moveHandler.Handle(new StartMoveAction(new Coordinate(4, 1, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MoveAction);
            var moveAction = handlerResult.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(1, 1, 0), moveAction.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 1, 0), moveAction.To);

            var handlerResult2 = moveHandler.Handle(new StartMoveAction(new Coordinate(2, 4, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult2.GetItems().Count);
            Assert.IsNotNull(handlerResult2.GetItems()[0] as MoveAction);
            var moveAction2 = handlerResult2.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(2, 1, 0), moveAction2.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(2, 2, 0), moveAction2.To);

        }



        //   [TestMethod]
        public void TestHandleMoveActionToNonLegalLocation_ExpectToGetMoveErrorActionAsPartOfResult()
        {
            Handler<IObserverArgs> startMoveActionHandler = new StartMoveActionHandler();
            
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(4, 2, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.FastHuman);

            // Call the GameWorldGuid To Calculate a Path
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

            //IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            //var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(1, 1, 0), new Coordinate(2, 2, 0), movable.Guid), utils);
            //Assert.AreEqual(1, handlerResult.GetItems().Count);
            //Assert.IsNotNull(handlerResult.GetItems()[0] as MovableErrorAction);
            //var errorAction = handlerResult.GetItems()[0] as MovableErrorAction;
            //Assert.AreEqual(ErrorType.NoPathForDestination, errorAction.Type);
            //Assert.AreEqual(new Coordinate(2, 2, 0), errorAction.CurrentCoordinate);

        }

       //S [TestMethod]
        public void TestOnCompleteEvent_ExpectEventToBeFired()
        {
            Handler<IObserverArgs> moveHandler = new StartMoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(4, 2, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.FastHuman);

            // Call the GameWorldGuid To Calculate a Path
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
