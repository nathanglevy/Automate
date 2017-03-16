using System;
using System.IO;
using Assets.src.PathFinding.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityTest;

namespace AutomateTests.PathFinding.MapModelComponents {
    [TestClass()]
    public class MapInfoTests {

        [TestMethod()]
        public void MapInfoTest_initNotNull() {
            Assert.IsNotNull(new MapInfo(10, 10, 2));
        }

        [TestMethod()]
        public void MapInfoTest_setCell_noException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.SetCell(new Coordinate(0, 0, 0), new CellInfo(true, 1, null));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MapInfoTest_setCell_outOfRangeException() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            mapInfo.SetCell(new Coordinate(11, 0, 0), new CellInfo(true, 1, null));
        }

        [TestMethod()]
        public void MapInfoTest_setCell_getCell() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            CellInfo cellInfo = new CellInfo(true, 1, null);
            mapInfo.SetCell(new Coordinate(0, 0, 0), cellInfo);
            Assert.AreEqual(mapInfo.GetCell(new Coordinate(0,0,0)), cellInfo);
        }

        [TestMethod()]
        public void MapInfoTest_setCell_getCellOverload() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            CellInfo cellInfo = new CellInfo(true, 1, null);
            mapInfo.SetCell(new Coordinate(0, 0, 0), cellInfo);
            Assert.AreEqual(mapInfo.GetCell(0, 0, 0), cellInfo);
        }

        [TestMethod()]
        public void MapInfoTest_getBoundaries() {
            MapInfo mapInfo = new MapInfo(10,10,2);
            Assert.AreEqual(mapInfo.GetBoundary(), new Boundary(new Coordinate(0, 0, 0), new Coordinate(10, 10, 2)));
        }

        [TestMethod()]
        public void MapInfoTest_checkWithinBounds() {
            MapInfo mapInfo = new MapInfo(10, 10, 2);
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(0,0,0)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(5,5,1)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(10,10,2)));
            Assert.AreEqual(true, mapInfo.IsCoordinateIsWithinBounds(new Coordinate(10,10,1)));
            Assert.AreEqual(false, mapInfo.IsCoordinateIsWithinBounds( new Coordinate(11,11,3)));
        }

        [TestMethod()]
        public void MapInfoTest_testInitBasicMap() {
            MapInfo mapInfo = new MapInfo(5,5,1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1, null));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(0, 0, 0)));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(2, 2, 0)));
            Assert.IsNotNull(mapInfo.GetCell(new Coordinate(3, 3, 1)));
        }

        [TestMethod()]
        public void MapInfoTest_testLoadingMap() {
            MapInfo mapInfo = MapInfo.LoadMap("testMap.json");
            Assert.IsNotNull(mapInfo);
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MapInfoTest_testLoadingMap_FileNotFoundException() {
            MapInfo mapInfo = MapInfo.LoadMap("./invalidPathThisdoesNotexist.json");
        }

        [TestMethod()]
        public void MapInfoTest_testSavingMap() {
            MapInfo mapInfo = new MapInfo(5, 5, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1, null));
            mapInfo.SaveMap("testMap.json");
        }

        [TestMethod()]
        public void MapInfoTest_testSavedMapLoadMap() {
            MapInfo mapInfo = new MapInfo(5, 5, 1);
            mapInfo.FillMapWithCells(new CellInfo(true, 1, null));
            mapInfo.SaveMap("testSaveLoad.json");
            MapInfo loaded = MapInfo.LoadMap("testSaveLoad.json");
            Assert.AreEqual(mapInfo.GetCell(new Coordinate(0,0,0)),new CellInfo(true,1,null));
            Assert.AreEqual(loaded.GetCell(new Coordinate(0,0,0)),new CellInfo(true,1,null));
            Assert.AreEqual(loaded.GetCell(new Coordinate(1,2,1)),new CellInfo(true,1,null));
        }

        //here add tests that will check if the loaded map is indeed that same


    }
}