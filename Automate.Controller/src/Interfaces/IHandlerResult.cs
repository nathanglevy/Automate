using System.Collections.Generic;
using Automate.Controller.Abstracts;

namespace Automate.Controller.Interfaces
{
    public interface IHandlerResult<T>
    {
        IList<T> GetItems();

        /// <summary>
        /// bool attribute to indicate if Handled Actions should be pushed to View or Just ReHandled
        /// </summary>
        bool IsInternal { get; set; }
    }
}