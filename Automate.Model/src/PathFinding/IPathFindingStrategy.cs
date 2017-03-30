using Automate.Model.src.MapModelComponents;

namespace Automate.Model.src.PathFinding
{
    public interface IPathFindingStrategy
    {
        MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target);
    }
}