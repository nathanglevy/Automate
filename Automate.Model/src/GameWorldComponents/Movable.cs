using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Automate.Model.Tasks;

[assembly: InternalsVisibleTo("AutomateTests")]
namespace Automate.Model.GameWorldComponents {
    //TODO: Need to do comments!
    public class Movable
    {
        private Coordinate _currentCoordinate;
        //private bool _inMotion;
        private bool _isTransitioning;
        private bool _isPendingNewPath;
        private MovementPath _pendingNewPath;
        private MovementPath _movementPath;
        private Guid _id = Guid.NewGuid();
        public bool PathToTargetHasBeenBroken { get; internal set; } = false;
        public MovableType MovableType { get; private set; }
        private double _speed;
        private readonly Object AccessLock = new Object();
        private List<Task> _taskList = new List<Task>();
        public ComponentStackGroup ComponentStackGroup { get; } = new ComponentStackGroup(1000,1000);

        internal Movable(Coordinate startinCoordinate, MovableType movableType)
        {
            if (startinCoordinate == null)
                throw new ArgumentNullException();
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
            lock (AccessLock)
                return !((_movementPath == null) || (_currentCoordinate == _movementPath.GetEndCoordinate()));
        }

        public bool IsTransitioning() {
            lock (AccessLock)
                return _isTransitioning;
        }

        //intermediate state where the object is between current and next cell
        public void StartTransitionToNext() {
            lock (AccessLock)
                if (IsInMotion())
                    _isTransitioning = true;
        }

        public Movement GetNextMovement()
        {
            lock (AccessLock)
                return IsInMotion() ? _movementPath.GetNextMovement(_currentCoordinate) : new Movement(0, 0, 0, 0);
        }

        public Coordinate GetNextCoordinate()
        {
            lock (AccessLock)
                return !IsInMotion() ? _currentCoordinate : _movementPath.GetNextCoordinate(_currentCoordinate);
        }

        public Coordinate GetCurrentCoordinate()
        {
            lock (AccessLock)
                return _currentCoordinate;
        }

        public Coordinate GetEffectiveCoordinate()
        {
            lock (AccessLock)
                return _isTransitioning ? GetNextCoordinate() : GetCurrentCoordinate();
        }

        public Movement MoveToNext()
        {
            lock (AccessLock)
            {
                _isTransitioning = false;
                if (!IsInMotion())
                    return new Movement(0, 0, 0, 0);

                Movement nextMovement = GetNextMovement();
                _currentCoordinate = _currentCoordinate + nextMovement.GetMoveDirection();
                //SetMotionStatus();
                if (_isPendingNewPath)
                {
                    SetPendingPathAsActivePath();
                }

                return nextMovement;
            }
        }

//        private void SetMotionStatus()
//        {
//            _inMotion = !((_movementPath == null) || (_currentCoordinate == _movementPath.GetEndCoordinate()));
//        }

        public void SetPath(MovementPath movementPath)
        {
            lock (AccessLock)
            {
                if (movementPath == null)
                    throw new ArgumentNullException();
                _pendingNewPath = new MovementPath(movementPath);
                _isPendingNewPath = true;
                PathToTargetHasBeenBroken = false;
                if (!_isTransitioning)
                {
                    SetPendingPathAsActivePath();
                }
            }
        }

        private void SetPendingPathAsActivePath()
        {
            _movementPath = _pendingNewPath;
            _isPendingNewPath = false;
            //SetMotionStatus();
        }

        public Coordinate GetFinalDestination()
        {
            lock (AccessLock)
                return (IsInMotion()) ? _movementPath.GetEndCoordinate() : _currentCoordinate;
        }

        public Guid GetId() { return _id; }

        public Object GetMovableAccessLock()
        {
            return AccessLock;
        }

        public void PickupFromComponentStackGroup(ComponentStackGroup pickupFromComponentStackGroup, Component component,
            int amount)
        {
            pickupFromComponentStackGroup.GetComponentStack(component).PickupAmount(_id, amount);
            ComponentStackGroup.GetComponentStack(component).DeliverAmount(_id,amount);
        }

        public void PickupFromComponentStackGroup(ComponentStackGroup pickupFromComponentStackGroup, ComponentType componentType,
            int amount)
        {
            PickupFromComponentStackGroup(pickupFromComponentStackGroup, Component.GetComponent(componentType), amount);
        }

        public void DeliverToComponentStackGroup(ComponentStackGroup deliverToComponentStackGroup, Component component,
            int amount) {
            ComponentStackGroup.GetComponentStack(component).PickupAmount(_id, amount);
            deliverToComponentStackGroup.GetComponentStack(component).DeliverAmount(_id, amount);
        }

        public void DeliverToComponentStackGroup(ComponentStackGroup deliverToComponentStackGroup, ComponentType componentType,
            int amount)
        {
            DeliverToComponentStackGroup(deliverToComponentStackGroup, Component.GetComponent(componentType), amount);
        }
    }
}