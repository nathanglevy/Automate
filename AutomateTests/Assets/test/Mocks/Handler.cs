using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;
using Automate.Controller.Interfaces;
using AutomateTests.Mocks;

namespace AutomateTests.test.Mocks
{

    public class MockHandler : Handler<IObserverArgs>
    {

        public List<MasterAction> Actions { get; private set; }

        public bool CanHandle<T>(T args) where T : IObserverArgs
        {
            if (args is MockNotificationArgs)
                return true;
            return false;
        }


        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("args must be MockObserverArgs, current handler cannot Handle it");
            }

            MockNotificationArgs mockArgs = args as MockNotificationArgs;

            var action = new MockMasterAction(ActionType.AreaSelection, "AhmadHamdan") {NeedAcknowledge = false};
            var action2 = new MockMasterAction(ActionType.Movement, "NaphLevy") {NeedAcknowledge = true};
            var actions = new List<MasterAction>();
            actions.Add(action);
            actions.Add(action2);
            Actions = actions;


            return new HandlerResult(actions);

        }

        public override bool CanHandle(IObserverArgs args)
        {
            return args is MockMasterAction;
        }


        //public override IAcknowledgeResult<MasterAction> Acknowledge(MasterAction action, IHandlerUtils utils)
        //{
        //    List<MasterAction> actions =new List<MasterAction>();
        //    actions.Add(new MasterAction(action.Type,action.TargetId + "_ACK"));
        //    return new AcknowledgeResult(actions);
        //}

        //public override bool CanAcknowledge(MasterAction action)
        //{
        //    return action is MockMasterAction;
        //}

       

   
    }
}