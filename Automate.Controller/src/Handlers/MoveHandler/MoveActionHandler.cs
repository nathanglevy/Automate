using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.MoveHandler
{
    public class MoveActionHandler : Handler<IObserverArgs>
    {
        private int _multplier = 5;

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("args is null, cannot determine if we can enable the handler");
            return (args is MoveAction) && !(args is StartMoveAction);
        }

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("Current Handler Can TimedOut only MoveAction");

            try
            {
                // get the move Action
                var moveAction = args as MoveAction;

                // get the movableItem from model
                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                var movableItem = gameWorldItem.GetMovableItem(moveAction.TargetId);

                // capture InMotion State before moving to Next
                var wasInMotion = movableItem.IsInMotion();

                // Move to Next to end current transition
                movableItem.MoveToNext();

                // Movable still in motion and need to feed it with Next Step
                if (movableItem.IsInMotion())
                {

                    var moveToNext = new MoveAction(movableItem.NextCoordinate, movableItem.EffectiveCoordinate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = true,
                        Duration = new TimeSpan(0, 0, 0, 0, (int)(movableItem.NextMovementDuration * 1000 / _multplier)),
                        OnCompleteDelegate = moveAction.OnCompleteDelegate,

                    };

                    // Start Transition
                    movableItem.StartTransitionToNext();

                    // Send Move to View
                    return new HandlerResult(new List<MasterAction>() {moveToNext});

                }
                // if Movable Was in Motion and Not Not --> it's the Last Move Command
                if (wasInMotion && !movableItem.IsInMotion()) 
                {
                    //  it's the Last Action
                    var moveToNext = new MoveAction(movableItem.CurrentCoordiate, movableItem.EffectiveCoordinate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = true,
                        Duration = new TimeSpan(0, 0, 0, 0),
                        OnCompleteDelegate = moveAction.OnCompleteDelegate,
                    };

                    var masterActions = new List<MasterAction>();
                    masterActions.Add(moveToNext);
                    var acknowledgeResult = new HandlerResult(masterActions);

                    return acknowledgeResult;
                }

                // If Target == Current -- Movable reached TARGET
                if (movableItem.CurrentCoordiate == moveAction.To)
                {
                    Console.Out.WriteLine(String.Format("Player {0} reached the Target - Good Job :-)",
                        movableItem.Guid.ToString()));

                    moveAction.FireOnComplete(new ControllerNotificationArgs(moveAction) {Utils = utils});
                    return new HandlerResult(new List<MasterAction>());
                }

                throw new Exception("Not Good, we shoudn;t reach here");
               

            }
            catch (Exception e)
            {
                Console.Out.Write("Cannot ACK to Move Object- " + e.Message);
                throw e;
            }
        }

    }
}