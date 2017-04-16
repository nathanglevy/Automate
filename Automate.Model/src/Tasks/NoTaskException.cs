using System;

namespace Automate.Model.Tasks
{
    public class NoTaskException : Exception
    {
        public NoTaskException(string message) : base(message) { }
    }
}