using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;

namespace Automate.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionHandler  : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            ViewSelectionNotification notification = args as ViewSelectionNotification;

            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            List<IMovable> selectedMovables = gameWorldItem.GetMovableListInBoundary(new Model.MapModelComponents.Boundary(notification.UpperLeft, notification.BottomRight));
            gameWorldItem.SelectMovableItems(selectedMovables);

            var selectedMovableItemList = gameWorldItem.GetSelectedMovableItemList();

            List<MasterAction> actions = new List<MasterAction>();
            foreach (var movableItem in selectedMovables) 
            {
                var selectPlayer = new SelectMovableAction(movableItem.CurrentCoordinate, movableItem.Guid.ToString());
                actions.Add(selectPlayer);
            }
            return new HandlerResult(actions);
        }


        public override bool CanHandle(IObserverArgs args)
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