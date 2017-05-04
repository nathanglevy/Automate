using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements.Tests {


    [TestClass()]
    public class TestComponentTransportRequirement
    {
        public ComponentStackGroup ComponentStackGroup = new ComponentStackGroup();

        [TestInitialize]
        public void TestInit()
        {
            ComponentStackGroup.AddComponentStack(ComponentType.IronIngot, 100);
        }

        [TestMethod()]
        public void TestSatisfyRequirement() {
            IRequirement requirement = new ComponentDeliveryRequirement(Component.IronIngot, 100);
            Assert.AreEqual(100, requirement.RequirementRemainingToSatisfy);
            requirement.SatisfyRequirement(50);
            Assert.AreEqual(50,requirement.RequirementRemainingToSatisfy);
            requirement.SatisfyRequirement(50);
            Assert.AreEqual(0, requirement.RequirementRemainingToSatisfy);
        }

        [TestMethod()]
        [ExpectedException(typeof(RequirementException))]
        public void TestSatisfyRequirement_TryToSatisfyOverRemaining_ExpectArgumentException() {
            IRequirement requirement = new ComponentDeliveryRequirement(Component.IronIngot, 100);
            requirement.SatisfyRequirement(50);
            requirement.SatisfyRequirement(50);
            requirement.SatisfyRequirement(50);
        }

        [TestMethod()]
        public void TestAttachAction() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.AttachAction(taskAction);
            taskAction.OnCompleted();
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskActionException))]
        public void TestAttachAction_AmountLargetThanSatisfiableAmount_ExpectRequirmentException() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 110);
            requirement.AttachAction(taskAction);
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskActionException))]
        public void TestAttachAction_WrongTypeOfAction_ExpectTaskActionException() {
            IRequirement requirement = new ComponentDeliveryRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.AttachAction(taskAction);
        }

        [TestMethod()]
        public void TestDettachAction() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.AttachAction(taskAction);
            requirement.DettachAction(taskAction);
            taskAction.OnCompleted();
        }

        [TestMethod()]
        [ExpectedException(typeof(TaskActionException))]
        public void TestDettachAction_TryToDetachUnconncted_ExpectException() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.DettachAction(taskAction);
        }

        [TestMethod()]
        public void TestOnTaskCompleted_WhenConnected_ExpectAmountToSatisfy() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.AttachAction(taskAction);
            taskAction.OnCompleted();
            Assert.AreEqual(90, requirement.RequirementRemainingToSatisfy);
        }

        [TestMethod()]
        public void TestOnTaskCompleted_WhenNotConnected_ExpectAmountToNotSatisfy() {
            IRequirement requirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            TaskAction taskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            requirement.AttachAction(taskAction);
            requirement.DettachAction(taskAction);
            taskAction.OnCompleted();
            Assert.AreEqual(100, requirement.RequirementRemainingToSatisfy);
        }

        [TestMethod()]
        public void TestCanAttachToAction() {
            IRequirement pickupRequirement = new ComponentPickupRequirement(Component.IronIngot, 100);
            IRequirement deliveryRequirement = new ComponentDeliveryRequirement(Component.IronIngot, 100);
            TaskAction pickupTaskAction = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            TaskAction pickupTaskActionBig = new PickupTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 110);
            TaskAction deliverTaskAction = new DeliverTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 10);
            TaskAction deliverTaskActionBig = new DeliverTaskAction(new Guid(), ComponentStackGroup, new Coordinate(1, 1, 0), Component.IronIngot, 110);
            TaskAction genericTaskAction = new TaskAction(new Guid(), new Coordinate(1, 1, 0),  10);

            Assert.IsTrue(pickupRequirement.CanAttachToAction(pickupTaskAction));
            Assert.IsTrue(deliveryRequirement.CanAttachToAction(deliverTaskAction));
            Assert.IsFalse(pickupRequirement.CanAttachToAction(deliverTaskAction));
            Assert.IsFalse(deliveryRequirement.CanAttachToAction(pickupTaskAction));
            Assert.IsFalse(pickupRequirement.CanAttachToAction(pickupTaskActionBig));
            Assert.IsFalse(deliveryRequirement.CanAttachToAction(deliverTaskActionBig));

            Assert.IsFalse(pickupRequirement.CanAttachToAction(genericTaskAction));
            Assert.IsFalse(deliveryRequirement.CanAttachToAction(genericTaskAction));
        }
    }
}