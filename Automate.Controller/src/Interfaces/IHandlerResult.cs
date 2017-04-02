using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public interface IHandlerResult
    {
        IList<MasterAction> GetActions();


    }
}