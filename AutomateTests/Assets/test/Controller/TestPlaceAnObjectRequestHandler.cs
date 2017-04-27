using Automate.Controller.Handlers;
using Automate.Controller.Handlers.PlaceAnObject;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestPlaceAnObjectRequestHandler
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            PlaceAnObjectRequestHandler handler = new PlaceAnObjectRequestHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestHandlePlaceAStructure_ExpectStructureToBeAdded()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var placeAnObjectRequest = new PlaceAStrcutureRequest(new Coordinate(3, 2, 0),
                new Coordinate(1, 1, 1), StructureType.Basic);
            var handler = new PlaceAnObjectRequestHandler();
            var handlerResult = handler.Handle(placeAnObjectRequest, new HandlerUtils(gameWorldItem.Guid, null, null));
            Assert.AreEqual(0, handlerResult.GetItems().Count);
            Assert.IsTrue(gameWorldItem.IsThereAnItemToBePlaced());
            Assert.AreEqual(ItemType.Structure, gameWorldItem.GetItemsToBePlaced().FindLast(p => p.Type == ItemType.Structure).Type);
        }


        [TestMethod]
        public void TestHandlePlaceAMovable_ExpectMovableToBeAdded()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(10, 10, 1));
            var placeAnObjectRequest = new PlaceAMovableRequest(new Coordinate(3, 2, 0),
                 MovableType.SimpleRobot);
            var handler = new PlaceAnObjectRequestHandler();
            var handlerResult = handler.Handle(placeAnObjectRequest, new HandlerUtils(gameWorldItem.Guid, null, null));
            Assert.AreEqual(0, handlerResult.GetItems().Count);
            Assert.IsTrue(gameWorldItem.IsThereAnItemToBePlaced());
            Assert.AreEqual(ItemType.Movable, gameWorldItem.GetItemsToBePlaced().FindLast(p => p.Type == ItemType.Movable).Type);
        }
    }
}
