using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.MoveHandler
{
    public class StartMoveActionHandler : Handler<IObserverArgs>
    {
        private double _multplier = 5;

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("Args type is incorrect, only StartMoveAction is allowed");

            // Cast to the correct type
            var startMoveAction = args as StartMoveAction;

            // Get the World
            var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            // Get the Movable
            var movableItem = gameWorld.GetMovable(startMoveAction.TargetId);


            var MovableAlreadyInMotion = movableItem.IsInMotion();
            // Default, Wasn't in motion and no in motion - let's create the first move
            // TODO: REMOVE THE ADDITION OF 1,0,0, IT"S JUST A WA FOR NOW
            var canMoveToTarget = movableItem.IssueMoveCommand(startMoveAction.To + new Coordinate(1,0,0));
            if (!canMoveToTarget)
                throw new Exception(String.Format("Cannot Move Movable: {0} to Target Destination at Coordinate: {1}",
                    movableItem.Guid, startMoveAction.To));

            // In Case Movable In Transition - nothing to do, the Next Move will get the correct Next Coordinate
            if (MovableAlreadyInMotion && movableItem.IsTransitioning)
                return new HandlerResult(new List<MasterAction>());

            // create First Move and Send To View
            var firstMoveAction = new MoveAction(movableItem.NextCoordinate, movableItem.CurrentCoordinate,
                movableItem.Guid)
            {
                Duration = new TimeSpan(0, 0, 0, 0, (int)(movableItem.NextMovementDuration * 1000 / _multplier)),
                NeedAcknowledge = true,
                OnCompleteDelegate = startMoveAction.OnCompleteDelegate,
                MasterTaskId = startMoveAction.MasterTaskId,
            };

            // Set Transition Start
            movableItem.StartTransitionToNext();

            // return first Move action
            return new HandlerResult(new List<MasterAction> { firstMoveAction });
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("ARgs is null cannot determine if Handler should be activated");
            if (args is StartMoveAction)
                return true;
            return false;
        }
    }
}