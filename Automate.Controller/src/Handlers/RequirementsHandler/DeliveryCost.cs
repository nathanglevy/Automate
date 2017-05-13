using Automate.Controller.Handlers.TaskHandler;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class DeliveryCost
    {
        public DeliveryCost(float cost, TaskContainer task,int amount)
        {
            Cost = cost;
            ScenarioTask = task;
            Amount = amount;
        }

        public float Cost { get; set; } = float.PositiveInfinity;
        public TaskContainer ScenarioTask { get; set; }
        public int Amount { get; }
    }
}