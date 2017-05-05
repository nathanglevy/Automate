using System.Collections.Generic;
using Automate.Model.CellComponents;
using Automate.Model.GameWorldComponents;
using Automate.Model.StructureComponents;

namespace Automate.Model.Requirements
{
    public interface IRequirementAgent {
        List<IStructure> GetStructuresWithJobInProgress();
        List<IStructure> GetStructuresWithCompletedJobs();
        void SetConstructionJob(IStructure structure);
        List<ICell> GetCellsWithJobInProgress();
        List<ICell> GetCellsWithCompletedJobs();
    }
}