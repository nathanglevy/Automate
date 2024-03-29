﻿using System;
using Automate.Controller.Abstracts;
using Automate.Controller.Actions;
using Automate.Controller.Handlers;
using Automate.Controller.Handlers.GoAndPickUp;
using Automate.Controller.Interfaces;
using Automate.Model.Components;
using Automate.Model.GameWorldComponents;
using Automate.Model.MapModelComponents;
using Automate.Model.Movables;
using Automate.Model.StructureComponents;
using Automate.Model.Tasks;
using AutomateTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Component = Automate.Model.Components.Component;

namespace AutomateTests.Controller
{
    [TestClass]
    public class TestPickUpActionHandler
    {
        private bool _onCompleteFired;

        [TestMethod]
        public void TestCreateNew_ShouldPass()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsNotNull(pickUpActionHandler);
        }

        [TestMethod]
        public void TestCreateNewPickUpAction_ShouldPass()
        {
            var pickUpAction = new PickUpAction(ComponentType.IronOre, new Coordinate(0, 0, 0), 150, Guid.NewGuid());
            Assert.IsNotNull(pickUpAction);
            Assert.AreEqual(new Coordinate(0,0,0),pickUpAction.TargetDest);
            Assert.AreEqual(150,pickUpAction.Amount);

        }

        [TestMethod]
        public void TestCanHandleWithCorrectArgument_ExpectTrue()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsTrue(pickUpActionHandler.CanHandle(new PickUpAction(ComponentType.IronOre, new Coordinate(0,0,0),100, Guid.NewGuid())));
        }

        [TestMethod]
        public void TestCanHandleWithInCorrectArgument_ExpectTrue()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsFalse(pickUpActionHandler.CanHandle(new MoveAction(new Coordinate(0, 0, 0), new Coordinate(0, 0, 0),Guid.Empty)));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCanHandleWithNullExpectToFail()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            Assert.IsFalse(pickUpActionHandler.CanHandle(null));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHandleNonCorrectAction_ExpectException()
        {
            var pickUpActionHandler = new PickUpActionHandler();
            var handlerResult = pickUpActionHandler.Handle(new MockMasterAction(ActionType.AreaSelection,Guid.NewGuid().ToString()),null);
        }
            
        [TestMethod]
        public void TestHandlePickUpAction_ExpectModelToBeUpdatedAndOnCompleteToBeFired()
        {
            var gameWorldItem = GameUniverse.CreateGameWorld(new Coordinate(5, 5, 1));
            var movableItem = gameWorldItem.CreateMovable(new Coordinate(3, 2, 0), MovableType.NormalHuman);
            var movableStack = movableItem.ComponentStackGroup.AddComponentStack(ComponentType.IronOre, 0);

            var structure = gameWorldItem.CreateStructure(new Coordinate(0, 0, 0), new Coordinate(1, 1, 1), StructureType.Basic);
            var componentsAtCoordinate = structure.ComponentStackGroup.AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            //            gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(0, 0, 0)).AddComponentStack(Component.GetComponent(ComponentType.IronOre), 100);
            //            var componentsAtCoordinate = gameWorldItem.GetComponentStackGroupAtCoordinate(new Coordinate(0, 0, 0)).GetComponentStack(ComponentType.IronOre);

            var newTask = gameWorldItem.TaskDelegator.CreateNewTask();
            gameWorldItem.TaskDelegator.AssignTask(movableItem.Guid,newTask);
            newTask.AddTransportAction(TaskActionType.PickupTask, new Coordinate(0,0,0), structure.ComponentStackGroup, Component.IronOre, 100 );
            var currentAction = newTask.GetCurrentAction();

            

            // allocate 70 units for movable
            componentsAtCoordinate.AssignOutgoingAmount(movableItem.Guid,70);
            movableStack.AssignIncomingAmount(movableItem.Guid,70);

            // create the pickup with same amount
            var pickUpAction = new PickUpAction(ComponentType.IronOre, new Coordinate(0,0,0),70, movableItem.Guid)
            {
                MasterTaskId =  newTask.Guid,
                MovableId          = movableItem.Guid,
                OnCompleteDelegate = OnCompleteSniffer,
            };

            // check before handle that stack has 100
            Assert.AreEqual(componentsAtCoordinate.CurrentAmount, 100);

            // Execute the PickUp Action
            var pickUpActionHandler = new PickUpActionHandler();
            var handlerResult = pickUpActionHandler.Handle(pickUpAction, new HandlerUtils(gameWorldItem.Guid));

            // Expect the 70 to be taken
            Assert.AreEqual(componentsAtCoordinate.CurrentAmount, 30);

            // Expect the Action to Fire ImOver
            Assert.IsTrue(_onCompleteFired);

        }



        private void OnCompleteSniffer(ControllerNotificationArgs args)
        {
            _onCompleteFired = true;
        }
    }
}
