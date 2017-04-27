using System;
using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
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
        public bool HasActiveJob => !CurrentJob.JobName.Equals("EMPTY_JOB");
        public bool HasCompletedJob => HasActiveJob && CurrentJob.PointsOfWorkDone >= CurrentJob.PointsOfWorkRequired;
        public StructureJob CurrentJob { get; set; } = new StructureJob("EMPTY_JOB",0);

//        [NotNull]
//        public StructureJob CurrentJob
//        {
//            get
//            {
//                if (!HasActiveJob)
//                    throw new ArgumentException("Cannot access job when there is no job!");
//                return _currentJob;
//            }
//            internal set
//            {
//                if (value == null) throw new ArgumentNullException(nameof(value));
//                _currentJob = value;
//            }
//        }

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
        public string JobName { get; }
        public int PointsOfWorkRequired { get; }
        public int PointsOfWorkDone { get; private set; }
        public int PercentageDone => 100*PointsOfWorkDone/PointsOfWorkRequired;

        internal StructureJob(string jobName, int pointsOfWorkRequired)
        {
            PointsOfWorkRequired = pointsOfWorkRequired;
            JobName = jobName;
        }

        public void AddPointsOfWorkDone(int amount)
        {
            PointsOfWorkDone += amount;
        }
    }
}