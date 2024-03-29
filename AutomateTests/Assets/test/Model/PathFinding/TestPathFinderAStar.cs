﻿using System;
using System.Collections.Generic;
using Automate.Model.MapModelComponents;
using Automate.Model.PathFinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Model.PathFinding {
    [TestClass()]
    public class PathFinderAStarTests {

        [TestMethod()]
        public void TestFindShortestPath_SameSourceTarget_ExpectEmptyList()
        {
            MapInfo mapInfo = new MapInfo(10,10,1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(0, 0, 0));
            Assert.AreEqual(result.GetStartCoordinate(),result.GetEndCoordinate());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestFindShortestPath_OutOfRangeSource_ExpectArgumentOutOfRangeException() {
            MapInfo mapInfo = new MapInfo(10, 10, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(11, 11, 0), new Coordinate(0, 0, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestFindShortestPath_OutOfRangeTarget_ExpectArgumentOutOfRangeException() {
            MapInfo mapInfo = new MapInfo(10, 10, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(11, 11, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFindShortestPath_NullMap_ExpectArgumentNullException() {
            PathFinderAStar.FindShortestPath(null, new Coordinate(0, 0, 0), new Coordinate(11, 11, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFindShortestPath_NullSource_ExpectArgumentNullException() {
            MapInfo mapInfo = new MapInfo(10, 10, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(1, 1, 0), null);
        }

        [TestMethod()]
        public void TestFindShortestPath_SimplePath_ExpectCorrentEndCoordinate() {
            MapInfo mapInfo = new MapInfo(10, 10, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(1, 1, 0));
            Assert.AreEqual(result.GetEndCoordinate(), new Coordinate(1, 1, 0));
            Assert.AreEqual(result.GetNextCoordinate(new Coordinate(0, 0, 0)), new Coordinate(1, 1, 0));
            Assert.AreEqual(result.GetNextMovement(new Coordinate(0, 0, 0)), new Movement(new Coordinate(1, 1, 0), 1));
        }

        [TestMethod()]
        public void TestFindShortestPath_LongerPath_ExpectCorrentEndCoordinate() {
            MapInfo mapInfo = new MapInfo(10, 10, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(5, 2, 0));
            Assert.AreEqual(result.GetEndCoordinate(), new Coordinate(5, 2, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoPathFoundException))]
        public void TestFindShortestPath_NoPossiblePath_ExpectNoPathFoundException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(5, 2, 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoPathFoundException))]
        public void TestFindShortestPath_TargetIsNotAccessible_ExpectNoPathFoundException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0,0,0), new CellInfo(false, 1) );
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(3, 3, 0), new Coordinate(0, 0, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(NoPathFoundException))]
        public void TestFindShortestPath_SourceIsNotAccessible_ExpectNoPathFoundException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 0, 0), new CellInfo(false, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(3, 3, 0));
        }

        [TestMethod()]
        public void TestFindShortestPath_ToABoundary_ExpectSuccess() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(2, 2, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(2, 3, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(3, 2, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(3, 3, 0), new CellInfo(false, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new List<Coordinate>()
            { new Coordinate(0,0,0)} , new Boundary(new Coordinate(2,2,0), new Coordinate(3,3,0) ), false );
            Assert.AreEqual(new Coordinate(1,1,0), result.GetEndCoordinate() );
        }

        [TestMethod()]
        public void TestFindShortestPath_ToABoundaryNotBlocked_ExpectSuccess() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new List<Coordinate>()
            { new Coordinate(0,0,0)}, new Boundary(new Coordinate(2, 2, 0), new Coordinate(3, 3, 0)), false);
            Assert.AreEqual(new Coordinate(2, 2, 0), result.GetEndCoordinate());
        }

        [TestMethod()]
        [ExpectedException(typeof(NoPathFoundException))]
        public void TestFindShortestPath_ToABoundaryWithTargetAccess_ExpectException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(2, 2, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(2, 3, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(3, 2, 0), new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(3, 3, 0), new CellInfo(false, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new List<Coordinate>()
            { new Coordinate(0,0,0)}, new Boundary(new Coordinate(2, 2, 0), new Coordinate(3, 3, 0)), true);
        }

        [TestMethod()]
        public void TestFindShortestPath_WithBlockedOffCells_ExpectCorrectPath() {
            MapInfo mapInfo = new MapInfo(4, 4, 1);
            mapInfo.FillMapWithCells(new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(0,0,0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0,1,0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0,2,0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(1,3,0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(2,3,0), new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(2, 3, 0));
            MovementPath compare = new MovementPath(new Coordinate(0,0,0));
            compare.AddMovement(new Movement(0,1,0,1));
            compare.AddMovement(new Movement(0,1,0,1));
            compare.AddMovement(new Movement(1,1,0,1));
            compare.AddMovement(new Movement(1,0,0,1));
            Assert.AreEqual(result,compare);
        }

        [TestMethod()]
        public void TestFindShortestPath_WithHightWeightCells_ExpectCorrectPath() {
            MapInfo mapInfo = new MapInfo(4, 4, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 20));
            mapInfo.SetCell(new Coordinate(0, 0, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 1, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 2, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(1, 3, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(2, 3, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(3, 3, 0), new CellInfo(true, 1));
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(3, 3, 0));
            MovementPath compare = new MovementPath(new Coordinate(0, 0, 0));
            compare.AddMovement(new Movement(0, 1, 0, 1));
            compare.AddMovement(new Movement(0, 1, 0, 1));
            compare.AddMovement(new Movement(1, 1, 0, 1));
            compare.AddMovement(new Movement(1, 0, 0, 1));
            compare.AddMovement(new Movement(1, 0, 0, 1));
            Assert.AreEqual(result, compare);
        }

        [TestMethod()]
        public void TestFindShortestPath_WithComplexStructure_ExpectCorrectPath() {
            MapInfo mapInfo = new MapInfo(4, 4, 1);
            mapInfo.FillMapWithCells(new CellInfo(false, 1));
            mapInfo.SetCell(new Coordinate(0, 0, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 1, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 2, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(0, 3, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(1, 4, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(2, 4, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(3, 4, 0), new CellInfo(true, 1));
            mapInfo.SetCell(new Coordinate(4, 4, 0), new CellInfo(true, 1)); //cost of 7
            mapInfo.SetCell(new Coordinate(1, 1, 0), new CellInfo(true, 2));
            mapInfo.SetCell(new Coordinate(2, 2, 0), new CellInfo(true, 2));
            mapInfo.SetCell(new Coordinate(3, 3, 0), new CellInfo(true, 2)); //cost of 3*2.414+1>7
            MovementPath result = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(4, 4, 0));
            MovementPath compare = new MovementPath(new Coordinate(0, 0, 0));
            compare.AddMovement(new Movement(0, 1, 0, 1));
            compare.AddMovement(new Movement(0, 1, 0, 1));
            compare.AddMovement(new Movement(0, 1, 0, 1));
            compare.AddMovement(new Movement(1, 1, 0, 1));
            compare.AddMovement(new Movement(1, 0, 0, 1));
            compare.AddMovement(new Movement(1, 0, 0, 1));
            compare.AddMovement(new Movement(1, 0, 0, 1));
            Assert.AreEqual(result, compare);

            //this path is shorter because it is a better to go diagonal 'roundabout' -- test this
            MovementPath result2 = PathFinderAStar.FindShortestPath(mapInfo, new Coordinate(0, 0, 0), new Coordinate(3, 3, 0));
            MovementPath compare2 = new MovementPath(new Coordinate(0, 0, 0));
            compare2.AddMovement(new Movement(1, 1, 0, 1.5));
            compare2.AddMovement(new Movement(1, 1, 0, 2));
            compare2.AddMovement(new Movement(1, 1, 0, 2));
            Console.Out.WriteLine(result2);
            Console.Out.WriteLine(compare2);
            Assert.AreEqual(result2, compare2);
        }
    }
}