using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Requirements;
using JetBrains.Annotations;

namespace Automate.Model.GameWorldComponents
{
    public class Structure {
        //TODO: Need to implement this...
        public Coordinate Coordinate { get; }
        public Coordinate Dimensions { get; }
        public StructureType StructureType { get; }
        public Boundary Boundary { get; }
        public Guid Guid { get; private set; }
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
            Guid = Guid.NewGuid();
        }
    }

    public class StructureJob
    {
        public JobType JobType { get; }
        public int TotalPointsOfWorkRequired => JobRequirements.GetAllRequirements().Sum(item => item.TotalRequirement);
        public int PointsOfWorkDone => TotalPointsOfWorkRequired - PointsOfWorkRemaining;
        public int PointsOfWorkRemaining
            => JobRequirements.GetIncompleteRequirements().Sum(item => item.RequirementRemainingToSatisfy);
        public int PercentageDone => 100*PointsOfWorkDone / TotalPointsOfWorkRequired;
        public RequirementContainer JobRequirements { get; } = new RequirementContainer();

        public StructureJob(JobType jobType)
        {
            JobType = jobType;
        }

        public void AddRequirement(IRequirement requirement)
        {
            JobRequirements.AddRequirement(requirement);
        }
    }

    public enum JobType
    {
        Idle,
        Construction,
        Crafting,
        Research
    }
}