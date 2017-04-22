using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Automate.Controller.Modules
{
    public class AutomateLogHandler : ILogHandler
    {
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            Debug.logger.logHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            Debug.logger.LogException(exception, context);
        }
    }
}