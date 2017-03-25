using src.Model.MapModelComponents;

namespace src.Model.PathFinding
{
    public interface IPathFindingStrategy
    {
        MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target);
    }
}