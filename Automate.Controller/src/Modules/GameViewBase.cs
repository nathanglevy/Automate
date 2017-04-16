using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Interfaces;
using Automate.Model.GameWorldInterface;
using Automate.Model.MapModelComponents;
using UnityEngine;


namespace Automate.Controller.Modules
{
    public class GameViewBase : IGameView
    {
        public event ViewUpdate OnUpdateStart;
        public event ViewUpdate OnUpdate;
        public event ViewUpdate OnUpdateFinish;
        public event ViewUpdate OnStart;

        

        public void PerformOnUpdate()
        {
            if (OnUpdate != null) OnUpdate.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnUpdateStart()
        {
            if (OnUpdateStart != null) OnUpdateStart.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnUpdateFinish()
        {
            if (OnUpdateFinish != null) OnUpdateFinish.Invoke(new ViewUpdateArgs());
        }

        public void PerformOnStart()
        {
            if (OnStart != null) OnStart.Invoke(new ViewUpdateArgs());
        }

        public void PerformCompleteUpdate()
        {
            /////////////////////
            PerformOnUpdateStart();


            //////////////////////////////
            PerformOnUpdate();

            foreach (var action in PullFromController())
            {
                HandleAction(action);    
            }


            //////////////////////
            PerformOnUpdateFinish();

        }

        public IEnumerable<MasterAction> PullFromController()
        {
            while (Controller.OutputSched.HasItems)
            {
                var action = Controller.OutputSched.Pull();
                yield return action;
            }
        }

        public IGameController Controller { get; set; }

        public void HandleAction(MasterAction action)
        {
            PerformOnActionReady(new ViewHandleActionArgs() {Action = action });
        }

       

        public event ViewHandleAction OnActionReady;
        public void PerformOnActionReady(ViewHandleActionArgs viewHandleArgs)
        {
            if (OnActionReady != null) OnActionReady(viewHandleArgs);
        }
    }
}