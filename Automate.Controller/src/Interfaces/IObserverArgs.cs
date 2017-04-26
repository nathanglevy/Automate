using System;

namespace Automate.Controller.Interfaces
{
    public interface IObserverArgs
    {
        Guid TargetId { get; }
        event ControllerNotification OnComplete;
    }
}