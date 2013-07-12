using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Water_adv
{
    class LinkableComponent : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new Water_adv();
        }
    }
}
