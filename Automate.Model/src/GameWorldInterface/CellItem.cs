using System;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldInterface
{
    public class CellItem : Item
    {
        private GameWorld _gameWorld;

        public CellItem(GameWorld gameWorld, Coordinate cellInfoCoordinate)
        {
            _gameWorld = gameWorld;
            Type = ItemType.Cell;
            Coordinate = cellInfoCoordinate;
            Guid = Guid.NewGuid();
        }

        public override Coordinate Coordinate { get; }


        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            CellItem item = (CellItem)obj;
            return (Guid == item.Guid && Coordinate == item.Coordinate);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }
    }
}