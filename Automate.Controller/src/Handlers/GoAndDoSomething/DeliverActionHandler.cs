using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.GoAndDoSomething
{
    public class DeliverActionHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("cannot Handle passed Args, only PickUpAction is allowed");

            // Do a safe casting
            var deliverAction = args as DeliverAction;

            // Get Game World and Movable
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            var movableItem = gameWorldItem.GetMovableItem(deliverAction.MovableId);

            // Get Target Stack Group at Target Coordinate
            var componentStackGroup = gameWorldItem.GetComponentStackGroupAtCoordinate(deliverAction.TargetDest);

            // Get Target Component Stack for Transfer
            var componentStack = componentStackGroup.GetComponentStack(deliverAction.ComponentType);

            // Transfer Amount from Movable to ComponentStack
            movableItem.ComponentStackGroup.TransferToStack(deliverAction.MovableId, componentStack, deliverAction.Amount);

            // Pick Up Operation Ended, Fire On Complete
            deliverAction.FireOnComplete(new ControllerNotificationArgs(deliverAction, utils));

            return new HandlerResult(new List<MasterAction>() {deliverAction});
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is DeliverAction;
        }
    }
}