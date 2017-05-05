using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;

[assembly: InternalsVisibleTo("AutomateTests")]
namespace Automate.Model.Movables {
    //TODO: Need to do comments!
    public class Movable : Item , IMovable
    {
        public Coordinate CurrentCoordinate { get; private set; }
        //private bool _inMotion;
        private bool _isTransitioning;
        private bool _isPendingNewPath;
        private MovementPath _pendingNewPath;
        private MovementPath _movementPath;
        private readonly Object AccessLock = new Object();
        private List<Task> _taskList = new List<Task>();
        public ComponentStackGroup ComponentStackGroup { get; } = new ComponentStackGroup();
        public override ItemType ItemType => ItemType.Movable;
        public bool PathToTargetHasBeenBroken { get; internal set; }
        public MovableType MovableType { get; private set; }
        public override Coordinate Coordinate => GetCurrentCoordinate();
        public Coordinate EffectiveCoordinate => GetEffectiveCoordinate();
        public Coordinate NextCoordinate => GetNextCoordinate();
        public Movement NextMovement => GetNextMovement();
        public double NextMovementDuration => GetNextMovement().GetMoveCost() / MovableCapabilities.MovementSpeed;
        public MovableCapabilities MovableCapabilities { get; } = new MovableCapabilities();

        public event PathRequirementHandler PathRequired;

        public object GetAccessLock() {
            return AccessLock;
        }

        public bool IssueMoveCommand(Coordinate targetCoordinate) {
            return PathRequired != null && PathRequired.Invoke(this, new PathRequirementArgs(Guid, targetCoordinate));
        }


        internal Movable(Coordinate startinCoordinate, MovableType movableType)
        {
            if (startinCoordinate == null)
                throw new ArgumentNullException();
            CurrentCoordinate = startinCoordinate;
            MovableType = movableType;
            Speed = 1;
        }

        public float Speed {
            get { return MovableCapabilities.MovementSpeed; }
            set {
                if (value <= 0)
                    throw new ArgumentException("cannot set speed below 0");
                MovableCapabilities.MovementSpeed = value;
            }
        }

        public bool IsInMotion()
        {
            lock (AccessLock)
                return !((_movementPath == null) || (CurrentCoordinate == _movementPath.GetEndCoordinate()));
        }

        public bool IsTransitioning {
            get {
                lock (AccessLock)
                    return _isTransitioning;
            }
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
                return IsInMotion() ? _movementPath.GetNextMovement(CurrentCoordinate) : new Movement(0, 0, 0, 0);
        }

        public Coordinate GetNextCoordinate()
        {
            lock (AccessLock)
                return !IsInMotion() ? CurrentCoordinate : _movementPath.GetNextCoordinate(CurrentCoordinate);
        }

        public Coordinate GetCurrentCoordinate()
        {
            lock (AccessLock)
                return CurrentCoordinate;
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
                CurrentCoordinate = CurrentCoordinate + nextMovement.GetMoveDirection();
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
                return (IsInMotion()) ? _movementPath.GetEndCoordinate() : CurrentCoordinate;
        }

        public Guid GetId() { return Guid; }

        public Object GetMovableAccessLock()
        {
            return AccessLock;
        }

        public void PickupFromComponentStackGroup(ComponentStackGroup pickupFromComponentStackGroup, Component component,
            int amount)
        {
            pickupFromComponentStackGroup.GetComponentStack(component).PickupAmount(Guid, amount);
            ComponentStackGroup.GetComponentStack(component).DeliverAmount(Guid,amount);
        }

        public void PickupFromComponentStackGroup(ComponentStackGroup pickupFromComponentStackGroup, ComponentType componentType,
            int amount)
        {
            PickupFromComponentStackGroup(pickupFromComponentStackGroup, Component.GetComponent(componentType), amount);
        }

        public void DeliverToComponentStackGroup(ComponentStackGroup deliverToComponentStackGroup, Component component,
            int amount) {
            ComponentStackGroup.GetComponentStack(component).PickupAmount(Guid, amount);
            deliverToComponentStackGroup.GetComponentStack(component).DeliverAmount(Guid, amount);
        }

        public void DeliverToComponentStackGroup(ComponentStackGroup deliverToComponentStackGroup, ComponentType componentType,
            int amount)
        {
            DeliverToComponentStackGroup(deliverToComponentStackGroup, Component.GetComponent(componentType), amount);
        }

    }
}