using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.TaskActionHandler
{
    public class TaskActionContainer : ModelMasterAction
    {
        public ITaskAction TargetAction { get; }

        public TaskActionContainer(ITaskAction taskAction, Guid masterTaskId) : base(ActionType.DEFAULT)
        {
            MasterTaskId = masterTaskId;
            TargetAction = taskAction;
        }

        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;
     
    }
}