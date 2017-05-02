using System.Collections.Generic;
using System.Linq;
using Automate.Model.GameWorldComponents;

namespace Automate.Model.Requirements
{
    public class RequirementHelper : IRequirementGetter
    {
        private GameWorld _gameWorld;

        private RequirementHelper(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public List<IStructure> GetStructuresWithActiveJobs()
        {
            return _gameWorld.GetStructuresList().Where(item => item.HasActiveJob && !item.HasCompletedJob).ToList();
        }
    }

    public interface IRequirementGetter
    {
    }
}