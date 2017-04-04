using System;
using Model.MapModelComponents;

namespace Model.GameWorldComponents
{
    public class CellItem : Item
    {
        private GameWorld _gameWorld;
        private Coordinate _cellInfoCoordinate;

        public CellItem(GameWorld gameWorld, Coordinate cellInfoCoordinate)
        {
            _gameWorld = gameWorld;
            Type = ItemType.Cell;
            _cellInfoCoordinate = cellInfoCoordinate;
            Guid = Guid.NewGuid();
        }

        public new Guid Guid
        {
            //get { return _gameWorld.GetCellInfo(_cellInfoCoordinate).Guid; }
            get; private set; }

        public Coordinate Coordinate { get { return _cellInfoCoordinate; } }


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