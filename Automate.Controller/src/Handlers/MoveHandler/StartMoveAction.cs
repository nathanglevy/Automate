using System;
using Automate.Controller.Actions;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.MoveHandler
{
    public class StartMoveAction : MoveAction
    {
        public StartMoveAction(Coordinate to, Guid movableGuid) : base(to, null, movableGuid)
        {
            IsNewMoveRequest = true;

        }
    }
}