using System.Collections.Generic;

namespace Automate.Model.RuleComponents
{
    public interface IRule
    {
        void AddCondition(IRuleCondition ruleCondition);
        void RemoveCondition(IRuleCondition ruleCondition);
        void ClearConditions();
        List<IRuleCondition> GetRuleConditions();
        void AddOnSatisfyConditionAction(IRuleOutcome ruleOutcome);
        void AddOnUnSatisfyConditionAction(IRuleOutcome ruleOutcome);
    }
}