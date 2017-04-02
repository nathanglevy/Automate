﻿using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model.src;

namespace Automate.Controller.Handlers
{
    public interface IHandlerUtils    
    {
        IModelAbstractionLayer Model { get;  }
        HandleActivation InvokeHandler { get;  }
        AcknowledgeActivation AcknowledgeHandler { get; }
    }
}