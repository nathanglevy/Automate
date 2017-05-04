using System;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents {
    public interface IStructure {
        Boundary Boundary { get; }
        ComponentStackGroup ComponentStackGroup { get; }
        Coordinate Coordinate { get; }
        StructureJob CurrentJob { get; set; }
        Coordinate Dimensions { get; }
        Guid Guid { get; }
        bool HasActiveJob { get; }
        bool HasCompletedJob { get; }
        bool IsStructureComplete { get; }
        StructureType StructureType { get; }
    }
}