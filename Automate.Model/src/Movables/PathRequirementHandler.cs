using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.Movables
{

    public delegate bool PathRequirementHandler(object sender, PathRequirementArgs e);
    public class PathRequirementArgs : EventArgs {
        public Coordinate TargetCoordinate { get; }
        public Guid MovableGuid { get; }

        public PathRequirementArgs(Guid guid, Coordinate target) {
            TargetCoordinate = target;
            MovableGuid = guid;
        }
    }

}