using Automate.Model.MapModelComponents;

namespace Automate.Model.PathFinding
{
    public interface IPathFindingStrategy
    {
        MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target);
    }
}