using System;
using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class GoAndPickUpAction : ModelMasterAction
    {
        public Coordinate TargetDestCoordinate { get; }
        public int Amount { get; }
        public Guid MovableGuid { get; }

        public GoAndPickUpAction(Coordinate targetDestCoordinate, int amount, Guid movableGuid) : base(
            ActionType.PickUp, Guid.NewGuid())
        {
            TargetDestCoordinate = targetDestCoordinate;
            Amount = amount;
            MovableGuid = movableGuid;
        }
    }
}