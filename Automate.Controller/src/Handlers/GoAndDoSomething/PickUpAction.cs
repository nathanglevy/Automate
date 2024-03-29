using System;
using Automate.Controller.Abstracts;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class PickUpAction : ModelMasterAction
    {
        public PickUpAction(ComponentType componentType, Coordinate targetDest, int amount, Guid pickUpperId) : base(ActionType.PickUp,pickUpperId)
        {
            TargetDest = targetDest;
            Amount = amount;
            MovableId = pickUpperId;
            ComponentType = componentType;
        }
        public Guid MovableId { get; set; }
        public int Amount { get; set; }
        public Coordinate TargetDest { get; set; }
    }
}