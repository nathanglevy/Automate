using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;

namespace Automate.Controller.Modules
{
    public delegate IList<ThreadInfo> HandleActivation(IObserverArgs args);

    public delegate IList<ThreadInfo> AcknowledgeActivation(MasterAction args);
    
    public delegate IList<ThreadInfo> TimedOut<T>(T args);
}