using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents
{
    public class CellItem : Item, ICell
    {
        private IGameWorld _gameWorld;
        public override ItemType ItemType { get; } = ItemType.Cell;
        public bool HasActiveJob => !CurrentJob.JobType.Equals(JobType.Idle);
        public bool HasCompletedJob => HasActiveJob && CurrentJob.PointsOfWorkRemaining <= 0;
        public bool HasJobInProgress => HasActiveJob && CurrentJob.PointsOfWorkRemaining > 0;
        public RequirementJob CurrentJob { get; set; } = new RequirementJob(JobType.Idle);

        public CellItem(IGameWorld gameWorld, Coordinate cellInfoCoordinate)
        {
            _gameWorld = gameWorld;
            Coordinate = cellInfoCoordinate;
        }

        public override Coordinate Coordinate { get; }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            CellItem item = (CellItem)obj;
            return (Guid == item.Guid && Coordinate == item.Coordinate);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }
    }
}