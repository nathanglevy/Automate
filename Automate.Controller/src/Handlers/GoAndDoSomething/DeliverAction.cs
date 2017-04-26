using System;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class DeliverAction : PickUpAction
    {
        public Coordinate DeliveryPickup { get; }

        public DeliverAction(Coordinate DeliverDest, Coordinate DeliveryPickup, int amount, Guid deliverAuthorizedId) : base(DeliverDest,amount,deliverAuthorizedId)
        {
            this.DeliveryPickup = DeliveryPickup;
        }

        public Guid TargetId { get; }
        public event ControllerNotification OnComplete;
    }
}