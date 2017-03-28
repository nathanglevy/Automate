using System;
using src.Model.MapModelComponents;

namespace src.Model.GameWorldComponents
{
    public class StructureItem : Item
    {
        private readonly GameWorld _gameWorld;

        public Boundary StructureBoundary
        {
            get { return _gameWorld.GetStructure(Guid).GetStructureBoundary(); }
        }

        public StructureItem(GameWorld gameWorld, Guid structureGuid) {
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