using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;

namespace Assets.src.Controller.Handlers.RightClickNotification
{
    public class RightClickNotificationHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
        public override IHandlerResult Handle(ObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("Args Must Be from the Type RightClickNotfication, please make sure your use the CanHandle Method");
            }

            // Get the RightClock Object
            RightClickNotification rightNotification = args as RightClickNotification;

            // Get All Selcted Objects
            List<string> selectedMovables = utils.Model.GetSelectedMovables();

            foreach (var selectedMovable in selectedMovables)
            {
//                utils.Model.GetMovableNextMovement();

            }
return null;
            


        }
    }
}