using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.TaskActionHandler
{
    public class TaskActionContainer : ModelMasterAction
    {
        public TaskAction TargetAction { get; }

        public TaskActionContainer(TaskAction taskAction) : base(ActionType.DEFAULT)
        {
            TargetAction = taskAction;
        }

        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;

        public new Guid MasterTaskId => TargetAction.MasterTask;
    }
}