using Automate.Controller.Handlers.TaskHandler;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class DeliveryCost
    {
        public DeliveryCost(float cost, TaskContainer task)
        {
            Cost = cost;
            ScenarioTask = task;
        }

        public float Cost { get; set; } = float.PositiveInfinity;
        public TaskContainer ScenarioTask { get; set; }
    }
}