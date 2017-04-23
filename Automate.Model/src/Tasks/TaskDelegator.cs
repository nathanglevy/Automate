using System;
using System.Collections.Generic;
using System.Linq;

namespace Automate.Model.Tasks
{
    public class TaskDelegator
    {
        private readonly List<Task> _pendingDelegationTasks = new List<Task>();
        private readonly Dictionary<Guid,Task> _delegatedTasks = new Dictionary<Guid, Task>();

        public void AssignTask(Guid assignTaskTo, Task taskToAssign)
        {
            if (taskToAssign.IsAssigned)
                throw new TaskAssignmentException("Task is already assigned, cannot assign again!");
            if (!_pendingDelegationTasks.Contains(taskToAssign))
                throw new TaskAssignmentException("Task is not in pending delegation task pool! Add task first and make sure it is not delegated");
            _pendingDelegationTasks.Remove(taskToAssign);
            taskToAssign.AssignedToGuid = assignTaskTo;
            _delegatedTasks[taskToAssign.Guid] = taskToAssign;
        }

        public bool IsPendingTasksForDelegation()
        {
            return _pendingDelegationTasks.Count > 0;
        }

        public IEnumerable<Task> GetDelegatedTasksForGuid(Guid delegatedGuid)
        {
            return _delegatedTasks.Where(pair => pair.Value.AssignedToGuid.Equals(delegatedGuid))
                  .Select(pair => pair.Value);
        }

        public Task GetNextPendingTask()
        {
            if (!IsPendingTasksForDelegation())
                throw new NoTaskException("No pending tasks");
            return _pendingDelegationTasks[0];
        }

        public Task GetNextDelegatedTaskForGuid(Guid delegatedGuid)
        {
            if (!HasDelegatedTasks(delegatedGuid))
                throw new NoTaskException("There are no delegated tasks for this GUID - cannot return next");
            return GetDelegatedTasksForGuid(delegatedGuid).First();
        }

//        private void AddPendingTask(Task newTask)
//        {
//            _pendingDelegationTasks.Add(newTask);
//        }

        public Task CreateNewTask()
        {
            Task newTask = new Task();
            _pendingDelegationTasks.Add(newTask);
            _delegatedTasks[newTask.Guid] = newTask;
            return newTask;
        }

        public bool HasDelegatedTasks(Guid assignee)
        {
            return GetDelegatedTasksForGuid(assignee).Any();
        }

        public void RemoveCompletedTasks(Guid assignee)
        {
            IEnumerable<Guid> tasksToRemove = GetDelegatedTasksForGuid(assignee).Where(task => task.IsTaskComplete()).Select(task => task.Guid);
            foreach (Guid taskToRemove in tasksToRemove)
            {
                _delegatedTasks.Remove(taskToRemove);
            }
        }

        public Task GetTaskByGuid(Guid taskGuid)
        {
            if (!_delegatedTasks.ContainsKey(taskGuid))
                throw new NoTaskException("Task with this GUID does not exist in task delegator");
            return _delegatedTasks[taskGuid];
        }
    }
}


//        public T GetNextDelegatedTaskForGuid<T>(Guid delegatedGuid) where T : Task {
//            return GetDelegatedTasksForGuid(delegatedGuid).First(s => s.GetType() == typeof(T)) as T;
//        }

//        public List<Guid> GetListWithNoAssignedTask(List<Guid> movableIdList)
//        {
//            List<Guid> result = new List<Guid>();
//            foreach (Guid guid in movableIdList)
//            {
//                throw new NotImplementedException();
//            }
//            return result;
//        }

//        public T GetNextPendingTask<T>() where T : Task
//        {
//            return _pendingDelegationTasks.First(s => s.GetType() == typeof(T)) as T;
//        }