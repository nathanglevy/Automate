using System;
using Assets.src.PathFinding.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.PathFinding.MapModelComponents {
    [TestClass()]
    public class CellInfoTests {
        [TestMethod()]
        public void IsPassableTest() {
            CellInfo cellInfo = new CellInfo(true, 0, null);
            Assert.AreEqual(true,cellInfo.IsPassable());
        }

        [TestMethod()]
        public void GetWeight() {
            CellInfo cellInfo = new CellInfo(true, 2, null);
            Assert.AreEqual(2, cellInfo.GetWeight());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExpectException() {
            CellInfo cellInfo = new CellInfo(true, -2, null);
        }



    }
}