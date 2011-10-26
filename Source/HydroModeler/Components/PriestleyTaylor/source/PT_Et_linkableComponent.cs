using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PT_ET_LC
{
    class PT_Et_linkableComponent: Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new PT_Evapotranspiration.PT_PET();
        }
        public PT_Et_linkableComponent()
        {
            _engineApiAccess = new PT_Evapotranspiration.PT_PET();
        }
    }
}
