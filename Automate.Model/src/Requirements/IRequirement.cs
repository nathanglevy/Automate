using System.Net.Configuration;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements
{
    public interface IRequirement : ITaskAttachable
    {
        bool IsSatisfied { get; }
        RequirementType RequirementType { get; }
        int RequirementRemainingToSatisfy { get; }
        int TotalRequirement { get; }
        bool SatisfyRequirement(int amount);
    }

    public enum RequirementType
    {
        ComponentDelivery,
        ComponentPickup,
        Environment,
        Work
    }
}