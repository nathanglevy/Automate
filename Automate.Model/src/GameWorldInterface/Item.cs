using System;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldInterface
{
    public abstract class Item
    {
        public ItemType Type { get; protected set; }
        public Guid Guid { get; protected set; }
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