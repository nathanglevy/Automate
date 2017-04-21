using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Automate.Model.Tasks;

[assembly: InternalsVisibleTo("AutomateTests")]
namespace Automate.Model.GameWorldComponents {
    //TODO: Need to do comments!
    public class Movable
    {
        private Coordinate _currentCoordinate;
        private bool _inMotion;
        private bool _isTransitioning;
        private bool _isPendingNewPath;
        private MovementPath _pendingNewPath;
        private MovementPath _movementPath;
        private Guid _id = Guid.NewGuid();
        public MovableType MovableType { get; private set; }
        private double _speed;
        private List<Task> _taskList = new List<Task>();

        internal Movable(Coordinate startinCoordinate, MovableType movableType)
        {
            if (startinCoordinate == null)
                throw new ArgumentNullException();
            _inMotion = false;
            _currentCoordinate = startinCoordinate;
            MovableType = movableType;
            Speed = 1;
        }

        public double Speed {
            get { return _speed; }
            set {
                if (value <= 0)
                    throw new ArgumentException("cannot set speed below 0");
                _speed = value;
            }
        }

        public bool IsInMotion()
        {
            return _inMotion;
        }

        public bool IsTransitioning() {
            return _isTransitioning;
        }

        //intermediate state where the object is between current and next cell
        public void StartTransitionToNext() {
            if (_inMotion)
                _isTransitioning = true;
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

        public Coordinate GetCurrentCoordinate()
        {
            return _currentCoordinate;
        }

        public Coordinate GetEffectiveCoordinate()
        {
            return _isTransitioning ? GetNextCoordinate() : GetCurrentCoordinate();
        }

        public Movement MoveToNext()
        {
            _isTransitioning = false;
            if (!_inMotion)
                return new Movement(0,0,0,0);

            Movement nextMovement = GetNextMovement();
            _currentCoordinate = _currentCoordinate + nextMovement.GetMoveDirection();
            SetMotionStatus();
            if (_isPendingNewPath)
            {
                SetPendingPathAsActivePath();
            }

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
            _pendingNewPath = new MovementPath(movementPath);
            _isPendingNewPath = true;
            if (!_isTransitioning)
            {
                SetPendingPathAsActivePath();
            }
        }

        private void SetPendingPathAsActivePath()
        {
            _movementPath = _pendingNewPath;
            _isPendingNewPath = false;
            SetMotionStatus();
        }

        public Coordinate GetFinalDestination()
        {
            return (_inMotion) ? _movementPath.GetEndCoordinate() : _currentCoordinate;
            
        
        }

        public Guid GetId() { return _id; }


    }
}