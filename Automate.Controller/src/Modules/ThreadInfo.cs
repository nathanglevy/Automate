﻿using System.Threading;

namespace Automate.Controller.Modules
{
    public class ThreadInfo
    {
        public ThreadInfo(AutoResetEvent syncEvent, Thread thread)
        {
            Thread = thread;
            SyncEvent = syncEvent;
        }

        public Thread Thread { get; private set; }
        public AutoResetEvent SyncEvent { get; private set; }
    }
}