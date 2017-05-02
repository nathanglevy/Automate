using System.Collections.Generic;
using System.Linq;

namespace Automate.Model.Requirements
{
    public class RequirementContainer
    {
        private List<IRequirement> _requirements = new List<IRequirement>();
        public IRequirement AddRequirement(IRequirement requirement) {
            _requirements.Add(requirement);
            return requirement;
        }

        public bool HasIncompleteRequirements()
        {
            return _requirements.Any(item => !item.IsSatisfied);
        }

        public List<IRequirement> GetIncompleteRequirements()
        {
            return _requirements.Where(item => !item.IsSatisfied).ToList();
        }

        public List<IRequirement> GetCompleteRequirements() {
            return _requirements.Where(item => item.IsSatisfied).ToList();
        }

        public List<IRequirement> GetAllRequirements() {
            return _requirements.Where(item => item.IsSatisfied || !item.IsSatisfied).ToList();
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