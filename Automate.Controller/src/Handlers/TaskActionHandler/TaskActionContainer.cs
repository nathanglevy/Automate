using System;
using Automate.Controller.Interfaces;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.TaskActionHandler
{
    public class TaskActionContainer : IObserverArgs
    {
        public TaskAction TargetAction { get; }

        public TaskActionContainer(TaskAction taskAction)
        {
            TargetAction = taskAction;
        }

        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;
    }
}