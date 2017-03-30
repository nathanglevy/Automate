using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using AutomateTests.test.Controller;
using src.Model.GameWorldComponents;
using src.Model.MapModelComponents;

namespace Assets.src.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionHandler  : Handler<ObserverArgs>
    {
        public override IHandlerResult Handle(ObserverArgs args, IHandlerUtils utils)
        {
            ViewSelectionNotification notification = args as ViewSelectionNotification;

            List<MovableItem> coordinates =
                utils.Model.GetMovableListInBoundary(new Boundary(notification.UpperLeft, notification.BottomRight));

            List<MasterAction> actions = new List<MasterAction>();
            foreach (var movableItem in coordinates) 
            {
                var selectPlayer = new SelectPlayer(movableItem.CurrentCoordiate, movableItem.ToString());
                actions.Add(selectPlayer);
            }
            return new HandlerResult(actions);
        }
        
    }

    public class GamePlayer
    {
        public Coordinate Coordinate { get; set; }
        public string ID { get; set; }
    }

    public class SelectPlayer : MasterAction
    {
        public Coordinate Coordinate { get; private set; }

        public SelectPlayer(Coordinate coordinate, string targetID) : base(ActionType.SelectPlayer,targetID)
        {
            Coordinate = coordinate;
        }
    }
}