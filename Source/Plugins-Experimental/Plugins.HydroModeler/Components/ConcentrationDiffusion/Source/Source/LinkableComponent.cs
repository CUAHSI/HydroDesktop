using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sediment_Diff
{
    class LinkableComponent :Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new Sediment_Diff();
        }
    }
}
 
