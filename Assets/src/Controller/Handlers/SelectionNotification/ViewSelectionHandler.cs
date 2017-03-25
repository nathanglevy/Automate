using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using AutomateTests.test.Controller;
using src.Model.MapModelComponents;

namespace Assets.src.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionHandler  : Handler<ObserverArgs>
    {
        public override void Handle(ObserverArgs args, IHandlerUtils utils)
        {
            ViewSelectionNotification notification = args as ViewSelectionNotification;

            List<Guid> coordinates = utils.Model.GetMovableListInBoundary(new Boundary(notification.UpperLeft, notification.BottomRight) );

            List<MasterAction> actions = new List<MasterAction>();
            foreach (var guid in coordinates)
            {
                Coordinate coordinate = utils.Model.GetMovableCurrentCoordinate(guid);
                var selectPlayer = new SelectPlayer(coordinate, guid.ToString());
                actions.Add(selectPlayer);
            }
            utils.Enqueue(new HandlerResult(actions));
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