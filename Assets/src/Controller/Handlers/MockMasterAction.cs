using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller
{
    public class MockMasterAction : MasterAction
    {
        public MockMasterAction(ActionType type) : base(type)
        {
        }
    }
}