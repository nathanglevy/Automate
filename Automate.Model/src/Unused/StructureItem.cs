﻿//using System;
//using Automate.Model.MapModelComponents;
//
//namespace Automate.Model.GameWorldComponents
//{
//    [Obsolete]
//    public class StructureItem : Item
//    {
//        private readonly GameWorld _gameWorld;
//        public override Coordinate Coordinate => StructureBoundary.topLeft;
//        //public ComponentStackGroup ComponentStackGroup => _gameWorld.GetStructure(Guid).ComponentStackGroup;
//
//        public Boundary StructureBoundary => _gameWorld.GetStructure(Guid).Boundary;
//        public Structure Structure => _gameWorld.GetStructure(Guid);
//
//        internal StructureItem(GameWorld gameWorld, Guid structureGuid) {
//            Guid = structureGuid;
//            Type = ItemType.Structure;
//            _gameWorld = gameWorld;
//        }
//
//        public override ItemType Type => ItemType.Structure;
//
//
//        //public Structure Structure => _gameWorld.GetStructure(Guid);

//        public Dictionary<string, ComponentStack> GetInternalComponentStacks() {
//            return _gameWorld.GetStructure(Guid).GetInternalComponentStacks();
//        }

//        public void AddNewStack(Component componentType, int amount) {
//            _gameWorld.GetStructure(Guid).AddNewStack(componentType, amount);
//        }
//
//        public void RemoveStack(Component componentType) {
//            _gameWorld.GetStructure(Guid).RemoveStack(componentType);
//        }

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
//    }
//}

namespace Automate.Model.Unused
{
}