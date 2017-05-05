using System.Linq;
using Automate.Model.Components;
using Automate.Model.Requirements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Requirements {
    [TestClass()]
    public class TestRequirementContainer {
        public RequirementContainer requirementContainer;

        [TestInitialize()]
        public void TestInit()
        {
            requirementContainer = new RequirementContainer();
        }

        [TestMethod()]
        public void TestAddRequirement_ExpectSuccess() {
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronOre, 10));
        }

        [TestMethod()]
        public void TestHasIncompleteRequirements_ExpectCorrectValue() {
            Assert.IsFalse(requirementContainer.HasIncompleteRequirements());
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronOre, 10));
            Assert.IsTrue(requirementContainer.HasIncompleteRequirements());
        }

        [TestMethod()]
        public void TestGetIncompleteRequirements() {
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 20));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 0));
            foreach (IRequirement incompleteRequirement in requirementContainer.GetIncompleteRequirements())
            {
                Assert.IsFalse(incompleteRequirement.IsSatisfied);
            }
        }

        [TestMethod()]
        public void TestGetCompleteRequirements() {
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 20));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 0));
            foreach (IRequirement incompleteRequirement in requirementContainer.GetCompleteRequirements()) {
                Assert.IsTrue(incompleteRequirement.IsSatisfied);
            }
        }

        [TestMethod()]
        public void TestGetAllRequirements() {
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 20));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 0));
            Assert.AreEqual(3, requirementContainer.GetAllRequirements().Count());
            Assert.AreEqual(30, requirementContainer.GetAllRequirements().Sum(item => item.RequirementRemainingToSatisfy));
        }

        [TestMethod()]
        public void TestRemoveRequirement() {
            IRequirement addRequirement = requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 20));
            requirementContainer.RemoveRequirement(addRequirement);
            Assert.AreEqual(1, requirementContainer.GetAllRequirements().Count());
        }

        [TestMethod()]
        public void TestRemoveRequirements() {
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 10));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 20));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 0));
            requirementContainer.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 0));
            requirementContainer.RemoveRequirements(requirementContainer.GetCompleteRequirements());
            Assert.AreEqual(2, requirementContainer.GetAllRequirements().Count());
        }
    }
}