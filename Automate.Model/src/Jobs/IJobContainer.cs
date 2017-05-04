namespace Automate.Model.Jobs
{
    public interface IJobContainer
    {
        bool HasActiveJob { get; }
        bool HasCompletedJob { get; }
        bool HasJobInProgress { get; }
        RequirementJob CurrentJob { get; set; }
        void ClearCurrentJob();
    }
}