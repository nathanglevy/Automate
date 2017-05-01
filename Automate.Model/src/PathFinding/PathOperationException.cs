using System;

namespace Automate.Model.PathFinding
{
    public class PathOperationException : Exception {
        public PathOperationException(string message) : base(message) { }
    }
}