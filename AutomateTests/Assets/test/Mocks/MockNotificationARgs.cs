using System;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Assets.src.Model.MapModelComponents;

namespace AutomateTests.test.Mocks
{
    public class MockNotificationArgs :ObserverArgs
    {
        public MockNotificationArgs(Coordinate dropCoordinate, string targetId)
        {
            DropCoordinate = dropCoordinate;
        }

        public Coordinate DropCoordinate { get; }

    }
}