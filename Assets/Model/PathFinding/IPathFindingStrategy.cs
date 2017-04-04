using Model.MapModelComponents;

namespace Model.PathFinding
{
    public interface IPathFindingStrategy
    {
        MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target);
    }
}