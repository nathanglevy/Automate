using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public abstract class GoAndDoSomethingAction : ModelMasterAction
    {
        public Coordinate TargetDestCoordinate { get; }
        public int Amount { get; }
        public Guid MovableGuid { get; }

        public GoAndDoSomethingAction(Coordinate targetDestCoordinate, int amount, Guid movableGuid) : base(
           ActionType.DEFAULT, Guid.NewGuid())
        {
            Type = GetActionType();
            TargetDestCoordinate = targetDestCoordinate;
            Amount = amount;
            MovableGuid = movableGuid;
        }

        protected abstract ActionType GetActionType();
    }
}