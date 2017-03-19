using System;
using Assets.src.Controller;

namespace AutomateTests.test.Mocks
{
    public class MockObserverArgs :IObserverArgs
    {
        public MockObserverArgs()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; protected set; }
    }
}