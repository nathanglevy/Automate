using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;

namespace Automate.Controller.Handlers.PlaceAnObject
{
    public class PlaceAnObjectRequestHandler : Handler<IObserverArgs>
    {

        public override IHandlerResult<MasterAction> Handle(IObserverArgs args, IHandlerUtils utils)
        {
            if (!CanHandle(args))
                throw new ArgumentException("Handler got non PlaceAnObjectRequset as args");

            try
            {
                var placeObjectArgs = args as PlaceAnObjectRequest;

                var gameWorld = GameUniverse.GetGameWorldItemById(utils.GameWorldId);
                switch (placeObjectArgs.ItemType)
                {
                    case ItemType.Movable:
                        var placeMovable = placeObjectArgs as PlaceAMovableRequest;
                        gameWorld.CreateMovable(placeMovable.Coordinate, placeMovable.MovableType);
                        break;
                    case ItemType.Structure:
                        var placeStruct = placeObjectArgs as PlaceAStrcutureRequest;
                        gameWorld.CreateStructure(placeStruct.Coordinate, placeStruct.Dim,
                            placeStruct.StructureType);
                        break;
                    case ItemType.Cell:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return new HandlerResult(new List<MasterAction>());
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to place an Object" + e.Message);
                throw e;
            }
        }

        public override bool CanHandle(IObserverArgs args)
        {
            return args is PlaceAStrcutureRequest || args is PlaceAMovableRequest;
        }

    }
}