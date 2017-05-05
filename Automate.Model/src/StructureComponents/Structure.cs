using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;

namespace Automate.Model.StructureComponents
{
    public class Structure : Item, IStructure {
        //TODO: Need to implement this...
        public override ItemType ItemType { get; } = ItemType.Structure; 
        public override Coordinate Coordinate { get; }
        public Coordinate Dimensions { get; }
        public StructureType StructureType { get; }
        public Boundary Boundary { get; }
        public ComponentStackGroup ComponentStackGroup { get; } = new ComponentStackGroup();
        public bool HasActiveJob => !CurrentJob.JobType.Equals(JobType.Idle);
        public bool HasCompletedJob => HasActiveJob && CurrentJob.PointsOfWorkRemaining <= 0;
        public bool HasJobInProgress => HasActiveJob && CurrentJob.PointsOfWorkRemaining > 0;
        public RequirementJob CurrentJob { get; set; } = new RequirementJob(JobType.Idle);
        public bool IsStructureComplete { get; set; }
        public HashSet<StructureAttribute> StructureAttributes { get; } = new HashSet<StructureAttribute>();

        public void ClearCurrentJob() {
            CurrentJob = new RequirementJob(JobType.Idle);
        }

        internal Structure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            this.Coordinate = coordinate;
            this.Dimensions = dimensions;
            this.StructureType = structureType;
            this.Boundary = new Boundary(coordinate, coordinate + dimensions - new Coordinate(1,1,1));
        }
    }


}