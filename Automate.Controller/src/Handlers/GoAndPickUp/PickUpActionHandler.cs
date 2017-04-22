using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class PickUpActionHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("cannot Handle passed Args, only PickUpAction is allowed");

            var pickUpAction = args as PickUpAction;
            var gameWorldItem = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
            var componentStack = gameWorldItem.GetComponentsAtCoordinate(pickUpAction.TargetDest);
            componentStack.RemoveAmount(pickUpAction.Amount);

            // Pick Up Operation Ended, Fire On Complete
            pickUpAction.FireOnComplete(new ControllerNotificationArgs(pickUpAction));

            return new AcknowledgeResult(new List<MasterAction>() { pickUpAction });
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is PickUpAction;
        }
    }
}