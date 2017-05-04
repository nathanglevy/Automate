using System;
using System.Linq;
using Automate.Model.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Components {
    [TestClass()]
    public class TestComponentStackGroup
    {
        public ComponentStackGroup ComponentStackGroup;
        public ComponentStackGroup ComponentStackGroup2;
        [TestInitialize]
        public void TestInit()
        {
            ComponentStackGroup = new ComponentStackGroup();
            ComponentStackGroup2 = new ComponentStackGroup(1000,1);
        }

        [TestMethod()]
        public void TestAddComponentStack_ExpectSuccess()
        {
            ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestAddComponentStack_OverStacking_ExpectFailure() {
            ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 10000);
        }

        [TestMethod()]
        public void TestAddComponentStackByType_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
        }

        [TestMethod()]
        public void TestAddComponentStackByType_AddTwice_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            Assert.AreEqual(200, ComponentStackGroup.GetComponentStack(ComponentType.IronOre).CurrentAmount);
        }

        [TestMethod()]
        public void TestGetComponentStack_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            ComponentStackGroup.GetComponentStack(Component.GetComponent(ComponentType.IronOre));
        }

        [TestMethod()]
        public void TestGetComponentStackByType_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            ComponentStackGroup.GetComponentStack(Component.GetComponent(ComponentType.IronOre));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetComponentStack_Noexist_ExpectArgumentException() {
            ComponentStackGroup.GetComponentStack(Component.GetComponent(ComponentType.IronOre));
        }

        [TestMethod()]
        public void TestRemoveComponentStack_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            ComponentStackGroup.RemoveComponentStack(Component.GetComponent(ComponentType.IronOre));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRemoveComponentStack_Nonexisting_ExpectArgumentException() {
            ComponentStackGroup.RemoveComponentStack(Component.GetComponent(ComponentType.IronOre));
        }

        [TestMethod()]
        public void TestRemoveComponentStackByType_ExpectSuccess() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            ComponentStackGroup.RemoveComponentStack(ComponentType.IronOre);
            Assert.IsFalse(ComponentStackGroup.IsContainingComponentStack(ComponentType.IronOre));
        }

        [TestMethod()]
        public void TestIsContainingComponentStackByType_ExpectCorrectValues() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            Assert.IsTrue(ComponentStackGroup.IsContainingComponentStack(ComponentType.IronOre));
            Assert.IsFalse(ComponentStackGroup.IsContainingComponentStack(ComponentType.IronIngot));
        }

        [TestMethod()]
        public void TestIsContainingComponentStack_ExpectCorrectValues() {
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            Assert.IsTrue(ComponentStackGroup.IsContainingComponentStack(Component.GetComponent(ComponentType.IronOre)));
            Assert.IsFalse(ComponentStackGroup.IsContainingComponentStack(Component.GetComponent(ComponentType.IronIngot)));
        }

        [TestMethod()]
        public void TestGetListOfComponentsInGroup_ExpectCorrectValues()
        {
            ComponentStackGroup.GetListOfComponentsInGroup();
            ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            Assert.AreEqual(Component.GetComponent(ComponentType.IronOre), ComponentStackGroup.GetListOfComponentsInGroup().First());
        }

        [TestMethod()]
        public void TestGetListOfComponentStacksInGroup_ExpectCorrectValues() {
            ComponentStackGroup.GetListOfComponentStacksInGroup();
            ComponentStack addComponentStack = ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            Assert.AreEqual(addComponentStack, ComponentStackGroup.GetListOfComponentStacksInGroup().First());
        }

        [TestMethod()]
        public void TestTransferToStackGroup() {
            ComponentStack componentStack = ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            Guid newGuid = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(newGuid, 100);
            ComponentStackGroup otherStackGroup = new ComponentStackGroup();
            ComponentStack otherComponentStack = otherStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            otherComponentStack.AssignIncomingAmount(newGuid, 100);
            ComponentStackGroup.TransferToStackGroup(newGuid, otherStackGroup, Component.GetComponent(ComponentType.IronOre), 100);
        }

        [TestMethod()]
        public void TestTransferToStackGroup1() {
            ComponentStack componentStack = ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            Guid newGuid = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(newGuid, 100);
            ComponentStackGroup otherStackGroup = new ComponentStackGroup();
            ComponentStack otherComponentStack = otherStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            otherComponentStack.AssignIncomingAmount(newGuid, 100);
            ComponentStackGroup.TransferToStackGroup(newGuid, otherStackGroup, ComponentType.IronOre, 100);
        }

        [TestMethod()]
        public void TestTransferToStack() {
            ComponentStack componentStack = ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            Guid newGuid = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(newGuid, 100);
            ComponentStackGroup otherStackGroup = new ComponentStackGroup();
            ComponentStack otherComponentStack = otherStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            otherComponentStack.AssignIncomingAmount(newGuid, 100);
            ComponentStackGroup.TransferToStack(newGuid, otherComponentStack, 100);
        }

        [TestMethod()]
        public void TestProperties()
        {
            Assert.AreEqual(0, ComponentStackGroup.CurrentTotalSpace);
            Assert.AreEqual(0, ComponentStackGroup.CurrentTotalWeight);
            Assert.AreEqual(0, ComponentStackGroup.TotalIncomingWeight);
            Assert.AreEqual(0, ComponentStackGroup.TotalIncomingSpace);
            ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 100);
            Assert.AreEqual(Component.GetComponent(ComponentType.IronOre).Weight * 100, ComponentStackGroup.CurrentTotalWeight);
            Assert.AreEqual(Component.GetComponent(ComponentType.IronOre).Size * 100, ComponentStackGroup.CurrentTotalSpace);
        }
    }
}