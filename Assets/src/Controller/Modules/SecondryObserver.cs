using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller
{
    public class SecondryObserver : ISecondryObserver
    {
        private int _notificationCount = 0;
        public IHandlersManager HandlersManager { get; private set; }

        public SecondryObserver()
        {
            HandlersManager = new HandlersManager();
        }

        public int GetNotificationCount()
        {
            return _notificationCount;

        }

        public IHandlersManager GetHandlersManager()
        {
            return HandlersManager;
        }

        public List<MasterAction> Notify(IObserverArgs notification)
        {
            _notificationCount++;
            return HandlersManager.Handle(notification);
        }
    }
}