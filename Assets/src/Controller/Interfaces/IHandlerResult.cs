using System.Collections.Generic;
using System.Threading;
using Assets.src.Controller.Abstracts;

namespace Assets.src.Controller.Interfaces
{
    public interface IHandlerResult
    {
        IList<MasterAction> GetActions();


    }
}