using System;

namespace Automate.Controller.Abstracts
{
    public class ModelMasterAction : MasterAction
    {
        public ModelMasterAction(ActionType type, string targetId) : base(type, targetId)
        {
        }

        public ModelMasterAction(ActionType type, Guid targetId) : base(type, targetId)
        {
        }

        public ModelMasterAction(ActionType type) : base(type)
        {
        }

        public Guid MovableId { get; set; }
        public Guid MasterTaskId { get; set; }
    }
}