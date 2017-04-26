using System;
using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Modules
{
    public class MovableErrorAction : MasterAction
    {
        public ErrorType Type { get; set; }
        public Coordinate CurrentCoordinate { get; set; }

        public MovableErrorAction(ActionType type, string targetId) : base(type, targetId)
        {
        }

        public MovableErrorAction(ActionType type, Guid targetId) : base(type, targetId)
        {
        }

        public MovableErrorAction(ActionType type) : base(type)
        {
        }
    }
}