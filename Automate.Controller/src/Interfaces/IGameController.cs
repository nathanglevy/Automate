using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;

namespace Automate.Controller.Interfaces
{
    public interface IGameController
    {
        /// <summary>
        /// ID for the GameWorld under the controller focus
        /// </summary>
        Guid Model { get; }

        /// <summary>
        /// refrence to the GameViewBase Object associated with the controller
        /// </summary>
        IGameView View { get; }

        /// <summary>
        /// reference to the OutPut Schuduler
        /// </summary>
        IScheduler<MasterAction> OutputSched { get;}

        /// <summary>
        /// Handle Method used to Handle an action and convert it to other form/update Model/...
        /// </summary>
        /// <param name="args">the action to be handled</param>
        /// <returns>list of thread info (Thread and Sync) event </returns>
        IList<ThreadInfo> Handle(IObserverArgs args);


        /// <summary>
        /// event to be fired before start the handling of an action
        /// </summary>
        event ControllerNotification OnPreHandle ;

        /// <summary>
        /// event to be fired after finishing the Handle process of an action
        /// </summary>
        event ControllerNotification OnPostHandle ;

        /// <summary>
        /// event to be fired when the controller finish the handle and all other actions need to be performed during Handle
        /// </summary>
        event ControllerNotification OnFinishHandle ;


        /// <summary>
        /// returns the Handlers Count registered to the controller
        /// </summary>
        /// <returns></returns>
        int GetHandlersCount();

        /// <summary>
        /// Add a handler to the Handlers collections in Controller
        /// </summary>
        /// <param name="handler"></param>
        void RegisterHandler(IHandler<IObserverArgs> handler);

        /// <summary>
        /// returns if any GameWorld under the Controller Focus
        /// </summary>
        bool HasFocusedGameWorld { get; }

        /// <summary>
        /// Set Focus for a specific GameWorld
        /// </summary>
        /// <param name="gameWorldId"></param>
        void FocusGameWorld(Guid gameWorldId);

        /// <summary>
        /// unfocus from the already focused GameWorld
        /// </summary>
        /// <returns></returns>
        Guid UnfocusGameWorld();
    }
}
