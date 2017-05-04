using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Handlers.PlaceAnObject
{
    public class PlaceAStrcutureRequest : PlaceAnObjectRequest
    {
        public PlaceAStrcutureRequest(Coordinate coordinate, Coordinate dim, StructureType structureType) : base(ItemType.Structure, coordinate)
        {
            Dim = dim;
            StructureType = structureType;
        }

        public Coordinate Dim { get; private set; }
        public StructureType StructureType { get; private set; }
    }
}