using System;

namespace Automate.Model.Requirements
{
    public class RequirementException : Exception
    {
        public RequirementException(string message) : base(message) { }
    }
}