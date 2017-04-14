using System;
using System.Collections.Generic;

namespace Automate.Model.Tasks
{
    public class TaskDelegator
    {
        private List<Task> _pendingDelegationTasks = new List<Task>();
        private Dictionary<Guid,Task> _delegatedTasks = new Dictionary<Guid, Task>();

        public void AssignTask(Guid assignTaskTo, Task taskToAssign)
        {
            _pendingDelegationTasks.Remove(taskToAssign);
            taskToAssign.AssignedToGuid = assignTaskTo;
            _delegatedTasks[taskToAssign.Guid] = taskToAssign;
        }

    }
}