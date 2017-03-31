using System;
using Automate.Controller.Abstracts;
using Automate.Model.src.MapModelComponents;

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