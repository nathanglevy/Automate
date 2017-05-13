using System;
using Automate.Controller.Handlers.RequirementsHandler;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.Requirements;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller.TestDeliveryProviders
{
    [TestClass]
    public class TestCarriedMovableDirectRoute

    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var directRouteScenario = new CarriedMovableDirectRoute();
            Assert.IsNotNull(directRouteScenario);
        }

        [TestMethod]
        public void TestCalcCostForDirectRouteScenarioNoMovablesCarryComponentExists_ExpectfloatMax()
        {
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable = gameWorld.CreateMovable(new Coordinate(3, 3, 0), MovableType.NormalHuman);

            var structure = gameWorld.CreateStructure(new Coordinate(3, 4, 0), new Coordinate(2, 2, 1),
                StructureType.LargeFire);
            structure.CurrentJob = new RequirementJob(JobType.ItemTransport);
            structure.CurrentJob.JobRequirements.AddRequirement(
                new ComponentDeliveryRequirement(Component.IronOre, 100));
            var addRequirement = structure.CurrentJob.JobRequirements.GetIncompleteTransportRequirements()[0];

            var directRouteScenario = new CarriedMovableDirectRoute();
            var calcScenarioCost = directRouteScenario.CalcScenarioCost(structure.CurrentJob, addRequirement,
                structure.Boundary, gameWorld);
            Assert.AreEqual(float.PositiveInfinity, calcScenarioCost.Cost);
            Assert.IsNull(calcScenarioCost.ScenarioTask);
        }

        [TestMethod]
        public void TestCalcCostForDirectRouteScenarioWithSingleMovablesCarryComponentExists_ExpectDeliverTask()
        {
            var gameWorld = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var movable1 = gameWorld.CreateMovable(new Coordinate(7, 8, 0), MovableType.NormalHuman);
            var movable = gameWorld.CreateMovable(new Coordinate(3, 3, 0), MovableType.NormalHuman);
            movable.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 80);
                
            var structure = gameWorld.CreateStructure(new Coordinate(3, 4, 0), new Coordinate(2, 2, 1),
                StructureType.LargeFire);
            structure.CurrentJob = new RequirementJob(JobType.ItemTransport);
            structure.CurrentJob.JobRequirements.AddRequirement(
                new ComponentDeliveryRequirement(Component.IronOre, 100));

            var directRouteScenario = new CarriedMovableDirectRoute();
            var addRequirement = structure.CurrentJob.JobRequirements.GetIncompleteTransportRequirements()[0];

            var calcScenarioCost = directRouteScenario.CalcScenarioCost(structure.CurrentJob, addRequirement,
                structure.Boundary, gameWorld);
            Assert.AreNotEqual(float.MaxValue, calcScenarioCost.Cost);
            Assert.IsNotNull(calcScenarioCost.ScenarioTask);
            Assert.AreEqual(movable.Guid,calcScenarioCost.ScenarioTask.TargetTask.AssignedToGuid);
            Assert.AreEqual(80, calcScenarioCost.ScenarioTask.TargetTask.GetCurrentAction().Amount);
            Assert.AreEqual(TaskActionType.DeliveryTask, calcScenarioCost.ScenarioTask.TargetTask.GetCurrentAction().TaskActionType);

        }

    }
}
