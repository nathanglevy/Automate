using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;

namespace Assets.src.Controller
{

    public class MockHandler : IHandler
    {
        public MockObserverArgs _args;

        public List<MasterAction> Actions { get; private set; }

        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            _args = args as MockObserverArgs;
            var action = new MockMasterAction(ActionType.AreaSelection);
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