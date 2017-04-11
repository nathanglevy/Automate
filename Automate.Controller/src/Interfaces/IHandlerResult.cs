using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public interface IHandlerResult<T>
    {
        IList<T> GetItems();

    }
}