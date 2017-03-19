using System.Collections.Generic;
using Assets.src.Controller.Abstracts;

namespace Assets.src.Controller.Interfaces
{
    public interface IScheduler
    {
        int GetActionsCount();
        void Schedule(List<MasterAction> actions);
        IPrimaryObserver getTarget();
    }
}