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

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
            {
                throw new ArgumentException("args must be MockObserverArgs, current handler cannot Handle it");
            }

            var mockArgs = args as MockNotificationArgs;
            if (mockArgs != null)
            {

                var actions = new List<MasterAction>();
                var action =
                    new MockMasterAction(ActionType.AreaSelection, "00000000-0000-0000-0000-000000000001")
                    {
                        NeedAcknowledge = false
                    };
                actions.Add(action);
                var action2 =
                    new MockMasterAction(ActionType.Movement, "00000000-0000-0000-0000-000000000002")
                    {
                        NeedAcknowledge = true
                    };


                actions.Add(action2);
                return new HandlerResult(actions);
            }
            else
            {
                var actions = new List<MasterAction>();
                actions.Add(args as MockMasterAction);
                return new HandlerResult(actions);
            }

        }

        public override bool CanHandle(IObserverArgs args)
        {
            if (args is MockNotificationArgs || args is MockMasterAction)
                return true;
            return false;
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