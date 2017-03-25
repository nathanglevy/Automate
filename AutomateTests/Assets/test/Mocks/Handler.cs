using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Handlers;
using Assets.src.Controller.Interfaces;

namespace AutomateTests.test.Mocks
{

    public class MockHandler : Handler<ObserverArgs>
    {

        public List<MasterAction> Actions { get; private set; }

        public bool CanHandle<T>(T args) where T : ObserverArgs
        {
            if (args is MockObserverArgs)
                return true;
            return false;
        }

        public string Name
        {
            get { return this.GetType().ToString(); }
        }

        public override void Handle(ObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args)) return;

            MockObserverArgs mockArgs = args as MockObserverArgs;

            var action = new MockMasterAction(ActionType.AreaSelection, mockArgs.TargetId);
            var action2 = new MockMasterAction(ActionType.Movement, mockArgs.TargetId);
            var actions = new List<MasterAction>();
            actions.Add(action);
            actions.Add(action2);
            Actions = actions;

            utils.Enqueue(new HandlerResult(actions));

        }
    }
}