using System.Collections.Generic;
using Assets.src.Model.MapModelComponents;

namespace Assets.src.Model.PathFinding
{
    public interface IPathFindingStrategy
    {
        MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target);
    }
}