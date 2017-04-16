using System;
using System.Collections.Generic;
using System.Linq;

namespace Automate.Model.Tasks
{
    public class TaskDelegator
    {
        private List<Task> _pendingDelegationTasks = new List<Task>();
        private Dictionary<Guid,Task> _delegatedTasks = new Dictionary<Guid, Task>();

        public void AssignTask(Guid assignTaskTo, Task taskToAssign)
        {
            if (taskToAssign.IsAssigned)
                throw new TaskAssignmentException("Task is already assigned, cannot assign again!");
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

        public T GetNextPendingTask<T>() where T : Task
        {
            return _pendingDelegationTasks.First(s => s.GetType() == typeof(T)) as T;
        }

        public Task GetNextDelegatedTaskForGuid(Guid delegatedGuid)
        {
            return GetDelegatedTasksForGuid(delegatedGuid).First();
        }

        public T GetNextDelegatedTaskForGuid<T>(Guid delegatedGuid) where T : Task {
            return GetDelegatedTasksForGuid(delegatedGuid).First(s => s.GetType() == typeof(T)) as T;
        }

//        public List<Guid> GetListWithNoAssignedTask(List<Guid> movableIdList)
//        {
//            List<Guid> result = new List<Guid>();
//            foreach (Guid guid in movableIdList)
//            {
//                throw new NotImplementedException();
//            }
//            return result;
//        }
        public void AddPendingTask(Task newTask)
        {
            _pendingDelegationTasks.Add(newTask);
        }
    }
}