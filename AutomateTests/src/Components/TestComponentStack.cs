using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automate.Model.Components.Tests {
    [TestClass()]
    public class TestComponentStack {

        [TestMethod()]
        public void TestComponentStack_ExpectSuccess_NotNull()
        {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 10);
            Assert.IsNotNull(componentStack);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestComponentStack_NegativeAmount_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), -2);
        }

        [TestMethod()]
        public void TestAmount_Remaining_Allocated() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 10);
            Assert.AreEqual(componentStack.Amount,10);
            Assert.AreEqual(componentStack.UnallocatedAmount,10);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount,0);
        }

        [TestMethod()]
        public void TestAddAmount_ExpectCorrectValues() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 10);
            componentStack.AddAmount(10);
            Assert.AreEqual(componentStack.Amount, 20);
            Assert.AreEqual(componentStack.UnallocatedAmount, 20);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 0);
            componentStack.AddAmount(10);
            Assert.AreEqual(componentStack.Amount, 30);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAmount_NegativeValue_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 10);
            componentStack.AddAmount(-10);
        }

        [TestMethod()]
        public void TestRemoveAmount_ExpectCorrectValues() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.RemoveAmount(10);
            Assert.AreEqual(componentStack.Amount, 10);
            Assert.AreEqual(componentStack.UnallocatedAmount, 10);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 0);
            componentStack.RemoveAmount(10);
            Assert.AreEqual(componentStack.Amount, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRemoveAmount_NegativeValue_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.RemoveAmount(-10);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAmount_NegativeOverflow_ExpectArgumentOutOfRangeException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.RemoveAmount(21);
        }

        [TestMethod()]
        public void TestAssignAmountTo_ExpectSuccess() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 10);
        }

        [TestMethod()]
        public void TestAssignAmountTo_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 10);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount,10);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 5);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 15);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestAssignAmountTo_AssignMoreThanExists_ExpectArgumentOutOfRangeException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 20);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 10);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 10);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 5);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 15);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 10);
        }

        [TestMethod()]
        public void TestGetAmountAssignedTo_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 10);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 10);
            componentStack.AssignOutgoingAmount(Guid.NewGuid(), 5);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 15);
        }

        [TestMethod()]
        public void TestAssignIncomingAmount_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            componentStack.AssignIncomingAmount(targetGuid, 10);
            Assert.AreEqual(componentStack.IncomingAllocatedAmount, 10);
        }

        [TestMethod()]
        public void TestGetIncomingAllocatedAmountForGuid_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignIncomingAmount(targetGuid, 10);
            componentStack.AssignIncomingAmount(targetGuid2, 20);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid), 10);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid2), 20);
        }

        [TestMethod()]
        public void TestUnassignIncomingAmount_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignIncomingAmount(targetGuid, 10);
            componentStack.AssignIncomingAmount(targetGuid2, 20);
            componentStack.UnassignIncomingAmount(targetGuid2, 15);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid), 10);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid2), 5);
        }

        [TestMethod()]
        public void TestAssignOutgoingAmount_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(targetGuid, 10);
            componentStack.AssignOutgoingAmount(targetGuid2, 20);
            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid), 10);
            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid2), 20);
        }

        [TestMethod()]
        public void TestUnAssignOutgoingAmount_ExpectCorrectValue() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(targetGuid, 10);
            componentStack.UnassignOutgoingAmount(targetGuid, 5);
            componentStack.AssignOutgoingAmount(targetGuid2, 20);
            componentStack.UnassignOutgoingAmount(targetGuid2, 10);

            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid), 5);
            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid2), 10);
        }

        [TestMethod()]
        public void TestDeliverAmount_ExpectCorrectUpdates() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 0);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignIncomingAmount(targetGuid, 10);
            componentStack.DeliverAmount(targetGuid, 5);
            componentStack.AssignIncomingAmount(targetGuid2, 20);
            componentStack.DeliverAmount(targetGuid2, 10);

            Assert.AreEqual(componentStack.Amount, 15);
            Assert.AreEqual(componentStack.IncomingAllocatedAmount, 15);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid), 5);
            Assert.AreEqual(componentStack.GetIncomingAllocatedAmountForGuid(targetGuid2), 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeliverAmount_IdNotDefined_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 0);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.DeliverAmount(targetGuid2, 10);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeliverAmount_NegativeAmount_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 0);
            Guid targetGuid = Guid.NewGuid();
            componentStack.DeliverAmount(targetGuid, -10);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeliverAmount_DeliverWithNoIncomingAssigned_ExpectArgumentException() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 0);
            Guid targetGuid = Guid.NewGuid();
            componentStack.DeliverAmount(targetGuid, 10);
        }

        [TestMethod()]
        public void TestPickupAmount_ExpectCorrectUpdates() {
            ComponentStack componentStack = new ComponentStack(new IronOreComponent(), 30);
            Guid targetGuid = Guid.NewGuid();
            Guid targetGuid2 = Guid.NewGuid();
            componentStack.AssignOutgoingAmount(targetGuid, 10);
            componentStack.PickupAmount(targetGuid, 5);
            componentStack.AssignOutgoingAmount(targetGuid2, 20);
            componentStack.PickupAmount(targetGuid2, 10);

            Assert.AreEqual(componentStack.Amount, 15);
            Assert.AreEqual(componentStack.OutgoingAllocatedAmount, 15);
            Assert.AreEqual(componentStack.UnallocatedAmount, 0);
            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid), 5);
            Assert.AreEqual(componentStack.GetOutgoingAllocatedAmountForGuid(targetGuid2), 10);
        }
    }
}