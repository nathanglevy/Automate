using System;
using System.Collections.Generic;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace src.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class GameWorld
    {
        private Dictionary<Guid, Movable> _movables = new Dictionary<Guid, Movable>();
        private Dictionary<Guid, Structure> _structures = new Dictionary<Guid, Structure>();
        private MapInfo _map;

        public GameWorld(Coordinate mapDimensions)
        {
            if (mapDimensions == null)
                throw new ArgumentNullException();

            Coordinate bottomRight = mapDimensions - new Coordinate(1, 1, 1);
            Coordinate topLeft = new Coordinate(0, 0, 0);

            if (!(bottomRight > topLeft))
                throw new ArgumentException("dimensions must all be positive");

            _map = new MapInfo(topLeft, bottomRight);
            _map.FillMapWithCells(new CellInfo(true, 1));
        }

        public Guid CreateMovable(Coordinate coordinate)
        {
            Movable movable = new Movable(coordinate);
            _movables.Add(movable.GetId(),movable);
            return movable.GetId();
        }

        public bool IssueMoveCommand(Guid id, Coordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public Movement GetNextMovement(Guid id)
        {
            throw new NotImplementedException();
        }

        public Coordinate GetNextCoordinate(Guid id)
        {
            throw new NotImplementedException();
        }

        public Coordinate MoveToNext(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetMovableIdList()
        {
            return new List<Guid>(_movables.Keys);
        }
    }
}