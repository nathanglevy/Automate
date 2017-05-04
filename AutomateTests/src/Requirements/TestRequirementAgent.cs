using System;
using Automate.Model.CellComponents;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Requirements {
    [TestClass()]
    public class TestRequirementAgent {
        public IGameWorld GameWorld = new GameWorld(new Coordinate(10,10,1));

        [TestInitialize]
        public void TestInit()
        {
            GameWorld.CreateMovable(new Coordinate(1, 1, 0), MovableType.NormalHuman);
            ComponentStackGroup componentStackGroup = GameWorld.GetComponentStackGroupAtCoordinate(new Coordinate(2, 2, 0));
            componentStackGroup.AddComponentStack(ComponentType.Wood, 100);
        }

        [TestMethod()]
        public void TestGetStructuresWithActiveJobs()
        {
            Assert.AreEqual(0,GameWorld.RequirementAgent.GetStructuresWithJobInProgress().Count);
            IStructure structure = GameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.SmallFire);
            structure.CurrentJob = new RequirementJob(JobType.Construction);
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetStructuresWithJobInProgress().Count);
            structure.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 100));
            Assert.AreEqual(1, GameWorld.RequirementAgent.GetStructuresWithJobInProgress().Count);
        }

        [TestMethod()]
        public void TestGetStructuresWithCompletedJobs() {
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetStructuresWithCompletedJobs().Count);
            IStructure structure = GameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.SmallFire);
            structure.CurrentJob = new RequirementJob(JobType.Construction);
            Assert.AreEqual(1, GameWorld.RequirementAgent.GetStructuresWithCompletedJobs().Count);
            structure.CurrentJob.AddRequirement(new ComponentDeliveryRequirement(Component.IronIngot, 100));
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetStructuresWithCompletedJobs().Count);
        }

        [TestMethod()]
        public void TestGetCellsWithActiveJobs() {
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetCellsWithJobInProgress().Count);
            ICell cellAtCoordinate = GameWorld.GetCellAtCoordinate(new Coordinate(2, 2, 0));
            cellAtCoordinate.CurrentJob = new RequirementJob(JobType.ItemTransport);
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetCellsWithJobInProgress().Count);
            cellAtCoordinate.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.Wood, 100));
            Assert.AreEqual(1, GameWorld.RequirementAgent.GetCellsWithJobInProgress().Count);
        }

        [TestMethod()]
        public void TestGetCellsWithCompletedJobs() {
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetCellsWithCompletedJobs().Count);
            ICell cellAtCoordinate = GameWorld.GetCellAtCoordinate(new Coordinate(2, 2, 0));
            cellAtCoordinate.CurrentJob = new RequirementJob(JobType.ItemTransport);
            Assert.AreEqual(1, GameWorld.RequirementAgent.GetCellsWithCompletedJobs().Count);
            cellAtCoordinate.CurrentJob.AddRequirement(new ComponentPickupRequirement(Component.Wood, 100));
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetCellsWithCompletedJobs().Count);
        }

        [TestMethod()]
        public void TestSetConstructionJob() {
            Assert.AreEqual(0, GameWorld.RequirementAgent.GetCellsWithJobInProgress().Count);
            IStructure structure = GameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.SmallFire);
            GameWorld.RequirementAgent.SetConstructionJob(structure);
            Assert.AreEqual(1, GameWorld.RequirementAgent.GetStructuresWithJobInProgress().Count);
            Assert.AreEqual(JobType.Construction, structure.CurrentJob.JobType);
        }

        [TestMethod()]
        [ExpectedException(typeof(RequirementException))]
        public void TestSetConstructionJob_HasJobInProgress_ExpectRequirementException() {
            IStructure structure = GameWorld.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.SmallFire);
            GameWorld.RequirementAgent.SetConstructionJob(structure);
            GameWorld.RequirementAgent.SetConstructionJob(structure);
        }
    }
}