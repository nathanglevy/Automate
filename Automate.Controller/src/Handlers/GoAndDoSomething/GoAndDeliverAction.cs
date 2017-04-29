using System;
using Automate.Controller.Abstracts;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class GoAndDeliverAction : GoAndDoSomethingAction
    {
        public ComponentType ComponentType { get; }

        public GoAndDeliverAction(ComponentType componentType, Coordinate targetDestCoordinate, int amount, Guid movableGuid) : base(targetDestCoordinate, amount, movableGuid)
        {
            ComponentType = componentType;
        }

        protected override ActionType GetActionType()
        {
            return ActionType.Deliver;
        }
    }
}