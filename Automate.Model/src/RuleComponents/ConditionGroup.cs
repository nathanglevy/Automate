using System.Collections.Generic;

namespace Automate.Model.RuleComponents
{
    public abstract class ConditionGroup : IConditionGroup
    {
        public abstract ConditionType ConditionType { get; }
        private readonly List<IConditionItem> _conditions = new List<IConditionItem>();

        public List<IConditionItem> GetConditionList()
        {
            return new List<IConditionItem>(_conditions);
        }

        public void AddCondition(IConditionItem conditionItem)
        {
            _conditions.Add(conditionItem);
        }

        public void RemoveCondition(IConditionItem conditionItem)
        {
            _conditions.Remove(conditionItem);
        }

        public void ClearConditions()
        {
            _conditions.Clear();
        }

    }
}