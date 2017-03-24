using Assets.src.Controller;
using Assets.src.Controller.Interfaces;
using Assets.src.Controller.Modules;
using Assets.src.Model.MapModelComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomateTests.Controller {
    [TestClass()]
    public class IntegrationTestController {


        [TestMethod]
        public void TestViewSelection_ExpectsListOfCells()
        {
            

            // init View Observable
            IPrimaryObserver selectionObservable = new PrimaryObserver();

            // Init HandlersManager fo the Observer
            var handlersManager = new HandlersManager();

            // init GameController Observer
            var observer = new SecondryObserver();

            // link GameController observer to the View Observable
            selectionObservable.RegisterObserver(observer);


            // Init HandlersManager fo the Observer
            var ModelHandlersManager = new HandlersManager();
//            ModelHandlersManager.AddHandler(new ModelSelectionHandler());

            // init Model Observer
            var ModelObserver = new SecondryObserver();

            var modelObservable = new PrimaryObserver();
            modelObservable.RegisterObserver(ModelObserver);

            // init Schudler Observerable
            var controllerScheduler = new ControllerScheduler(modelObservable);

            /// CREATE THE TEST CASE
            /// create a selection object
            var viewSelection = new SelectionArgs(new Coordinate(0, 0, 0), new Coordinate(10, 10, 0));

            // invoke the ViewObservable Args
            selectionObservable.Invoke(viewSelection);


            // 
            Assert.IsNotNull(selectionObservable);
        }

      

    }

}