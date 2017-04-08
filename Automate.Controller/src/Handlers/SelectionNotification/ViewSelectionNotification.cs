using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.SelectionNotification
{
    public class ViewSelectionNotification : ObserverArgs
    {
        public Coordinate UpperLeft { get; private set; }
        public Coordinate BottomRight { get; private set; }

        public ViewSelectionNotification(Coordinate UpperLeft, Coordinate bottomRight, string id)
        {
            TargetId = id;
            this.UpperLeft = UpperLeft;
            this.BottomRight = bottomRight;
        }

    }
}