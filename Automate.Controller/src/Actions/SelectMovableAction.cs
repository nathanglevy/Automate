using Automate.Controller.Abstracts;
using Automate.Model.src.MapModelComponents;

namespace Automate.Controller.Actions
{
    public class SelectMovableAction : MasterAction
    {
        public Coordinate Coordinate { get; private set; }

        public SelectMovableAction(Coordinate coordinate, string targetID) : base(ActionType.SelectPlayer,targetID)
        {
            Coordinate = coordinate;
        }
    }
}