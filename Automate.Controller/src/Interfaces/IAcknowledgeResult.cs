using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public interface IAcknowledgeResult:IHandlerResult
    {
    }

    public class AcknowledgeResult : HandlerResult,IAcknowledgeResult
    {
        public AcknowledgeResult(List<MasterAction> actions) : base(actions)
        {
        }
    }
}