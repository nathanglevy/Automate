using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.Model.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automate.Model.Components.Tests {
    [TestClass()]
    public class TestComponent {
        [TestMethod()]
        public void TestGetComponent()
        {
            Component.GetComponent(ComponentType.IronOre);
            Component.GetComponent(ComponentType.IronIngot);
        }

        [TestMethod()]
        public void TestGetComponent1() {
            Component.GetComponent("IronIngot");
            Component.GetComponent("IronOre");
        }

        [TestMethod()]
        public void TestNewComponent() {
            Component.NewComponent("OilBreaker", 2, 1);
            Assert.AreEqual(1,Component.GetComponent("OilBreaker").Size);
            Assert.AreEqual(2,Component.GetComponent("OilBreaker").Weight);
        }

        [TestMethod()]
        public void TestEquals() {
            Assert.AreEqual(Component.GetComponent(ComponentType.IronIngot), Component.GetComponent("IronIngot"));
            Assert.AreEqual(Component.GetComponent("IronIngot"), Component.GetComponent("IronIngot"));
            Component.NewComponent("OilBreaker", 2, 1);
            Assert.AreNotEqual(Component.GetComponent("OilBreaker"), Component.GetComponent("IronIngot"));
            Assert.AreNotEqual(Component.GetComponent(ComponentType.IronOre), Component.GetComponent("IronIngot"));
            Assert.AreNotEqual(Component.GetComponent(ComponentType.IronOre), Component.GetComponent(ComponentType.IronIngot));

        }

        [TestMethod()]
        public void TestGetHashCode()
        {
            Component.GetComponent(ComponentType.IronOre).GetHashCode();
        }
    }
}