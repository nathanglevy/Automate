using System;

namespace Automate.Model.PathFinding
{
    public class PathFindingException : Exception
    {
        public PathFindingException(string message) : base(message) { }
    }
}