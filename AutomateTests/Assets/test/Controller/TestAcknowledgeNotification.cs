using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.AcknowledgeNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.src.MapModelComponents;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestAcknowledgeNotification
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            ObserverArgs args = new AcknowledgeNotification(null);
            Assert.IsNotNull(args);
        }

        [TestMethod]
        public void TestGetExecutedActionOnView_ExpectNotNull()
        {
            MoveAction executedAction = new MoveAction(new Coordinate(1,0,0),"MyId");
            AcknowledgeNotification args = new AcknowledgeNotification(executedAction);
            Assert.IsNotNull(args.ExecutedAction);
        }

        [TestMethod]
        public void TestCreateNewHandler_ExpectPass()
        {
            var handler = new AcknowledgeNotificationHandler();
            Assert.IsNotNull(handler);
        }

//        [TestMethod]
//        public void TestAcknowledgeHandleDelegationToOtherHandler_ExpectMasterActionWithPlayerMovment()
//        {
//            
//            IHandlerUtils utils = new HandlerUtils(new MockGameModel(),testingSnifferMethod,ackknowledgeMethod);
//            var handler = new AcknowledgeNotificationHandler();
//            AcknowledgeNotification acknowledgeNotificationArgs = new AcknowledgeNotification(new MoveAction(new Coordinate(5,5,5),new Coordinate(3,3,3),"MyID"  ));
//            var handlerResult = handler.Handle(acknowledgeNotificationArgs, utils);
//            Assert.AreEqual(1,handlerResult.GetActions().Count);
//            Assert.AreEqual(ActionType.Movement,handlerResult.GetActions()[0].Type);
//            Assert.IsTrue(handlerResult.GetActions()[0] is MoveAction);
//
//        }


        private IList<ThreadInfo> ackknowledgeMethod(MasterAction args)
        {
            throw new System.NotImplementedException();
        }

        private IList<ThreadInfo> testingSnifferMethod(ObserverArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}
