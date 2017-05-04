using System;
using Automate.Controller.Abstracts;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;

namespace Automate.Controller.Modules
{
    public class PlaceAGameObjectAction : MasterAction
    {
        public ItemType ItemType { get; }
        public Guid Id { get; }
        public Item Item { get; }
        public Coordinate Coordinate { get; }

        public PlaceAGameObjectAction(ItemType type, Guid guid, Item item, Coordinate coordinate) : base(ActionType.PlaceGameObject,guid.ToString())
        {
            ItemType = type;
            Id = guid;
            Item = item;
            Coordinate = coordinate;
        }
    }
}