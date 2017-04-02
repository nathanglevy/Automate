using Automate.Controller.Abstracts;

namespace Automate.Controller.Handlers.AcknowledgeNotification
{
    public class AcknowledgeNotification : ObserverArgs
    {

        public AcknowledgeNotification(MasterAction executedAction)
        {
            ExecutedAction = executedAction;
        }

        public MasterAction ExecutedAction { get; private set; }
    }
}