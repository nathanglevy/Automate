using System;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;

namespace Automate.Model.CellComponents
{
    public interface ICell : IJobContainer {
        ItemType ItemType { get; }
        Coordinate Coordinate { get; }
        Guid Guid { get; }
        bool Equals(Object obj);
        int GetHashCode();
    }
}