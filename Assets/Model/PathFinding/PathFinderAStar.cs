using System;
using System.Collections.Generic;
using Automate.Model.Utility;
using Model.MapModelComponents;

namespace Model.PathFinding
{
    public class PathFinderAStar : IPathFindingStrategy
    {
        /// <summary>
        /// Find shortest path algorithm, returns the shortest path between source and target
        /// Uses A* Search algorithm to find shortest path in a branching out shortest path tactic
        /// </summary>
        /// <exception cref="NoPathFoundException">Thrown if no path could be found</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if map or source/target coordinates are null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if source or target are out of map bounds</exception>
        /// <param name="mapInfo">mapInfo containing data on the weight of the cell</param>
        /// <param name="source">starting coordinate to begin path</param>
        /// <param name="target">ending coordinate to end path</param>
        /// <returns></returns>
        public static MovementPath FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target)
        {
            if (mapInfo == null || source == null || target == null)
                throw new ArgumentNullException();
            if (!(mapInfo.IsCoordinateIsWithinBounds(source) && mapInfo.IsCoordinateIsWithinBounds(target)))
                throw new ArgumentOutOfRangeException();

            MovementPath resultPath = new MovementPath(source);

            if (source == target)
                return resultPath;

            Dictionary<Coordinate,Movement> movementList = new Dictionary<Coordinate, Movement>();
            SortedList<double, Coordinate> toVisitList = new SortedList<double, Coordinate>(new DuplicateKeyComparer<double>());
            HashSet<Coordinate> visitedSet = new HashSet<Coordinate>();
            List<Coordinate> pathingMovements = GetPathingMovements();

            //start from target and work backwards
            toVisitList.Add(0, target);

            //while the to visit list is not empty
            while ((toVisitList.Count > 0) && !visitedSet.Contains(source))
            {
                Coordinate currentCoordinate = toVisitList.Values[0];
                double currentWeight = toVisitList.Keys[0];
                toVisitList.RemoveAt(0);
                //if current coordinate has already been visited, it is surely the shortest
                //path so we can skip it
                if (visitedSet.Contains(currentCoordinate))
                    continue;
                //add to visited list
                visitedSet.Add(currentCoordinate);

                foreach (var pathingMovement in pathingMovements)
                {
                    //coordinate to visit
                    Coordinate visitingCoordinate = pathingMovement + currentCoordinate;
                    //if is within bounds, we didnt visit it yet, is not about to be visited, and is passable
                    if (mapInfo.IsCoordinateIsWithinBounds(visitingCoordinate) &&
                        mapInfo.GetCell(visitingCoordinate).IsPassable() &&
                        !visitedSet.Contains(visitingCoordinate) &&
                        !toVisitList.ContainsValue(visitingCoordinate))
                    {
                        //add to the to visit list and movement list multiply by diagonal cost
                        double visitingWeight = mapInfo.GetCell(visitingCoordinate).GetWeight();
                        double currentCellWeight = mapInfo.GetCell(currentCoordinate).GetWeight();
                        double totalWeight = visitingWeight * 0.5F + currentCellWeight * 0.5F;
                        Movement newMove = new Movement(-pathingMovement, totalWeight);
                        movementList.Add(visitingCoordinate, newMove);

                        double newWeight = currentWeight + newMove.GetMoveCost();
                        toVisitList.Add(newWeight, visitingCoordinate);
                    }
                }
            }

            // no path -- throw an exception
            if (!visitedSet.Contains(source))
                throw new NoPathFoundException();

            //debug
            Console.Out.WriteLine("Now working backwards, starting at: " + resultPath.GetStartCoordinate().ToString());
            //build the return path
            while (resultPath.GetEndCoordinate() != target)
            {
                Console.Out.WriteLine("Moving by: " + movementList[resultPath.GetEndCoordinate()].GetMoveDirection().ToString());
                resultPath.AddMovement(movementList[resultPath.GetEndCoordinate()]);
                Console.Out.WriteLine("End is now: " + resultPath.GetEndCoordinate().ToString());

            }

            return resultPath;
        }

        private static List<Coordinate> GetPathingMovements()
        {
            List<Coordinate> result = new List<Coordinate>
            {
                new Coordinate(1, 0, 0),
                new Coordinate(1, 1, 0),
                new Coordinate(0, 1, 0),
                new Coordinate(-1, 0, 0),
                new Coordinate(-1, -1, 0),
                new Coordinate(0, -1, 0),
                new Coordinate(1, -1, 0),
                new Coordinate(-1, 1, 0)
            };

            return result;
        }

        MovementPath IPathFindingStrategy.FindShortestPath(MapInfo mapInfo, Coordinate source, Coordinate target)
        {
            return FindShortestPath(mapInfo, source, target);
        }

    }
}