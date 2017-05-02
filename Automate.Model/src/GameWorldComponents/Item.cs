using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents
{
    public abstract class Item
    {
        public abstract ItemType ItemType { get; }
        public Guid Guid { get; } = Guid.NewGuid();
        public abstract Coordinate Coordinate { get; }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Item item = (Item)obj;
            return (Guid == item.Guid);
        }

        public override int GetHashCode() {
            return Guid.GetHashCode();
        }
    }



    public enum ItemType
    {
        Movable,
        Structure,
        Cell
    }
}