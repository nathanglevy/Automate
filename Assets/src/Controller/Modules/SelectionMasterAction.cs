using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.PathFinding.MapModelComponents;

namespace Assets.src.Controller.Modules
{
    public class SelectionMasterAction : MasterAction
    {
        public Coordinate UpperLeft { get; private set; }
        public Coordinate BottomRight { get; private set; }
        
        public SelectionMasterAction(Coordinate upperLeft, Coordinate bottomRight) : base(ActionType.AreaSelection)
        {
            UpperLeft = upperLeft;
            BottomRight = bottomRight;
        }


    }
}