﻿using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using AutomateTests.test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IObserverArgs = Assets.src.Controller.IObserverArgs;
using MockObserverArgs = AutomateTests.test.Mocks.MockObserverArgs;

namespace AutomateTests.test.Controller
{
    [TestClass]
    public class TestController
    {
        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IPrimaryObserver viewObservable = new PrimaryObserver();
            IPrimaryObserver modelObservable = new PrimaryObserver();
            GameController gameController = new GameController(viewObservable,modelObservable,null,null);
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(gameController.ViewObservable);
            Assert.IsNotNull(gameController.ModelObservable);
        }

        [TestMethod]
        public void TestHandleViewObservableNotify_ExpectDelegationToModel()
        {

            // create View Observable, will be used by the controller to delegate actions to the view
            IPrimaryObserver controllerPrimaryObserverAtView = new PrimaryObserver();
            IGameView gameview = new MockGameView(controllerPrimaryObserverAtView);
          
            // create View Observable, will be used by the controller to delegate actions to the view
            IPrimaryObserver controllerPrimaryObserverAtModel = new PrimaryObserver();
            IGameModel gameModel = new MockGameModel(controllerPrimaryObserverAtModel);
            MockControllerObserver mockControllerObserver = new MockControllerObserver();
            controllerPrimaryObserverAtModel.RegisterObserver(mockControllerObserver);
            var mockHandler = new MockHandler();
            mockControllerObserver.HandlersManager.AddHandler(mockHandler);

            // Init the Controller
            IGameController gameController = new GameController(
                gameview.GetViewObservable(), // Controller-->View
                gameModel.GetModelObservable(), // Controller-->Model
                controllerPrimaryObserverAtView, // View -->Controller
                controllerPrimaryObserverAtModel // Model --> Controller
                );

            // Add Mock Handler
//            gameController.

            IObserverArgs mockObserverArgs = new MockObserverArgs();
            controllerPrimaryObserverAtView.Invoke(mockObserverArgs);

            Assert.AreNotEqual(1,mockHandler.Actions.Count);
            var mockHandlerAction = mockHandler.Actions[0];
            Assert.AreEqual(ActionType.AreaSelection,mockHandlerAction.Type);
            Assert.AreEqual(mockHandlerAction.Id,mockObserverArgs.Id);
        }

    }
}
