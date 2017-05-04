using System;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;

namespace Automate.Model.Movables {
    public interface IMovable : IPlacable {
        ComponentStackGroup ComponentStackGroup { get; }
        Guid Guid { get; }
        Coordinate Coordinate { get; }
        Coordinate CurrentCoordinate { get; }
        Coordinate EffectiveCoordinate { get; }
        bool IsTransitioning { get; }
        MovableType MovableType { get; }
        Coordinate NextCoordinate { get; }
        Movement NextMovement { get; }
        double NextMovementDuration { get; }
        double Speed { get; set; }

        void DeliverToComponentStackGroup(ComponentStackGroup deliverToComponentStackGroup, Component component, int amount);
        bool Equals(object obj);
        object GetAccessLock();
        Coordinate GetFinalDestination();
        int GetHashCode();
        bool IsInMotion();
        bool IssueMoveCommand(Coordinate targetCoordinate);
        Movement MoveToNext();
        void PickupFromComponentStackGroup(ComponentStackGroup pickupFromComponentStackGroup, Component component, int amount);
        void StartTransitionToNext();
    }

    public partial interface IPlacable
    {
    }
}