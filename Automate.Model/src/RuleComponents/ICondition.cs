using UnityEngine;

namespace Automate.Model.RuleComponents
{
    public interface ICondition : IConditionItem
    {
        bool CheckCondition();
    }
}