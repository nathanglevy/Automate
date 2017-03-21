using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;

namespace Assets.src.Controller
{

    public class MockHandler2 : IHandler
    {

        public List<MasterAction> Actions { get; private set; }

        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            var action = new MockMasterAction(ActionType.AreaSelection,args.Id);
            var actions = new List<MasterAction>();
            actions.Add(action);
            Actions = actions;
            return actions;
        }

        public bool isApplicable<T>(T args) where T : IObserverArgs
        {
            return true;
        }
    }
}