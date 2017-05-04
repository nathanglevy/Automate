using System;
using Automate.Controller.Abstracts;

namespace AutomateTests.Mocks
{
    public class MockMasterAction : MasterAction
    {
        public MockMasterAction(ActionType type, string id) : base(type)
        {
            TargetId = new Guid(id);
        }
    }
}