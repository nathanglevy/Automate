using System;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldInterface
{
    public class StructureItem : Item
    {
        private readonly GameWorld _gameWorld;

        public override Coordinate Coordinate => StructureBoundary.topLeft;

        public Boundary StructureBoundary
        {
            get { return _gameWorld.GetStructure(Guid).GetStructureBoundary(); }
        }

        internal StructureItem(GameWorld gameWorld, Guid structureGuid) {
            Guid = structureGuid;
            Type = ItemType.Structure;
            _gameWorld = gameWorld;
        }

//        public override bool Equals(Object obj) {
//            // Check for null values and compare run-time types.
//            if (obj == null || GetType() != obj.GetType())
//                return false;
//
//
//            StructureItem movableItem = (StructureItem)obj;
//            return (Guid == movableItem.Guid);
//        }
//
//        public override int GetHashCode() {
//            return Guid.GetHashCode();
//        }
    }
}