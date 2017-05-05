using System;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;

namespace Automate.Model.StructureComponents {
    public interface IStructure : IJobContainer {
        Boundary Boundary { get; }
        ComponentStackGroup ComponentStackGroup { get; }
        Coordinate Coordinate { get; }
        Coordinate Dimensions { get; }
        Guid Guid { get; }
        bool IsStructureComplete { get; }
        StructureType StructureType { get; }
    }
}