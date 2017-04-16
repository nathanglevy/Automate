using System;
using System.Collections.Generic;
using System.Threading;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Assets.test.Controller
{
    [TestClass]
    public class TestGameViewBase
    {
        private bool _onUpdateFired = false;
        private bool _onUpdateStartFired;
        private bool _onUpdateFinishFired;
        private bool _onStartFired;
        private int _handledActionsCount = 0;
        private bool _onActionReadyFired;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            IGameView view = new GameViewBase();
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void TestOnUpdateMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            view.OnUpdate += onUpdate;
            view.PerformOnUpdate();
            Assert.IsTrue(_onUpdateFired);
        }

        [TestMethod]
        public void TestOnUpdateStartMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            view.OnUpdateStart += onUpdateStart;
            view.PerformOnUpdateStart();
            Assert.IsTrue(_onUpdateStartFired);
        }

        [TestMethod]
        public void TestOnUpdateFinishMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            view.OnUpdateFinish += onUpdateFinish;
            view.PerformOnUpdateFinish();
            Assert.IsTrue(_onUpdateFinishFired);
        }

        [TestMethod]
        public void TestOnActionReadyMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            view.OnActionReady += onActionReadyFired;
            view.PerformOnActionReady(new ViewHandleActionArgs()
                {
                    Action = new MasterAction(ActionType.PlaceGameObject, Guid.NewGuid().ToString())
                }
            );
            Assert.IsTrue(_onActionReadyFired);
        }

        private void onActionReadyFired(ViewHandleActionArgs args)
        {
            _onActionReadyFired = true;
        }


        [TestMethod]
        public void TestOnStartMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            view.OnStart += onStart;
            view.PerformOnStart();
            Assert.IsTrue(_onStartFired);
        }

        [TestMethod]
        public void TestRegisterController_ExpectControllerToBeNotNull()
        {
            // init gameView and Controller
            IGameView view = new GameViewBase();
            var gameController = new GameController(view);

            Assert.IsNotNull(view.Controller);
        }

        [TestMethod]
        public void TestPullControllerJobs_ExpectMoreThanZero()
        {
            // init gameView and Controller
            IGameView view = new GameViewBase();
            var gameController = new GameController(view);

            // PerformOnStart to create the initial world
            view.PerformOnStart();

            // Perform Update to propogate all actions to Scheduler
            view.PerformOnUpdateStart();
            view.PerformOnUpdate();

            // check that there are actions to be pul
            Assert.IsTrue(gameController.OutputSched.HasItems);

            // pull the actions from the controller to the View
            IEnumerable<MasterAction> actions= view.PullFromController();
            var enumerator = actions.GetEnumerator();
            int counter = 0;
            while (enumerator.MoveNext())
            {
                counter++;
            }
            Assert.IsTrue(counter > 0);
        }

        [TestMethod]
        public void TestForwardActionToUnity_ExpectsameActionId()
        {
            // init gameView and Controller
            IGameView view = new GameViewBase();
            view.OnActionReady += onActionReady;

            var gameController = new GameController(view);

            // PerformOnStart to create the initial world
            view.PerformOnStart();

            // Perform Update to propogate all actions to Scheduler
            view.PerformCompleteUpdate();

            // check that there are actions to be pul
           // Assert.IsTrue(gameController.OutputSched.HasItems);

            // pull the actions from the controller to the View
            //IEnumerable<MasterAction> actions = view.PullFromController();

            //view.HandleAction(actions.GetEnumerator().Current);
            Assert.AreEqual(200, _handledActionsCount);
        }

        private void onActionReady(ViewHandleActionArgs args)
        {
            _handledActionsCount++;
        }

       

        private void onStart(ViewUpdateArgs args)
        {
            _onStartFired = true;
        }


        [TestMethod]
        public void TestOnCompleteUpdateMethodCall_ExpectNothing()
        {
            IGameView view = new GameViewBase();
            var gameController = new GameController(view);
            gameController.FocusGameWorld(GameUniverse.CreateGameWorld(new Coordinate(10,10,1)).Guid);
            view.OnUpdateStart += onUpdateStart;
            view.OnUpdate += onUpdate;
            view.OnUpdateFinish += onUpdateFinish;
            view.PerformCompleteUpdate();
            Assert.IsTrue(_onUpdateStartFired);
            Assert.IsTrue(_onUpdateFired);
            Assert.IsTrue(_onUpdateFinishFired);
        }


        private void onUpdateFinish(ViewUpdateArgs args)
        {
            _onUpdateFinishFired = true;
        }

        private void onUpdateStart(ViewUpdateArgs args)
        {
            _onUpdateStartFired = true;
        }

        private void onUpdate(ViewUpdateArgs args)
        {
            _onUpdateFired = true;
        }
    }
}
