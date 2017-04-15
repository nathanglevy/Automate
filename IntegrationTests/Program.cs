using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Automate.Controller.Actions;
using Automate.Controller.Handlers.AcknowledgeNotification;
using Automate.Controller.Handlers.RightClockNotification;
using Automate.Controller.Handlers.SelectionNotification;
using Automate.Controller.Interfaces;
using Automate.Controller.Modules;
using Automate.Model;
using Automate.Model.GameWorldComponents;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using AutomateTests.Mocks;
using AutomateTests.test.Mocks;
using Timer = System.Timers.Timer;

namespace IntegrationTests
{
    class Program
    {
        private static GameController _gameController;
            
        public static volatile  bool _canPull;
        private   static void Main(string[] args)
        {
            GameWorldItem gameWorld = GameUniverse.CreateGameWorld(new Coordinate(11,11,2));
            gameWorld.CreateMovable(new Coordinate(2, 3, 0), MovableType.NormalHuman);
            gameWorld.CreateMovable(new Coordinate(5, 2, 0), MovableType.NormalHuman);

            IGameView view = new MockGameView();
            _gameController = new GameController(view);

            // Select all Game World
            var selection = new ViewSelectionNotification(gameWorld.GetWorldBoundary().topLeft, gameWorld.GetWorldBoundary().bottomRight, "ID");
            _gameController.Handle(selection);

            Thread.Sleep(100);

            // Right Click on Some Point to start moving the players
            Coordinate target = new Coordinate(7, 3, 0);
            Console.Out.WriteLine("Moving Players To Target:" + target.ToString());
            var rightClickNotification = new RightClickNotification(target);
            _gameController.Handle(rightClickNotification);

            var timer = new System.Threading.Timer(MimicViewupdate, null, 0, 100);


            _canPull = true;
            new Thread(delegate ()
            {
                while (_canPull)
                {
                    while (_gameController.OutputSched.HasItems)
                    {
                        try
                        {
                            var masterAction = _gameController.OutputSched.Pull();
                            var move = (masterAction as MoveAction);
                            if (move != null)
                                Console.Out.WriteLine("Move Action: {0} To: {1}", masterAction.TargetId, move.To);
                        }
                        catch (Exception e)
                        {

                            Console.Out.WriteLine("GOT EXCEPTION KILLED PULL THREAD");
                            Console.Out.WriteLine(e.Message);
                        }
                    }
                    Thread.Sleep(100);
                }
            }).Start();



           /* Thread.Sleep(2000);

            while (_gameController.OutputSched.HasItems)
            {
                // get the action from sched to handle on view side
                var masterAction = _gameController.OutputSched.Pull();
                if (masterAction == null)
                    Thread.Sleep(1000);
                var move = (masterAction as MoveAction);
                if (move != null)
                    Console.Out.WriteLine("Move Action: {0} To: {1}", masterAction.TargetId, move.To);

                // acknoledge
                _gameController.Handle(new AcknowledgeNotification(masterAction));
            }*/
            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();

            Console.WriteLine("Move Target Now to 7,18.2");
            _gameController.Handle(selection);

            var rightClickNotification2 = new RightClickNotification(new Coordinate(3, 8, 0));
            _gameController.Handle(rightClickNotification2);

            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();

            _canPull = false;
            timer.Dispose();

        }

        private static void MimicViewupdate(object state)
        {
            _gameController.View.PerformOnUpdate();
        }

    }
}
