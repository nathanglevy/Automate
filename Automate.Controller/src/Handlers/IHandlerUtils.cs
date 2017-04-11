using System;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;

namespace Automate.Controller.Handlers
{
    public interface IHandlerUtils    
    {
        Guid GameWorldId { get;  }
        HandleActivation InvokeHandler { get;  }
        AcknowledgeActivation AcknowledgeHandler { get; }
    }
}