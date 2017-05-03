using System.Linq;
using Automate.Model.Requirements;

namespace Automate.Model.GameWorldComponents
{
    public class RequirementJob
    {
        public JobType JobType { get; }
        public int TotalPointsOfWorkRequired => JobRequirements.GetAllRequirements().Sum(item => item.TotalRequirement);
        public int PointsOfWorkDone => TotalPointsOfWorkRequired - PointsOfWorkRemaining;
        public int PointsOfWorkRemaining
            => JobRequirements.GetIncompleteRequirements().Sum(item => item.RequirementRemainingToSatisfy);
        public int PercentageDone => 100 * PointsOfWorkDone / TotalPointsOfWorkRequired;
        public RequirementContainer JobRequirements { get; } = new RequirementContainer();

        public void AddRequirement(IRequirement requirement) {
            JobRequirements.AddRequirement(requirement);
        }

        public RequirementJob(JobType jobType)
        {
            JobType = jobType;
        }
    }
}