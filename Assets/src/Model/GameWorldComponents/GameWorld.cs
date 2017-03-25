using System;
using System.Collections.Generic;
using Assets.src.Model.GameWorldComponents;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace src.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class GameWorld
    {
        private Dictionary<long, Movable> _movables = new Dictionary<long, Movable>();
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

        public long CreateMovable(Coordinate coordinate)
        {
            Movable movable = new Movable(coordinate);
            _movables.Add(movable.getId(),movable);
            return movable.getId();
        }

        public bool IssueMoveCommand(long id, Coordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public Movement GetNextMovement(long id)
        {
            throw new NotImplementedException();
        }

        public Coordinate GetNextCoordinate(long id)
        {
            throw new NotImplementedException();
        }

        public Coordinate MoveToNext(long id)
        {
            throw new NotImplementedException();
        }

        public List<long> GetMovableIdList()
        {
            return new List<long>(_movables.Keys);
        }
    }
}