using System.Collections.Generic;
using System.Linq;
using Automate.Model.CellComponents;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.Jobs;
using Automate.Model.StructureComponents;

namespace Automate.Model.Requirements
{
    public class RequirementAgent : IRequirementAgent
    {
        private GameWorld _gameWorld;

        internal RequirementAgent(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public List<IStructure> GetStructuresWithJobInProgress()
        {
            return _gameWorld.GetStructuresList().Where(item => item.HasJobInProgress).ToList();
        }

        public List<IStructure> GetStructuresWithCompletedJobs() {
            return _gameWorld.GetStructuresList().Where(item => item.HasCompletedJob).ToList();
        }

        public List<ICell> GetCellsWithJobInProgress()
        {
            return _gameWorld.GetCells.Where(item => item.HasJobInProgress).ToList();
        }

        public List<ICell> GetCellsWithCompletedJobs() {
            return _gameWorld.GetCells.Where(item => item.HasCompletedJob).ToList();
        }

        public void SetConstructionJob(IStructure structure)
        {
            if (structure.HasJobInProgress)
                throw new RequirementException("Structure already has a job in progress, cannot define a new job requirement!");
            RequirementJob requirementJob = new RequirementJob(JobType.Construction);
            foreach (KeyValuePair<Component, int> costPair in StructureCost.GetStructureCost(structure.StructureType))
            {
                requirementJob.AddRequirement(new ComponentDeliveryRequirement(costPair.Key,costPair.Value));
                requirementJob.AddRequirement(new WorkRequirement(costPair.Key,costPair.Value));
            }
            structure.CurrentJob = requirementJob;
        }
    }
}