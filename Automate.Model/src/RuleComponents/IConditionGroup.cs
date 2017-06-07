using System.Collections.Generic;
using NUnit.Framework;

namespace Automate.Model.RuleComponents
{
    public interface IConditionGroup : IConditionItem
    {
        List<IConditionItem> GetConditionList();
        void AddCondition(IConditionItem conditionItem);
        void RemoveCondition(IConditionItem conditionItem);
        void ClearConditions();
    }
}