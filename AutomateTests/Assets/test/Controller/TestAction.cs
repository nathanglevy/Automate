using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass()]
    public class TestGameTestAction
    {
        [TestMethod]
        public void TestGetID_ExpectNotNull()
        {
            var action = new MasterTestMasterAction(ActionType.AreaSelection);
            Assert.IsNotNull(action.Id);

        }
    }
}
