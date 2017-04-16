using System;

namespace Automate.Model.Tasks
{
    public class TaskAssignmentException : Exception {
        public TaskAssignmentException(string message) : base(message) { }
    }
}