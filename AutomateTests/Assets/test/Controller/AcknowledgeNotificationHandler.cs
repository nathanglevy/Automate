﻿using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.AcknowledgeNotification;
using Automate.Controller.Interfaces;

namespace AutomateTests.test.Controller
{
    public class AcknowledgeNotificationHandler : Handler<ObserverArgs>, IHandler<ObserverArgs>
    {
        public override bool CanHandle(ObserverArgs args)
        {
            return args is AcknowledgeNotification;
        }

        public override IHandlerResult Handle(ObserverArgs args, IHandlerUtils utils)
        {
            var acknowledgeNotification = args as AcknowledgeNotification;
            if (acknowledgeNotification == null)
            {
                throw new ArgumentException("Args is Not AcknowledgeNotification");
            }
            
            // Get the Executed ACtion
            var executedAction = acknowledgeNotification.ExecutedAction;

            // invoke Acknowledge Master Method to acknowledge the sepecefic action and continue the flow
            utils.AcknowledgeHandler(executedAction);

            // return an empty result 
            // TOOD: is to return null better?, or some kind of empty result new type
            return new HandlerResult(new List<MasterAction>());
        }

        public override IAcknowledgeResult Acknowledge(MasterAction action, IHandlerUtils utils)
        {
            throw new NotImplementedException();
        }

        public override bool CanAcknowledge(MasterAction action)
        {
            throw new NotImplementedException();
        }
    }
}