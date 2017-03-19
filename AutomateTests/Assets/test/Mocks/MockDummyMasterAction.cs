using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace AutomateTests.test.Mocks
{
    public class MockDummyMasterAction : MasterAction
    {
        public MockObserverArgs Args { get; }

        public MockDummyMasterAction(ActionType type,MockObserverArgs args) : base(type)
        {
            Args = args;
        }

        public MockDummyMasterAction() : base(ActionType.AreaSelection)
        {
            throw new System.NotImplementedException();
        }
    }
}