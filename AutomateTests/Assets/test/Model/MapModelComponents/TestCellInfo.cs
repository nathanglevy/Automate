using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.src.MapModelComponents;

namespace AutomateTests.test.Model.MapModelComponents {
    [TestClass()]
    public class TestCellInfo {
        [TestMethod()]
        public void TestIsPassable_ExpectTrue() {
            CellInfo cellInfo = new CellInfo(true, 0);
            Assert.IsTrue(cellInfo.IsPassable());
        }

        [TestMethod()]
        public void TestIsPassable_ExpectFalse() {
            CellInfo cellInfo = new CellInfo(false, 0);
            Assert.IsFalse(cellInfo.IsPassable());
        }

        [TestMethod()]
        public void TestGetWeight_ExpectValue() {
            CellInfo cellInfo = new CellInfo(true, 2);
            Assert.AreEqual(2, cellInfo.GetWeight());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCellInfoConstructor_ExpectArgumentException() {
            CellInfo cellInfo = new CellInfo(true, -2);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCellInfoConstructor_ExpectArgumentNullExceptionn() {
            CellInfo cellInfo = new CellInfo(true, 2, null, null);
        }


    }
}