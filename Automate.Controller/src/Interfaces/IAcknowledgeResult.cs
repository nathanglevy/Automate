using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public interface IAcknowledgeResult:IHandlerResult<MasterAction>
    {
    }

    public class AcknowledgeResult : HandlerResult,IAcknowledgeResult<MasterAction>
    {
        public AcknowledgeResult(List<MasterAction> actions) : base(actions)
        {
        }
    }
}