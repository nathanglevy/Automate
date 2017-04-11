using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;

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

        public override IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        {
         if (!CanAcknowledge(action))
                throw new ArgumentException("Current Handler Can TimedOut only MoveAction");

         // get the move Action
            var moveAction = action as MoveAction;

            // get the movableItem from model
            
            var movableItem = utils.Model.GetMovableItem(new Guid(moveAction.TargetId));

            // get next step
            //            var toNext = movableItem.MoveToNext();

            //            var movableItemNextCoordinate = toNext.GetMoveDirection();
            
            var movableItemNextCoordinate = movableItem.NextCoordinate;

            movableItem.MoveToNext();
            //            movableItem.IssueMoveCommand(movableItemNextCoordinate);
            //            var issueMoveCommand = movableItem.IssueMoveCommand(movableItemNextCoordinate);
            //            if (!issueMoveCommand)
            //                throw new Exception("cannot move to next coordinate" + movableItemNextCoordinate);
            //            

            if (movableItemNextCoordinate != moveAction.To)
            {

                var moveToNext = new MoveAction(movableItemNextCoordinate, movableItem.Guid.ToString());
                var masterActions = new List<MasterAction>();
                   masterActions.Add(moveToNext);
                var acknowledgeResult = new AcknowledgeResult(masterActions);

                // instruct model to move to next
                //ovableItem.MoveToNext();

                return acknowledgeResult;
            }
            else
            {

                Console.Out.WriteLine(String.Format("Player {0} reached the Target - Good Job :-)",movableItem.Guid.ToString()));
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