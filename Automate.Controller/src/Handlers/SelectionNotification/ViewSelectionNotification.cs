using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionNotification : ObserverArgs,IObserverArgs
    {
        public Coordinate UpperLeft { get; private set; }
        public Coordinate BottomRight { get; private set; }

        public ViewSelectionNotification(Coordinate UpperLeft, Coordinate bottomRight, string id):this(UpperLeft,bottomRight,new Guid(id)) { }
        public ViewSelectionNotification(Coordinate UpperLeft, Coordinate bottomRight, Guid id)
        {
            TargetId = id;
            this.UpperLeft = UpperLeft;
            this.BottomRight = bottomRight;
        }

        public Guid TargetId { get; }
    }
}