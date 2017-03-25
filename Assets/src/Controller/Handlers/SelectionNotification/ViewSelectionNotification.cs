using System;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using src.Model.MapModelComponents;

namespace Assets.src.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionNotification : ObserverArgs
    {
        public Coordinate UpperLeft { get; private set; }
        public Coordinate BottomRight { get; private set; }

        public ViewSelectionNotification(Coordinate UpperLeft, Coordinate bottomRight)
        {
            this.UpperLeft = UpperLeft;
            this.BottomRight = bottomRight;
        }

        public string TargetId { get; private set; }
    }
}