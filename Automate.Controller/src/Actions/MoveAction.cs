using Automate.Controller.Abstracts;
using Automate.Model.src.MapModelComponents;

namespace Automate.Controller.Actions
{
    public class MoveAction : MasterAction
    {
        public Coordinate To { get; private set; }

        public MoveAction(Coordinate to, string playerID) : base(ActionType.Movement,playerID)
        {
            To = to;
        }


    }
}