namespace Automate.Model.GameWorldComponents
{
    public interface IJobContainer
    {
        bool HasActiveJob { get; }
        bool HasCompletedJob { get; }
        bool HasJobInProgress { get; }
        RequirementJob CurrentJob { get; set; }
    }
}