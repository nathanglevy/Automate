using System;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers
{
    public interface IHandlerUtils    
    {
        Guid GameWorldId { get;  }
        HandleActivation InvokeHandler { get;  }
        AcknowledgeActivation AcknowledgeHandler { get; }
    }
}