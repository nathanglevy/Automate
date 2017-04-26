using Automate.Controller.Abstracts;
using Automate.Controller.Handlers;

namespace Automate.Controller.Interfaces
{
    public class ControllerNotificationArgs
    {
        public IObserverArgs Args { get; }
        public IHandlerUtils Utils { get; set; }

        public ControllerNotificationArgs(IObserverArgs args)
        {
            Args = args;
        }
    }
}