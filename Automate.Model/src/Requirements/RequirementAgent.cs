using System.Collections.Generic;
using System.Linq;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
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

        public List<IStructure> GetStructuresWithActiveJobs()
        {
            return _gameWorld.GetStructuresList().Where(item => item.HasJobInProgress).ToList();
        }

        public List<IStructure> GetStructuresWithCompletedJobs() {
            return _gameWorld.GetStructuresList().Where(item => item.HasCompletedJob).ToList();
        }

        public void SetConstructionJob(IStructure structure)
        {
            if (structure.HasJobInProgress)
                throw new RequirementException("Structure already has a job in progress, cannot define a new job requirement!");
            StructureJob structureJob = new StructureJob(JobType.Construction);
            foreach (KeyValuePair<Component, int> costPair in StructureCost.GetStructureCost(structure.StructureType))
            {
                structureJob.AddRequirement(new ComponentDeliveryRequirement(costPair.Key,costPair.Value));
                structureJob.AddRequirement(new WorkRequirement(costPair.Key,costPair.Value));
            }
            structure.CurrentJob = structureJob;
        }
    }
}