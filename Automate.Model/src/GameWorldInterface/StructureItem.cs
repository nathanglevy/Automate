using System;
using System.Collections.Generic;
using Automate.Model.Components;
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

        //public Structure Structure => _gameWorld.GetStructure(Guid);

        public Dictionary<string, ComponentStack> GetInternalComponentStacks() {
            return _gameWorld.GetStructure(Guid).GetInternalComponentStacks();
        }

        public void AddNewStack(Component componentType, int amount) {
            _gameWorld.GetStructure(Guid).AddNewStack(componentType, amount);
        }

        public void RemoveStack(Component componentType) {
            _gameWorld.GetStructure(Guid).RemoveStack(componentType);
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