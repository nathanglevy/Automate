using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Model.GameWorldComponents;
using Model.MapModelComponents;

namespace Automate.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionHandler  : Handler<ObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(ObserverArgs args, IHandlerUtils utils)
        {
            ViewSelectionNotification notification = args as ViewSelectionNotification;

            List<MovableItem> selectedMovables =
                utils.Model.GetMovableListInBoundary(new Boundary(notification.UpperLeft, notification.BottomRight));
            utils.Model.AddToSelectedMovableItems(selectedMovables);

            List<MasterAction> actions = new List<MasterAction>();
            foreach (var movableItem in selectedMovables) 
            {
                var selectPlayer = new SelectMovableAction(movableItem.CurrentCoordiate, movableItem.Guid.ToString());
                actions.Add(selectPlayer);
            }
            return new HandlerResult(actions);
        }

        public override IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        {
            return new AcknowledgeResult(new List<MasterAction>());
        }

        public override bool CanAcknowledge(MasterAction action)
        {
            return action is SelectMovableAction;
        }

        public override bool CanHandle(ObserverArgs args)
        {
            return args is ViewSelectionNotification;
        }
    }

    public class GamePlayer
    {
        public Coordinate Coordinate { get; set; }
        public string ID { get; set; }
    }
}