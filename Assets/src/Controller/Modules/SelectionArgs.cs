using Assets.src.Controller.Interfaces;
using Assets.src.Model.MapModelComponents;

namespace Assets.src.Controller.Modules
{
    public class SelectionArgs : IObserverArgs
    {
        public Coordinate UpperLeft { get; private set; }
        public Coordinate LowerRight { get; private set; }

        public SelectionArgs(Coordinate upperLeft, Coordinate lowerRight)
        {
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        public string Id { get; private set; }
    }
}