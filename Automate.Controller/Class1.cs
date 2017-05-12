using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automate.Model.Components;
using Automate.Model.Requirements;

namespace Automate.Controller
{
    public class Class1
    {
        private ComponentPickupRequirement _com = new ComponentPickupRequirement(Component.IronIngot, 100);

        Class1()
        {
            var comRequirementRemainingToSatisfy = _com.RequirementRemainingToDelegate;
        }
    }
}
