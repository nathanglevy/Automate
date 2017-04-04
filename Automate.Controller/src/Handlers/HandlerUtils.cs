using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;
using Model;

namespace Automate.Controller.Handlers
{
    public class HandlerUtils : IHandlerUtils
    {
        public HandlerUtils(IModelAbstractionLayer model,HandleActivation invokeHandle,AcknowledgeActivation ackowledgeHandler)
        {
            Model = model;
            InvokeHandler = invokeHandle;
            AcknowledgeHandler = ackowledgeHandler;
        }

        public IModelAbstractionLayer Model { get; private set; }
        public HandleActivation InvokeHandler { get; private set; }
        public AcknowledgeActivation AcknowledgeHandler { get; }
    }
}