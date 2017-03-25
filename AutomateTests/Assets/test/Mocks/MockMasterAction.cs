using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace AutomateTests.test.Mocks
{
    public class MockMasterAction : MasterAction
    {
        public MockMasterAction(ActionType type, string id) : base(type)
        {
            TargetId = id;
        }
    }
}