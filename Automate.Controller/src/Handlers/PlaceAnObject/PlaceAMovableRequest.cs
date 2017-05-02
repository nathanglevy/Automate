using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;

namespace Automate.Controller.Handlers.PlaceAnObject
{
    public class PlaceAMovableRequest : PlaceAnObjectRequest
    {
        public MovableType MovableType { get; private set; }

        public PlaceAMovableRequest(Coordinate coordinate) : base(Model.GameWorldComponents.ItemType.Movable, coordinate)
        {
        }

        public PlaceAMovableRequest(Coordinate coordinate, MovableType movableType) : this(coordinate)
        {
            this.MovableType = movableType;
        }
    }
}