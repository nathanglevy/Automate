using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.MoveHandler;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldComponents;
using Automate.Model.Movables;
using UnityEngine;

namespace Automate.Controller.Handlers.RightClockNotification
{
    public class RightClickNotificationHandler : Handler<IObserverArgs>, IHandler<IObserverArgs>
    {
        //Logger _logger = new Logger(new AutomateLogHandler());
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("Args Must Be from the Type RightClickNotfication, please make sure your use the CanHandle Method");
            }

            try
            {
                // Get the RightClock Object
                
                RightClickNotification rightNotification = args as RightClickNotification;
                //_logger.Log(LogType.Log, "HANDLE_RIGHT_CLICK", "Right Click Fired, Target: " + rightNotification.Coordinate);

                // Get All Selcted Objects
                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                List<IMovable> selectedMovables = gameWorldItem.GetSelectedMovableItemList();

                // iterate over selectable movables and create move actions
                var masterActions = new List<MasterAction>();
                foreach (var movable in selectedMovables)
                {
                    //Debug.Log(String.Format("Build New Path From to {0}", rightNotification.Coordinate));
                    var moveAction = new StartMoveAction(rightNotification.Coordinate,
                        movable.Guid);
                    masterActions.Add(moveAction);
                    //utils.InvokeHandler(moveAction);
                }

                return new HandlerResult(masterActions) {IsInternal = true};

            }
            catch (Exception e)
            {
                Console.Out.Write("Cannot Move Object- "  + e.Message);
                return null;
            }
        }

        public override bool CanHandle(IObserverArgs args)
        {
            return args is RightClickNotification;
        }

        //        public override IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        //        {
        //            if (!CanAcknowledge(action))
        //                throw new ArgumentException("Current Handler Can TimedOut only MoveAction");
        //            try
        //            {
        //                // get the move Action
        //                var moveAction = action as MoveAction;

        //                // get the movableItem from model
        //                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
        //                var movableItem = gameWorldItem.GetMovableItem(moveAction.TargetId);
        //                var wasInMotion = movableItem.IsInMotion();
        //                movableItem.MoveToNext();

        ////                if (isInMotion)
        //                if (movableItem.IsInMotion())
        //                {

        //                    var moveToNext = new MoveAction(movableItem.NextCoordinate, movableItem.CurrentCoordiate,
        //                        movableItem.Guid.ToString())
        //                    {
        //                        NeedAcknowledge = true,
        //                        Duration = new TimeSpan(0, 0, 0, 0, (int) (movableItem.NextMovementDuration * 1000/5)),

        //                    };
        //                    //Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
        //                    //    movableItem.NextCoordinate));
        //                    movableItem.StartTransitionToNext();


        //                    var masterActions = new List<MasterAction>();
        //                    masterActions.Add(moveToNext);
        //                    var acknowledgeResult = new AcknowledgeResult(masterActions);

        //                    return acknowledgeResult;
        //                }
        //                else if (wasInMotion)
        //                {
        //                    var moveToNext = new MoveAction(movableItem.CurrentCoordiate, movableItem.CurrentCoordiate,
        //                          movableItem.Guid.ToString())
        //                    {
        //                        NeedAcknowledge = false,
        //                        Duration = new TimeSpan(0, 0, 0, 0, 0),
        //                    };
        //                    //Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
        //                    //    movableItem.CurrentCoordiate));
        //                    var masterActions = new List<MasterAction>();
        //                    masterActions.Add(moveToNext);
        //                    var acknowledgeResult = new AcknowledgeResult(masterActions);

        //                    return acknowledgeResult;
        //                }
        //                else { 

        //                Console.Out.WriteLine(String.Format("Player {0} reached the Target - Good Job :-)",
        //                        movableItem.Guid.ToString()));
        //                    return new AcknowledgeResult(new List<MasterAction>());
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.Out.Write("Cannot ACK to Move Object- " + e.Message);
        //                throw e;
        //            }
        //        }

        //        public override bool CanAcknowledge(MasterAction action)
        //        {
        //            return action is MoveAction;
        //        }

    }
}