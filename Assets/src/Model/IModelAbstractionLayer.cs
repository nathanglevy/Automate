using System;
using System.Collections.Generic;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace Assets.src.Model
{
    public interface IModelAbstractionLayer
    {
        MovementPath GetMovementPath();
        List<String> GetPlayersInSelection(Coordinate notificationUpperLeft, Coordinate notificationBottomRight);
        Coordinate GetPlayerCoordinate(string guid);
    }
}