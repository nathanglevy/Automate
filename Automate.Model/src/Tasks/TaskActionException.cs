using System;

namespace Automate.Model.Tasks
{
    public class TaskActionException : Exception {
        public TaskActionException(string message) : base(message) { }
    }
}