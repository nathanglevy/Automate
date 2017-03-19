using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using JetBrains.Annotations;

namespace Assets.src.Controller.Interfaces
{
    public interface ISecondryObserver
    {
        int GetNotificationCount();
        List<MasterAction> Notify(IObserverArgs notification);
        IHandlersManager GetHandlersManager();
    }
}