using Automate.Controller.Abstracts;
using Model.MapModelComponents;

namespace AutomateTests.Mocks
{
    public class MockNotificationArgs :ObserverArgs
    {
        public MockNotificationArgs(Coordinate dropCoordinate, string targetId)
        {
            DropCoordinate = dropCoordinate;
        }

        public Coordinate DropCoordinate { get; private set; }

    }
}