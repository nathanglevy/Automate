using Assets.src.Controller.Abstracts;
using Assets.src.Model.MapModelComponents;

namespace Assets.src.Controller.Handlers.RightClickNotification
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