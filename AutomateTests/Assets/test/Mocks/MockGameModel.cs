using System;
using System.Collections.Generic;
using Assets.src.Controller.Interfaces;
using Assets.src.Model;
using Assets.src.Model.MapModelComponents;
using Assets.src.Model.PathFinding;

namespace AutomateTests.Mocks
{
    public class MockGameModel : IModelAbstractionLayer
    {
        public MovementPath GetMovementPath()
        {
            return new MovementPath(new Coordinate(10,10,0));
        }

        public List<String> GetPlayersInSelection(Coordinate notificationUpperLeft, Coordinate notificationBottomRight)
        {
            List<string> ids = new List<string>();
            ids.Add("AhmadHamdan");
            ids.Add("NaphLevy");
            return ids;
        }



        public Coordinate GetPlayerCoordinate(string guid)
        {
            return new Coordinate(10, 10, 0);
        }
    }
}