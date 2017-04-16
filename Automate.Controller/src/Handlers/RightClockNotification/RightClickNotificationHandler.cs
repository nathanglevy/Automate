using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using UnityEngine;

namespace Automate.Controller.Handlers.RightClockNotification
{
    public class RightClickNotificationHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
        bool IHandler.CanAcknowledge(MasterAction action)
        {
            return action is MoveAction;
        }
        public override IHandlerResult<MasterAction> Handle(ObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("Args Must Be from the Type RightClickNotfication, please make sure your use the CanHandle Method");
            }

            try
            {
                // Get the RightClock Object

                RightClickNotification rightNotification = args as RightClickNotification;

                // Get All Selcted Objects
                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                List<MovableItem> selectedMovables = gameWorldItem.GetSelectedMovableItemList();

                // iterate over selectable movables and create move actions
                var masterActions = new List<MasterAction>();
                foreach (var movable in selectedMovables)
                {
                    Debug.Log(String.Format("Build New Path From to {0}", rightNotification.Coordinate));

                    var isInMotion = movable.IsInMotion();
                    movable.IssueMoveCommand(rightNotification.Coordinate);

                    if (!isInMotion)
                    {
                        movable.StartTransitionToNext();
                        masterActions.Add(new MoveAction(movable.NextCoordinate, movable.CurrentCoordiate,
                            movable.Guid.ToString())
                        {
                            Duration = new TimeSpan(0, 0, 0, 0, (int) (movable.NextMovementDuration * 1000)),
                            NeedAcknowledge = true
                        });
                        Debug.Log(String.Format("New Path: Go From {0} to {1}", movable.CurrentCoordiate,
                            movable.NextCoordinate));
                    }
                }

                return new HandlerResult(masterActions);

            }
            catch (Exception e)
            {
                Console.Out.Write("Cannot Move Object- "  + e.Message);
                throw e;
            }
        }

        public override IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        {
            if (!CanAcknowledge(action))
                throw new ArgumentException("Current Handler Can TimedOut only MoveAction");
            try
            {
                // get the move Action
                var moveAction = action as MoveAction;

                // get the movableItem from model
                var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                var movableItem = gameWorldItem.GetMovableItem(new Guid(moveAction.TargetId));
                var isInMotion = movableItem.IsInMotion();
                movableItem.MoveToNext();

                if (isInMotion)
                {

                    var moveToNext = new MoveAction(movableItem.NextCoordinate, movableItem.CurrentCoordiate,
                        movableItem.Guid.ToString())
                    {
                        NeedAcknowledge = true,
                        Duration = new TimeSpan(0, 0, 0, 0, (int) (movableItem.NextMovementDuration * 1000)),

                    };
                    Debug.Log(String.Format("Ack From {0} to {1}", movableItem.CurrentCoordiate,
                        movableItem.NextCoordinate));
                    movableItem.StartTransitionToNext();


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

        public override bool CanAcknowledge(MasterAction action)
        {
            return action is MoveAction;
        }

        public override bool CanHandle(ObserverArgs args)
        {
            return args is RightClickNotification;
        }
    }
}