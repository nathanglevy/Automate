using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class DeliverActionHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("cannot Handle passed Args, only PickUpAction is allowed");

            var deliverAction = args as DeliverAction;
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            var movableItem = gameWorldItem.GetMovableItem(deliverAction.MovableId);
            var componentStackGroup = gameWorldItem.GetComponentStackGroupAtCoordinate(deliverAction.TargetDest);
            //if (componentStack.GetOutgoingAllocatedAmountForGuid(pickUpAction.MovableId) != pickUpAction.Amount)
            //{

            //}
            movableItem.DeliverToComponentStackGroup(componentStackGroup, Component.GetComponent(ComponentType.IronOre), deliverAction.Amount );

            // Pick Up Operation Ended, Fire On Complete
            deliverAction.FireOnComplete(new ControllerNotificationArgs(deliverAction));

            return new HandlerResult(new List<MasterAction>() { deliverAction });
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is DeliverAction;
        }
    }
}