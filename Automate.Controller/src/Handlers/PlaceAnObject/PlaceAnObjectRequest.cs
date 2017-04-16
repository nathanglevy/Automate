using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.PlaceAnObject
{
    public class PlaceAnObjectRequest : ObserverArgs, IObserverArgs
    {
        public ItemType ItemType { get; private set; }
        public Coordinate Coordinate { get; private set; }

        public PlaceAnObjectRequest(ItemType itemType, Coordinate coordinate)
        {
            ItemType = itemType;
            Coordinate = coordinate;
        }

    }
}