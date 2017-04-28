﻿using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldInterface;
using UnityEngine.VR.WSA.WebCam;

namespace Automate.Controller.Handlers.GoAndPickUp
{
    public class PickUpActionHandler : Handler<IObserverArgs>
    {
        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("cannot Handle passed Args, only PickUpAction is allowed");

            // do the casting
            var pickUpAction = args as PickUpAction;

            // Get Game World
            var gameWorldItem= GameUniverse.GetGameWorldItemById(utils.GameWorldId);

            // Get the Component Stack Group in the Pickup Location - Source
            var sourceComponentStackGroup = gameWorldItem.GetComponentStackGroupAtCoordinate(pickUpAction.TargetDest);

            // Get the Movable Object - Target
            var movableItem = gameWorldItem.GetMovableItem(pickUpAction.MovableId);

            // TODO: HOW TO CHECK IF IT HAS COMPONENT STACK

            var componentStack = movableItem.ComponentStackGroup.AddComponentStack(pickUpAction.ComponentType, 0);
            // Transfer Amount from Source to Target
            sourceComponentStackGroup.TransferToStackGroup(pickUpAction.MovableId,movableItem.ComponentStackGroup,pickUpAction.ComponentType,pickUpAction.Amount);

            // Pick Up Operation Ended, Fire On Complete
            pickUpAction.FireOnComplete(new ControllerNotificationArgs(pickUpAction));

            return new HandlerResult(new List<MasterAction>() { pickUpAction });
        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args == null)
                throw new NullReferenceException("Args is null, cannot determine if Handler should be activated");
            return args is PickUpAction;
        }
    }
}