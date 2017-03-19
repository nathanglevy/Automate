using System.Collections.Generic;
using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Automate.Assets.src.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestingForHandlersManager 
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IHandlersManager handlersesManager = new HandlersManager();
            Assert.IsNotNull(handlersesManager);
        }

        [TestMethod]
        public void TestGetHandlers_Expect0()
        {
            IHandlersManager handlersManager = new HandlersManager();
            var count = handlersManager.GetHandlersCount();
            Assert.AreEqual(0,count);
        }

        [TestMethod]
        public void TestAdd2NewHandler_ExpectCount2()
        {
            IHandlersManager handlersManager = new HandlersManager();
            handlersManager.AddHandler(new HandlerForTest());
            handlersManager.AddHandler(new HandlerForTest());
            var count = handlersManager.GetHandlersCount();
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestHandleOfHandlers_ExpectListOfHandleResult()
        {
            IHandlersManager handlersManager = new HandlersManager();
            handlersManager.AddHandler(new HandlerForTest());
            handlersManager.AddHandler(new HandlerForTest());
            IObserverArgs args = new ObserverArgs();
            List<MasterAction> results = handlersManager.Handle(args);
            Assert.AreEqual(2,results.Count);
        } 


    }
}
