using System;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;

namespace Automate.Controller.Handlers
{
    public class HandlerUtils : IHandlerUtils
    {
        public HandlerUtils(Guid gameWorldId,HandleActivation invokeHandle,AcknowledgeActivation ackowledgeHandler)
        {
            GameWorldId = gameWorldId;
            InvokeHandler = invokeHandle;
            AcknowledgeHandler = ackowledgeHandler;
        }

        public HandlerUtils(Guid gameWorldId): this(gameWorldId,null,null)
        {
        }

        public HandleActivation InvokeHandler { get; private set; }
        public AcknowledgeActivation AcknowledgeHandler { get; }
        public Guid GameWorldId { get; }
    }
}