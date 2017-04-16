using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;

namespace Automate.Controller.Interfaces
{
    public interface IScheduler<T>
    {
        int ItemsCount { get;  }

        bool HasItems { get;  }
        event NotifyOnPull<T> OnPull;
        event NotifyOnPull<T> OnEnqueue;

//        event SchedItemPull OnPull

        void Enqueue(IList<T> items);
        void Enqueue(T item);

        T Pull();

        HandlerResultListner<T> GetPushInvoker();
        void OnPullFinish(ViewUpdateArgs args);
        void OnPullStart(ViewUpdateArgs args);
    }

    public delegate void NotifyOnPull<T>(T item);
}