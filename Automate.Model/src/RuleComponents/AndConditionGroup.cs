namespace Automate.Model.RuleComponents
{
    public class AndConditionGroup : ConditionGroup
    {
        public override ConditionType ConditionType { get; } = ConditionType.AndGroup;
    }
}