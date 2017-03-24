using Assets.src.Controller;
using Assets.src.Controller.Handlers;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Assets.src.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestControllerSelectionHandler   
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IHandler handler  = new ControllerSelectionHandler();
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void TestSelectionAction_expectsTypeAndCorrdinates()
        {
            Coordinate source = new Coordinate(0,0,0);
            Coordinate target = new Coordinate(10, 0, 0);
            var selectionAction = new SelectionMasterAction(source, target);
            Assert.AreEqual(ActionType.AreaSelection,selectionAction.Type);
            Assert.AreEqual(source,selectionAction.UpperLeft);
            Assert.AreEqual(target, selectionAction.BottomRight);
        }


        [TestMethod]
        public void TestHandleMethod_ExpectGetSelectionResultAction()
        {
            IHandler handler = new ControllerSelectionHandler();
            SelectionArgs args = new SelectionArgs(new Coordinate(0,0,0),new Coordinate(10,10,0) );
            var handled = handler.Handle(args);
            Assert.IsNotNull(handled);
            Assert.AreEqual(1,handled.Count);
            var masterAction = handled[0];
            Assert.AreEqual(ActionType.AreaSelection,masterAction.Type);
        }

    }
}
