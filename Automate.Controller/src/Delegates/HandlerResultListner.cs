using Automate.Controller.Interfaces;

namespace Automate.Controller.Delegates
{
    public delegate void HandlerResultListner<T>(IHandlerResult<T> handlerResult);
}