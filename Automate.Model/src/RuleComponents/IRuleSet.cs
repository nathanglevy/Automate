using System.Collections.Generic;

namespace Automate.Model.RuleComponents
{
    public interface IRuleSet
    {
        void AddRule(IRule rule);
        void RemoveRule(IRule rule);
        void Clear(IRule rule);
        List<IRule> GetRules();
    }
}