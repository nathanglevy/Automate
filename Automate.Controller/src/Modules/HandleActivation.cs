using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Modules
{
    public delegate IList<ThreadInfo> HandleActivation(ObserverArgs args);

    public delegate IList<ThreadInfo> AcknowledgeActivation(MasterAction args);
}