namespace Automate.Model.RuleComponents
{
    public interface IConditionItem
    {
        ConditionType ConditionType { get; }
    }

    public enum ConditionType
    {
        AndGroup, OrGroup, SingleCondition
    }
}