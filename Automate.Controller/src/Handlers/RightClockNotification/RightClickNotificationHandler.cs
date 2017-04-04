using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Model.GameWorldComponents;

namespace Automate.Controller.Handlers.RightClockNotification
{
    public class RightClickNotificationHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
        bool IHandler.CanAcknowledge(MasterAction action)
        {
            return action is MoveAction;
        }
        public override IHandlerResult Handle(ObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("Args Must Be from the Type RightClickNotfication, please make sure your use the CanHandle Method");
            }

            // Get the RightClock Object
            RightClickNotification rightNotification = args as RightClickNotification;

            // Get All Selcted Objects
            List<MovableItem> selectedMovables = utils.Model.GetSelectedMovableItemList();

            // iterate over selectable movables and create move actions
            var masterActions = new List<MasterAction>();
            foreach (var movable in selectedMovables)
            {
                movable.IssueMoveCommand(rightNotification.Coordinate);
                masterActions.Add(new MoveAction(movable.NextMovement.GetMoveDirection(), movable.Guid.ToString()));
            }
                
            return new HandlerResult(masterActions);
        }

        public override IAcknowledgeResult Acknowledge(MasterAction action, IHandlerUtils utils)
        {
         if (!CanAcknowledge(action))
                throw new ArgumentException("Current Handler Can Acknowledge only MoveAction");

         // get the move Action
            var moveAction = action as MoveAction;

            // get the movableItem from model
            var movableItem = utils.Model.GetMovableItem(Guid.Parse(moveAction.TargetId));

            // get next step
            var movableItemNextCoordinate = movableItem.NextCoordinate;

            if (movableItemNextCoordinate != moveAction.To)
            {
                var moveToNext = new MoveAction(movableItemNextCoordinate, movableItem.Guid.ToString());
                var masterActions = new List<MasterAction>();
                masterActions.Add(moveToNext);
                var acknowledgeResult = new AcknowledgeResult(masterActions);
                return acknowledgeResult;
            }
            else
            {
                // Current Coordinate and Next Coordinate -- no Move should Be Done, Stopping the Flow By Returnning Empty result
                return new AcknowledgeResult(new List<MasterAction>());
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