using System;
using Automate.Controller.Modules;

namespace Automate.Controller.Interfaces
{
    public interface ITimerScheduler<T> 
    {
        int ItemsCount { get; }

        bool HasItems { get; }

        void Enqueue(DateTime timeoutTime, T item);

        void Update(TimerSchudulerUpdateArgs args);
    }
}