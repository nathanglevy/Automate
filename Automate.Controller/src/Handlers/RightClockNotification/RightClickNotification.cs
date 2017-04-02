using Automate.Controller.Abstracts;
using Automate.Model.src.MapModelComponents;

namespace Automate.Controller.Handlers.RightClockNotification
{
    public class RightClickNotification : ObserverArgs
    {
        public Coordinate Coordinate { get; private set; }

        public RightClickNotification(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}