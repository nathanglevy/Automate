using System;
using Automate.Model.Components;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Automate.Model.RuleComponents;

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
        float Speed { get; set; }

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
        IConditionGroup GetConditionsForJob(JobType jobType);
        void SetConditionsForJob(JobType jobType, IConditionGroup conditionGroup);
    }

    public partial interface IPlacable
    {
    }
}