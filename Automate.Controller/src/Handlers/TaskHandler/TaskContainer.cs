using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.Tasks;

namespace Automate.Controller.Handlers.TaskHandler
{
    public class TaskContainer : ModelMasterAction
    {
        public Task TargetTask { get; }

        public TaskContainer(Task targetTask) : base(ActionType.Internal)
        {
            TargetTask = targetTask;
            MasterTaskId = targetTask.Guid;
        }


        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;


    }
}