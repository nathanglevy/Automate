﻿using System;
using Mono.Security.Authenticode;
using src.Model.MapModelComponents;

namespace src.Model.GameWorldComponents
{
    public class Structure {
        //TODO: Need to implement this...
        private Coordinate coordinate;
        private Coordinate dimensions;
        private StructureType structureType;
        private Boundary boundary;
        public Guid Guid { get; private set; }

        public Structure(Coordinate coordinate, Coordinate dimensions, StructureType structureType) {
            this.coordinate = coordinate;
            this.dimensions = dimensions;
            this.structureType = structureType;
            this.boundary = new Boundary(coordinate, coordinate + dimensions);
            Guid = Guid.NewGuid();
        }

        public Boundary GetStructureBoundary()
        {
            return boundary;
        }
    }
}