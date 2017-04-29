using System;
using Automate.Model.Components;

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

        
        public Guid MasterTaskId { get; set; }
        public ComponentType ComponentType { get; protected set; }
    }
}