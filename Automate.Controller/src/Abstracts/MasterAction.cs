using System;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Abstracts
{
    public class MasterAction : IObserverArgs
    {
        public TimeSpan Duration { get; set; }

        public bool NeedAcknowledge { get; set; }

        public MasterAction(ActionType type, string targetId) : this(type, new Guid(targetId))
        {}


        public MasterAction(ActionType type, Guid targetId)
        {
            Type = type;
            TargetId = targetId;
            Duration = new TimeSpan();
            NeedAcknowledge = false;
        }

        public MasterAction(ActionType type)
        {
            Type = type;
        }

        public ActionType Type { get; protected set; }

        public Guid TargetId { get; set; }
        public event ControllerNotification OnComplete;

        public bool IsActionHasOver { get; set; }
        public ControllerNotification OnCompleteDelegate { get; set; }

        public void FireOnComplete(ControllerNotificationArgs controllerNotificationArgs)
        {
            if (OnCompleteDelegate == null) return;

            OnCompleteDelegate.Invoke(controllerNotificationArgs);
        }
    }
}