using System;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;
using Assets.src.Utility;

namespace Assets.src.Model.GameWorldComponents
{
    //TODO: Need to do comments!
    public class Movable
    {
        private Coordinate _currentCoordinate;
        private bool _inMotion;
        private MovementPath _movementPath = null;
        private long _id = CommonUtility.GetUid();

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

        public long getId() { return _id; }
    }
}