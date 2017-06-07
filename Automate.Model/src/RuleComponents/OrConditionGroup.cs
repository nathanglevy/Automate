namespace Automate.Model.RuleComponents
{
    public class OrConditionGroup : ConditionGroup
    {
        public override ConditionType ConditionType { get; } = ConditionType.OrGroup;
    }
}