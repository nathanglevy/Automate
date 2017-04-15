﻿using System;
using System.Collections.Generic;
using Automate.Controller.Abstracts;
using Automate.Controller.Delegates;
using Automate.Controller.Modules;
using Automate.Model;

namespace Automate.Controller.Interfaces
{
    public interface IGameController
    {
        Guid Model { get; }
        IGameView View { get; }
        IScheduler<MasterAction> OutputSched { get;}
        bool HasFocusedGameWorld { get; }
        IList<ThreadInfo> Handle(ObserverArgs args);
        int GetHandlersCount();
        void RegisterHandler(IHandler<ObserverArgs> handler);
        void FocusGameWorld(Guid gameWorldId);
        Guid UnfocusGameWorld();
    }
}
