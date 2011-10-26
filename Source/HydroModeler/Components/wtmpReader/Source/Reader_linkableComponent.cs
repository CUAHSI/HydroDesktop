using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WR_LC
{
    class Et_linkableComponent: Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new csvfileReader.WR();
        }
        public Et_linkableComponent()
        {
            _engineApiAccess = new csvfileReader.WR();
        }
    }
}
