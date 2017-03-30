using Automate.Controller.src.Abstracts;

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