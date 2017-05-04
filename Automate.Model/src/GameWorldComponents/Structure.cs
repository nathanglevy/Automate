using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;
using JetBrains.Annotations;

namespace Automate.Model.GameWorldComponents
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
        public StructureJob CurrentJob { get; set; } = new StructureJob(JobType.Idle);
        public bool IsStructureComplete => ConstructionRequirements.HasIncompleteRequirements();
        public readonly RequirementContainer ConstructionRequirements = new RequirementContainer();

        internal Structure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            this.Coordinate = coordinate;
            this.Dimensions = dimensions;
            this.StructureType = structureType;
            this.Boundary = new Boundary(coordinate, coordinate + dimensions - new Coordinate(1,1,1));
        }
    }


}