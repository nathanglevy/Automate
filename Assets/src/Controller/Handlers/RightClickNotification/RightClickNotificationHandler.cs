using System;
using System.Collections.Generic;
using System.Linq;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;

namespace Assets.src.Controller.Handlers.RightClickNotification
{
    public class RightClickNotificationHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
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
                masterActions.Add(new MoveAction(movable.CurrentCoordiate, movable.NextMovement.GetMoveDirection(), movable.Guid.ToString()));
            }
                
            return new HandlerResult(masterActions);
        }
    }

    public class MoveAction : MasterAction
    {
        public Coordinate From { get; private set; }
        public Coordinate To { get; private set; }

        public MoveAction(Coordinate @from, Coordinate to, string playerID) : base(ActionType.Movement,playerID)
        {
            From = @from;
            To = to;
        }
    }
}