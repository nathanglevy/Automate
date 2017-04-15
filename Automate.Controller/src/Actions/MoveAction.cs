using Automate.Controller.Abstracts;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Actions
{
    public class MoveAction : MasterAction
    {
        public Coordinate To { get; private set; }
        public Coordinate CurrentCoordiate { get; private set; }

        public MoveAction(Coordinate to, Coordinate currentCoordiate, string playerID) : base(ActionType.Movement,playerID)
        {
            To = to;
            CurrentCoordiate = currentCoordiate;
        }


    }
}