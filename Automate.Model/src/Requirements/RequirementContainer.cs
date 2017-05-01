using System.Collections.Generic;
using System.Linq;

namespace Automate.Model.Requirements
{
    public class RequirementContainer
    {
        private List<IRequirement> _requirements = new List<IRequirement>();
        public void AddRequirement(IRequirement requirement) {
            _requirements.Add(requirement);
        }

        public bool HasIncompleteRequirements()
        {
            return _requirements.Any(item => !item.IsSatisfied);
        }

        public IEnumerable<IRequirement> GetIncompleteRequirements()
        {
            return _requirements.Where(item => !item.IsSatisfied);
        }

        public IEnumerable<IRequirement> GetCompleteRequirements() {
            return _requirements.Where(item => item.IsSatisfied);
        }

        public IEnumerable<IRequirement> GetAllRequirements() {
            return _requirements.Where(item => item.IsSatisfied || !item.IsSatisfied);
        }

        public void RemoveRequirement(IRequirement requirementToClear) {
            _requirements.Remove(requirementToClear);
        }

        public void RemoveRequirements(List<IRequirement> requirementsToClear)
        {
            _requirements.RemoveAll(requirementsToClear.Contains);
        }

    }
}