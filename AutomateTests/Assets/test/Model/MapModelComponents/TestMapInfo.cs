using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using src.Model.MapModelComponents;

namespace AutomateTests.test.Model.MapModelComponents {
    [TestClass()]
    public class TestMapInfo {

        [TestMethod()]
        public void TestNewMapInfo_ExpectNotNull() {
            Assert.IsNotNull(new MapInfo(10, 10, 2));
        }

        [TestMethod()]
        public void TestSetCell_ExpectSuccess() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.SetCell(new Coordinate(0, 0, 0), new CellInfo(true, 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetCell_OutOfRange_ExpectArgumentOutOfRangeException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.SetCell(new Coordinate(11, 0, 0), new CellInfo(true, 1));
        }

        [TestMethod()]
        public void TestSetCell_WithCoordinate_ExpectValueToBeSet() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            CellInfo cellInfo = new CellInfo(true, 1);
            mapInfo.SetCell(new Coordinate(0, 0, 0), cellInfo);
            Assert.AreEqual(mapInfo.GetCell(new Coordinate(0,0,0)), cellInfo);
        }

        [TestMethod()]
        public void TestSetCell_WithXYZArgs_ExpectValueToBeSet() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            CellInfo cellInfo = new CellInfo(true, 1);
            mapInfo.SetCell(new Coordinate(0, 0, 0), cellInfo);
            Assert.AreEqual(mapInfo.GetCell(0, 0, 0), cellInfo);
        }

        [TestMethod()]
        public void TestGetBoundary_ExpectCorrectValues() {
            MapInfo mapInfo = new MapInfo(10,10,2);
            Assert.AreEqual(mapInfo.GetBoundary(), new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2)));
        }

        [TestMethod()]
        public void TestIsCoordinateIsWithinBounds_ExpectCorrectValues() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(0,0,0)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(5,5,1)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(10,10,2)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(10,10,1)));
            Assert.AreEqual(false, mapInfo.IsCoordinateIsWithinBounds( new Coordinate(11,11,3)));
        }

        [TestMethod()]
        public void TestGetCell_ExpectCorrectValues() {
            MapInfo mapInfo = new MapInfo(5,5,1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(0, 0, 0)));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(2, 2, 0)));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(3, 3, 1)));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetCell_OutOfRange_ExpectArgumentOutOfRangeException() {
            MapInfo mapInfo = new MapInfo(5, 5, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.GetCell(new Coordinate(10, 10, 0));
        }

        [TestMethod()]
        public void TestLoadMap_ExpectNotNull() {
            MapInfo mapInfo = MapInfo.LoadMap("testMap.json");
            Assert.IsNotNull(mapInfo);
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestLoadMap_InvalidPath_ExpectFileNotFoundException() {
            MapInfo mapInfo = MapInfo.LoadMap("./invalidPathThisdoesNotexist.json");
        }

        [TestMethod()]
        public void TestLoad_ExpectSuccess() {
            MapInfo mapInfo = new MapInfo(5, 5, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SaveMap("testMap.json");
        }

        [TestMethod()]
        public void TestLoadAfterSave_ExpectSuccessAndCorrectValues() {
            MapInfo mapInfo = new MapInfo(5, 5, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1));
            mapInfo.SaveMap("testSaveLoad.json");
            MapInfo loaded = MapInfo.LoadMap("testSaveLoad.json");
            Assert.AreEqual(mapInfo.GetCell(new Coordinate(0,0,0)),new CellInfo(true,1));
            Assert.AreEqual(loaded.GetCell(new Coordinate(0,0,0)),new CellInfo(true,1));
            Assert.AreEqual(loaded.GetCell(new Coordinate(1,2,1)),new CellInfo(true,1));
        }

        //here add tests that will check if the loaded map is indeed that same


    }
}
