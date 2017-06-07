namespace Automate.Model.RuleComponents
{
    public abstract class SingleCondition : ICondition
    {
        public ConditionType ConditionType { get; } = ConditionType.SingleCondition;
        public abstract bool CheckCondition();
    }
}