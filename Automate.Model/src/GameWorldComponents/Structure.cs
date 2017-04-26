using System;
using System.Collections.Generic;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Model.GameWorldComponents
{
    public class Structure {
        //TODO: Need to implement this...
        private Coordinate coordinate;
        private Coordinate dimensions;
        private StructureType structureType;
        private Boundary boundary;
        private Dictionary<string, ComponentStack> _internalComponentStacks = new Dictionary<string, ComponentStack>();
        private int[] array = new int[10];
        public Guid Guid { get; private set; }

        internal Structure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            this.coordinate = coordinate;
            this.dimensions = dimensions;
            this.structureType = structureType;
            this.boundary = new Boundary(coordinate, coordinate + dimensions - new Coordinate(1,1,1));
            Guid = Guid.NewGuid();
        }

        public Boundary GetStructureBoundary()
        {
            return boundary;
        }

        public Dictionary<string, ComponentStack> GetInternalComponentStacks()
        {
            return new Dictionary<string, ComponentStack>(_internalComponentStacks);
        }

        public void AddNewStack(Component componentType, int amount)
        {
            if (_internalComponentStacks.ContainsKey(componentType.Type))
                throw new ArgumentException("Stack already contains component of this type");
            _internalComponentStacks.Add(componentType.Type, new ComponentStack(componentType, amount));
        }

        public void RemoveStack(Component componentType) {
            _internalComponentStacks.Remove(componentType.Type);
        }
    }
}