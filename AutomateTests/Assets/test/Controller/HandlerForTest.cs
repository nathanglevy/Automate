using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Automate.Assets.src.Controller;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller
{
    public class HandlerForTest:IHandler
    {
        public bool CanHandle<T>(T args) where T : IObserverArgs
        {
            return true;
        }

        public List<MasterAction> Handle<T>(T args) where T : IObserverArgs
        {
            var masterActions = new List<MasterAction>();
            masterActions.Add(new MasterAction(ActionType.AreaSelection));
            return masterActions;

        }
    }
}