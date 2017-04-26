using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public class HandlerResult : IHandlerResult<MasterAction>
    {
        private readonly List<MasterAction> _actions;

        /// <summary>
        /// bool attribute to indicate if Handled Actions should be pushed to View or Just ReHandled
        /// </summary>
        public bool IsInternal { get; set; } 

        public HandlerResult(List<MasterAction> actions)
        {
            _actions = actions;
        }

        public IList<MasterAction> GetItems()
        {
            return _actions;
        }
    }
}