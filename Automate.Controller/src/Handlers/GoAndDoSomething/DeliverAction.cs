using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class DeliverAction : PickUpAction
    {
        public Coordinate DeliveryCoordinate { get; }

        public DeliverAction(ComponentType componentType, Coordinate DeliverDest, Coordinate deliveryCoordinate, int amount, Guid deliverAuthorizedId) : base(componentType, DeliverDest,amount,deliverAuthorizedId)
        {
            Type = ActionType.Deliver;
            this.DeliveryCoordinate = deliveryCoordinate;
        }

        public event ControllerNotification OnComplete;
    }
}