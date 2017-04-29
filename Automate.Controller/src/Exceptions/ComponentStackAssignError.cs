using System;

namespace Automate.Controller.Exceptions
{
    public class ComponentStackAssignError:Exception
    {
        public ComponentStackAssignError(string message):base(message)
        {}
    }
}