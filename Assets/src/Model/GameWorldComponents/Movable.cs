using System;
using Assets.src.Utility;
using src.Model.MapModelComponents;
using src.Model.PathFinding;

namespace src.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class Movable
    {
        private Coordinate _currentCoordinate;
        private bool _inMotion;
        private MovementPath _movementPath = null;
        private Guid _id = Guid.NewGuid();

        public Movable(Coordinate startinCoordinate)
        {
            if (startinCoordinate == null)
                throw new ArgumentNullException();
            _inMotion = false;
            _currentCoordinate = startinCoordinate;
        }

        public bool IsInMotion()
        {
            return _inMotion;
        }

        public Movement GetNextMovement()
        {
            if (!_inMotion)
                return new Movement(0,0,0,0);
            return _movementPath.GetNextMovement(_currentCoordinate);
        }

        public Coordinate GetNextCoordinate()
        {
            if (!_inMotion)
                return _currentCoordinate;
            return _movementPath.GetNextCoordinate(_currentCoordinate);
        }

        public Movement MoveToNext()
        {
            if (!_inMotion)
                return new Movement(0,0,0,0);

            Movement nextMovement = GetNextMovement();
            _currentCoordinate = _currentCoordinate + nextMovement.GetMoveDirection();
            SetMotionStatus();

            return nextMovement;
        }

        private void SetMotionStatus()
        {
            _inMotion = !((_movementPath == null) || (_currentCoordinate == _movementPath.GetEndCoordinate()));
        }

        public void SetPath(MovementPath movementPath)
        {
            if (movementPath == null)
                throw new ArgumentNullException();
            _movementPath = new MovementPath(movementPath);
            SetMotionStatus();
        }

        public Guid GetId() { return _id; }
    }
}