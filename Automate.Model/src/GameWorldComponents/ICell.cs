using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents
{
    public interface ICell : IJobContainer {
        ItemType ItemType { get; }
        Coordinate Coordinate { get; }
        Guid Guid { get; }
        bool Equals(Object obj);
        int GetHashCode();
    }
}