using System;
using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class PickUpAction : ModelMasterAction
    {
        public PickUpAction(Coordinate targetDest, int amount) : base(ActionType.PickUp,Guid.NewGuid())
        {
            TargetDest = targetDest;
            Amount = amount;
        }

        public int Amount { get; set; }
        public Coordinate TargetDest { get; set; }
    }
}