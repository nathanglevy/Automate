using System;
using Model.MapModelComponents;
using Model.PathFinding;

namespace Model.GameWorldComponents
{
    public class MovableItem : Item
    {
        private readonly GameWorld _gameWorld;

        public MovableItem(GameWorld gameWorld, Guid movableGuid) {
            Guid = movableGuid;
            Type = ItemType.Movable;
            _gameWorld = gameWorld;
        }


        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            MovableItem movableItem = (MovableItem)obj;
            return (Guid == movableItem.Guid);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }

        public Coordinate CurrentCoordiate
        {
            get { return _gameWorld.GetMovable(Guid).GetCurrentCoordinate(); }
        }

        public Movement NextMovement
        {
            get { return _gameWorld.GetMovable(Guid).GetNextMovement(); }
        }

        public Coordinate NextCoordinate
        {
            get { return _gameWorld.GetMovable(Guid).GetNextCoordinate(); }
        }

        public MovableType MovableType
        {
            get { return _gameWorld.GetMovable(Guid).MovableType; }
        }

        public double Speed
        {
            get { return _gameWorld.GetMovable(Guid).Speed; }
            set { _gameWorld.GetMovable(Guid).Speed = value; }
        }

        public bool IssueMoveCommand(Coordinate targetCoordinate)
        {
            return _gameWorld.IssueMoveCommand(Guid, targetCoordinate);
        }

        public bool IsInMotion()
        {
            return _gameWorld.GetMovable(Guid).IsInMotion();
        }

        public Movement MoveToNext()
        {
            return _gameWorld.GetMovable(Guid).MoveToNext();
        }

    }
}