using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Wrapper;


namespace TopModel
{
    class Linkable_Component:Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new TopModel();
        }
    }
}
