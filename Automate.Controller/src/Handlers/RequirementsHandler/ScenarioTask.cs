using System;
using Automate.Controller.Handlers.TaskHandler;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class ScenarioTask
    {
        private readonly Action _incaseOfExecution;

        public ScenarioTask(ScenarioCost cost, TaskContainer task, Action incaseOfExecution)
        {
            _incaseOfExecution = incaseOfExecution;
            ScenarioCost = cost;
            Task = task;
        }

        public void Execute()
        {
            _incaseOfExecution.Invoke();
        }

        public ScenarioCost ScenarioCost { get; set; }
        public TaskContainer Task { get; set; }
    }
}