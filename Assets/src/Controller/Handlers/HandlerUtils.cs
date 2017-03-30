using Assets.src.Controller.Handlers;
using Assets.src.Controller.Interfaces;
using src.Model;

namespace AutomateTests.test.Controller
{
    public class HandlerUtils : IHandlerUtils
    {
        public HandlerUtils(IModelAbstractionLayer model, ViewCallBack viewCallBack)
        {
            Model = model;
            Enqueue = viewCallBack;
        }

        public IModelAbstractionLayer Model { get; private set; }
        public ViewCallBack Enqueue { get; private set; }
    }
}