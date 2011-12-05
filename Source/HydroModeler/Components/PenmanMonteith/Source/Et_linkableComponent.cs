using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eT_LC
{
    class et_linkableComponent: Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new Evptrnsprtion.eT();
        }
        public et_linkableComponent()
        {
            _engineApiAccess = new Evptrnsprtion.eT();
        }
    }
}
