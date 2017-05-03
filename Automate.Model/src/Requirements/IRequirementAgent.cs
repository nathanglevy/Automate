using System.Collections.Generic;
using Automate.Model.GameWorldComponents;

namespace Automate.Model.Requirements
{
    public interface IRequirementAgent {
        List<IStructure> GetStructuresWithActiveJobs();
        List<IStructure> GetStructuresWithCompletedJobs();
        void SetConstructionJob(IStructure structure);
    }
}