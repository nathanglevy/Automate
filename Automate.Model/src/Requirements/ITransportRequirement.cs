using Automate.Model.Components;
using Automate.Model.Tasks;

namespace Automate.Model.Requirements {
    public interface ITransportRequirement : IRequirement {
        Component Component { get; }
    }
}