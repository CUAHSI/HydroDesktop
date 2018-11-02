using System;
using System.Collections.Generic;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;

namespace GreenAmpt
{
    class GreenAmpt_LC : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        public GreenAmpt_LC()
        {
            _engineApiAccess = new GreenAmptMethod();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new GreenAmptMethod();
        }
    }
}
