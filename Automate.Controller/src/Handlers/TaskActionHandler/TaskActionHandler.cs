using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Handlers.TaskActionHandler
{
    public class TaskActionHandler : Handler<IObserverArgs>
    {

        public override bool CanHandle(IObserverArgs args)
        {
            throw new NotImplementedException();
        }

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            throw new NotImplementedException();
        }

    }
}