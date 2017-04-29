using System;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class DeliverAction : PickUpAction
    {
        public Coordinate DeliveryPickup { get; }

        public DeliverAction(ComponentType componentType, Coordinate DeliverDest, Coordinate DeliveryPickup, int amount, Guid deliverAuthorizedId) : base(componentType, DeliverDest,amount,deliverAuthorizedId)
        {
            this.DeliveryPickup = DeliveryPickup;
        }

        public Guid TargetId { get; }

        public event ControllerNotification OnComplete;
    }
}