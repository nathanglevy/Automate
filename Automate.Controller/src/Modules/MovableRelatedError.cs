using System;

namespace Automate.Controller.Modules
{
    public class MovableRelatedError : Exception
    {
        public MovableRelatedError(string message) : base(message)
        {
        }
    }
}