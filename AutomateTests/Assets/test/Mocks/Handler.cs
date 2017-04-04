using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Interfaces;
using AutomateTests.Mocks;

namespace AutomateTests.test.Mocks
{

    public class MockHandler : Handler<ObserverArgs>
    {

        public List<MasterAction> Actions { get; private set; }

        public bool CanHandle<T>(T args) where T : ObserverArgs
        {
            if (args is MockNotificationArgs)
                return true;
            return false;
        }


        public override IHandlerResult Handle(ObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("args must be MockObserverArgs, current handler cannot Handle it");
            }

            MockNotificationArgs mockArgs = args as MockNotificationArgs;

            var action = new MockMasterAction(ActionType.AreaSelection, "AhmadHamdan");
            var action2 = new MockMasterAction(ActionType.Movement, "NaphLevy");
            var actions = new List<MasterAction>();
            actions.Add(action);
            actions.Add(action2);
            Actions = actions;


            return new HandlerResult(actions);

        }

        public override IAcknowledgeResult Acknowledge(MasterAction action, IHandlerUtils utils)
        {
            throw new NotImplementedException();
        }

        public override bool CanAcknowledge(MasterAction action)
        {
            return action is MockMasterAction;
        }

        public override bool CanHandle(ObserverArgs args)
        {
            return args is MockNotificationArgs;

        }

        protected override Type GetInputType()
        {
            return typeof(MockObserverArgs);
        }
    }
}