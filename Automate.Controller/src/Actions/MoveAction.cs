using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Actions
{
    public class MoveAction : MasterAction
    {
        public Coordinate To { get; private set; }
        public Coordinate CurrentCoordiate { get; private set; }

        public MoveAction(Coordinate to, Coordinate currentCoordiate, string id) : this(to,currentCoordiate,new Guid(id))
        {
        }

        public MoveAction(Coordinate to, Coordinate currentCoordiate, Guid playerId) : base(ActionType.Movement,playerId)
        {
            To = to;
            CurrentCoordiate = currentCoordiate;
        }
    }
}