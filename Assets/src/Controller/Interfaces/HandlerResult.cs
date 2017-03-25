using System.Collections.Generic;
using Assets.src.Controller.Abstracts;

namespace Assets.src.Controller.Interfaces
{
    public class HandlerResult : IHandlerResult
    {
        private readonly List<MasterAction> _actions;

        public HandlerResult(List<MasterAction> actions)
        {
            _actions = actions;
        }

        public IList<MasterAction> GetActions()
        {
            return _actions;
        }
    }
}