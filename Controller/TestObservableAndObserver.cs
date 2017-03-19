using Assets.src.Controller;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using Automate.Assets.src.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IObserverArgs = Assets.src.Controller.IObserverArgs;

namespace AutomateTests.test.Controller {
    [TestClass()]
    public class TestObservableAndObserver {
        private static HandlersManager _handlersManager;

        [TestInitialize]
        public void Init()
        {
            _handlersManager = new HandlersManager();

        }

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IPrimaryObserver selectionObservable = new PrimaryObserver();
            Assert.IsNotNull(selectionObservable);
        }

        [TestMethod]
        public void TestRegisterObserver_GetCountExpect0()
        {
            IPrimaryObserver selectionObservable = new PrimaryObserver();
            Assert.AreEqual(0, selectionObservable.GetObserversCount());

        }


        [TestMethod]
        public void TestRegisterObserver_ExpectCount1()
        {
            IPrimaryObserver selectionObservable = new PrimaryObserver();
            ISecondryObserver secondryObserver = new SecondryObserver();
            selectionObservable.RegisterObserver(secondryObserver);
            Assert.AreEqual(1, selectionObservable.GetObserversCount());
        }

        [TestMethod]
        public void TestGetObserverInvokeCount_Expect0()
        {
            ISecondryObserver observer = new SecondryObserver();
            Assert.AreEqual(0,observer.GetNotificationCount());
        }

        [TestMethod]
        public void TestGetHandlersManager_ExpectNotNull()
        {
            ISecondryObserver observer = new SecondryObserver();
            Assert.IsNotNull(observer.GetHandlersManager());
        }
        [TestMethod]
        public void TestNotifyObserver_ExpectNotificationCount1()
        {
            ISecondryObserver observer = new SecondryObserver();
            observer.Notify(new ObserverArgs());
            Assert.AreEqual(1, observer.GetNotificationCount());
        }


        [TestMethod]
        public void TestSingleObserverInvoke_ExpectInvokes1()
        {
            IPrimaryObserver selectionObservable = new PrimaryObserver();
            ISecondryObserver secondryObserver = new SecondryObserver();
            selectionObservable.RegisterObserver(secondryObserver);
            var invokeArgs = new ObserverArgs();
            selectionObservable.Invoke(invokeArgs);
            Assert.AreEqual(1,secondryObserver.GetNotificationCount());

        }


    }
}