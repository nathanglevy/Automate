using Automate.Controller.Abstracts;
using Model.MapModelComponents;

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