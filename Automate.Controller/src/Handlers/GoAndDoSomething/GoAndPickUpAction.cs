﻿using System;
using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.GoAndPickUp
{
 

    public class GoAndPickUpAction : GoAndDoSomethingAction  {
        public GoAndPickUpAction(Coordinate targetDestCoordinate, int amount, Guid movableGuid) : base(targetDestCoordinate, amount, movableGuid)
        {
        }

        protected override ActionType GetActionType()
        {
            return ActionType.PickUp;
        }
    }
}