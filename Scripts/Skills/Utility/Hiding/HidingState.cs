using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.Skills.Utility.Hiding
{
    public enum HidingState
    {
        None = 0,
        TryingToHide = 1,    // We are in the process of hiding (that is, waiting GetHideTime()). Hiding may be interupted in this state.
        Sequencing = 2
    }
}
