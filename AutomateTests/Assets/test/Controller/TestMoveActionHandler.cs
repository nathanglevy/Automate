using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestMoveActionHandler
    {
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
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.FastHuman);

            // Call the Model To Calculate a Path
            var sucess = movable.IssueMoveCommand(new Coordinate(4, 4, 0));

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid,null,null);
            var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(1, 1, 0), new Coordinate(2, 2, 0), movable.Guid),utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MoveAction);
            var moveAction = handlerResult.GetItems()[0] as MoveAction;
            Assert.AreEqual(new Coordinate(2,2,0),moveAction.CurrentCoordiate);
            Assert.AreEqual(new Coordinate(3,3,0),moveAction.To);

        }


        [TestMethod]
        public void TestHandleMoveActionToNonPossible_ExpectToGetMoveErrorActionAsPartOfResult()
        {
            Handler<IObserverArgs> moveHandler = new MoveActionHandler();
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(4, 2, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(0, 0, 0), MovableType.FastHuman);

            // Call the Model To Calculate a Path
            var sucess = movable.IssueMoveCommand(new Coordinate(3, 0, 0));
            Assert.IsTrue(sucess);

            gameWorld.CreateStructure(new Coordinate(2, 0, 0), new Coordinate(1, 1, 1), StructureType.Basic);
            gameWorld.CreateStructure(new Coordinate(2, 1, 0), new Coordinate(1, 1, 1), StructureType.Basic);

            IHandlerUtils utils = new HandlerUtils(gameWorld.Guid, null, null);
            var handlerResult = moveHandler.Handle(new MoveAction(new Coordinate(1, 1, 0), new Coordinate(2, 2, 0), movable.Guid), utils);
            Assert.AreEqual(1, handlerResult.GetItems().Count);
            Assert.IsNotNull(handlerResult.GetItems()[0] as MovableErrorAction);
            var errorAction = handlerResult.GetItems()[0] as MovableErrorAction;
            Assert.AreEqual(ErrorType.NoPathForDestination, errorAction.Type);
            Assert.AreEqual(new Coordinate(2, 2, 0), errorAction.CurrentCoordinate);

        }


    }

    public class MovableErrorAction : MasterAction
    {
        public ErrorType Type { get; set; }
        public Coordinate CurrentCoordinate { get; set; }

        public MovableErrorAction(ActionType type, string targetId) : base(type, targetId)
        {
        }

        public MovableErrorAction(ActionType type, Guid targetId) : base(type, targetId)
        {
        }

        public MovableErrorAction(ActionType type) : base(type)
        {
        }
    }

    public enum ErrorType
    {
        NoPathForDestination
    }

    public class MoveActionHandler : Handler<IObserverArgs>
    {
        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("args is null, cannot determine if we can enable the handler");
            return args is MoveAction;
        }

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            //if (!CanHandle(args))
            //{
            //throw new ArgumentException("Handler can get only a MoveAction, args seems to have a different type: " + args.GetType().ToString());
            //}
            if (!CanHandle(args))
                throw new ArgumentException("Current Handler Can TimedOut only MoveAction");
            try
            {
                // get the move Action
                var moveAction = args as MoveAction;

                // get the movableItem from model
                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                var movableItem = gameWorldItem.GetMovableItem(moveAction.TargetId);
                var wasInMotion = movableItem.IsInMotion();
                var toNext = movableItem.MoveToNext();

                //                if (isInMotion)
                if (movableItem.IsInMotion())
                {

                    var moveToNext = new MoveAction(movableItem.NextCoordinate, movableItem.CurrentCoordiate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = true,
                        Duration = new TimeSpan(0, 0, 0, 0, (int)(movableItem.NextMovementDuration * 1000 / 5)),

                    };
                    //Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
                    //    movableItem.NextCoordinate));
                    movableItem.StartTransitionToNext();


                    var masterActions = new List<MasterAction>();
                    masterActions.Add(moveToNext);
                    var acknowledgeResult = new AcknowledgeResult(masterActions);

                    return acknowledgeResult;
                }
                else if (wasInMotion)
                {
                    var moveToNext = new MoveAction(movableItem.CurrentCoordiate, movableItem.CurrentCoordiate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = false,
                        Duration = new TimeSpan(0, 0, 0, 0, 0),
                    };
                    //Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
                    //    movableItem.CurrentCoordiate));
                    var masterActions = new List<MasterAction>();
                    masterActions.Add(moveToNext);
                    var acknowledgeResult = new AcknowledgeResult(masterActions);

                    return acknowledgeResult;
                }
                else
                {

                    Console.Out.WriteLine(String.Format("Player {0} reached the Target - Good Job :-)",
                        movableItem.Guid.ToString()));
                    return new AcknowledgeResult(new List<MasterAction>());
                }
            }
            catch (Exception e)
            {
                Console.Out.Write("Cannot ACK to Move Object- " + e.Message);
                throw e;
            }
        }

    }
}
