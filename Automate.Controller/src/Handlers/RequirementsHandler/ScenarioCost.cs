using System;
using Automate.Controller.Handlers.TaskHandler;

namespace Automate.Controller.Handlers.RequirementsHandler
{
    public class ScenarioCost : IComparable
    {
        public ScenarioCost(float cost)
        {
            Cost = cost;
        }

        public float Cost { get; set; } = float.PositiveInfinity;

        public int CompareTo(object obj)
        {
            if (!(obj is ScenarioCost)) 
                return 0;
            var targetCost = obj as ScenarioCost;

            return targetCost.Cost.CompareTo(this.Cost);
        }
    }
}