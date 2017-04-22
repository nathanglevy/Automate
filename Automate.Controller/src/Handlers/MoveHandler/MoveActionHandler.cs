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
                        OnCompleteDelegate = moveAction.OnCompleteDelegate,

                    };

                    //Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
                    //    movableItem.NextCoordinate));
                    movableItem.StartTransitionToNext();


                    var masterActions = new List<MasterAction>();
                    masterActions.Add(moveToNext);
                    var acknowledgeResult = new HandlerResult(masterActions);

                    return acknowledgeResult;
                } else if (wasInMotion)
                {
                    var moveToNext = new MoveAction(movableItem.CurrentCoordiate, movableItem.CurrentCoordiate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = true,
                        //Duration = new TimeSpan(0, 0, 0, 0, ),
                        Duration = new TimeSpan(0, 0, 0, 0, (int)(movableItem.NextMovementDuration * 1000 / 5)),
                        OnCompleteDelegate = moveAction.OnCompleteDelegate,
                    };

                    var masterActions = new List<MasterAction>();
                    masterActions.Add(moveToNext);
                    var acknowledgeResult = new HandlerResult(masterActions);

                    return acknowledgeResult;
                } else if (movableItem.CurrentCoordiate == moveAction.To)
                {
                    Console.Out.WriteLine(String.Format("Player {0} reached the Target - Good Job :-)",
                        movableItem.Guid.ToString()));

                    moveAction.FireOnComplete(new ControllerNotificationArgs(moveAction) {Utils = utils});
                    return new HandlerResult(new List<MasterAction>());
                }

                // We Should Start Move Action series to Target
                movableItem.IssueMoveCommand(moveAction.To);

                movableItem.StartTransitionToNext();
                var FirstMoveAction = new MoveAction(movableItem.NextCoordinate, movableItem.CurrentCoordiate,
                    movableItem.Guid)
                {
                    Duration = new TimeSpan(0, 0, 0, 0, (int)(movableItem.NextMovementDuration * 1000 / 5)),
                    NeedAcknowledge = true,
                    OnCompleteDelegate = moveAction.OnCompleteDelegate
                };
                return new HandlerResult(new List<MasterAction> { FirstMoveAction });

            }
            catch (Exception e)
            {
                Console.Out.Write("Cannot ACK to Move Object- " + e.Message);
                throw e;
            }
        }

    }
}