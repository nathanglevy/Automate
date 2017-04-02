using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;

namespace Automate.Controller.Interfaces
{
    public interface IScheduler
    {
        int ActionsCount { get;  }
        bool HasActions { get;  }
        void Enqueue(IList<MasterAction> actions);

        MasterAction Pull();

        HandlerResultListner GetPushInvoker();
    }

    // Delegate to all objects want to 
}