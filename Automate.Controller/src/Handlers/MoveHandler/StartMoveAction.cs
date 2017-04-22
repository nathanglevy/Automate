using System;
using Automate.Controller.Actions;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.MoveHandler
{
    public class StartMoveAction : MoveAction
    {
        public StartMoveAction(Coordinate to, Coordinate current, Guid movableGuid) : base(to,current,movableGuid)
        {}
    }
}