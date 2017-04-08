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
using Automate.Controller.Modules;
using Automate.Model;
using Automate.Model.MapModelComponents;
using AutomateTests.test.Mocks;

namespace IntegrationTests
{
    class Program
    {
        private static void Main(string[] args)
        {
            IModelAbstractionLayer model = new MockGameModel();
            var gameController = new GameController(null, model);

            // Select all Game World
            var selection = new ViewSelectionNotification(new Coordinate(0, 0, 0), new Coordinate(20, 20, 20), "ID");
            gameController.Handle(selection);

            // Right Click on Some Point to start moving the players
            Coordinate target = new Coordinate(13, 7, 1);
            Console.Out.WriteLine("Moving Players To Target:" + target.ToString());
            var rightClickNotification = new RightClickNotification(target);
            gameController.Handle(rightClickNotification);

            Thread.Sleep(2000);

            while (gameController.OutputSched.HasActions)
            {
                // get the action from sched to handle on view side
                var masterAction = gameController.OutputSched.Pull();
                if (masterAction == null)
                    Thread.Sleep(1000);
                var move = (masterAction as MoveAction);
                if (move != null)
                    Console.Out.WriteLine("Move Action: {0} To: {1}", masterAction.TargetId, move.To);

                // acknoledge
                gameController.Handle(new AcknowledgeNotification(masterAction));
            }

            Console.Out.WriteLine("Process Finished.");
            Thread.Sleep(10000);
        }
    }
}
