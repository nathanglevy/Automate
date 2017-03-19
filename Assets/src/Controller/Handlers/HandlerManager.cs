
using System;
using System.Collections.Generic;
using Assets.src.Controller.Abstracts;
using Assets.src.Controller.Interfaces;
using JetBrains.Annotations;

namespace Assets.src.Controller
{
    public class HandlersManager : IHandlersManager
    {
        [NotNull] private readonly  List<IHandler> _handlers;
        private int _handlersCount = 0;

        public HandlersManager()
        {
            _handlers = new List<IHandler>();
        }

        public int GetHandlersCount()
        {
            return _handlersCount;
        }

        public void AddHandler(IHandler handler)
        {
            _handlersCount++;
            _handlers.Add(handler);
            
        }

        public List<MasterAction> Handle(IObserverArgs args)
        {
            var results = new List<MasterAction>();
            foreach (var handler in _handlers)
            {
                if (!handler.isApplicable(args)) continue;
                var result = handler.Handle(args);
                results.AddRange(result);
            }
            return results;

        }
    }
}