//--- System Assemblies ----
using System;
using System.Collections.Generic;
using System.Text;
///--- OpenMI Assemblies ----
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;


namespace edu.sc.Models.Routing
{
    public class SCSUnitHydrograph_LC :LinkableEngine
    {
        public SCSUnitHydrograph_LC()
        {
            _engineApiAccess = new SCSUnitHydrograph();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new SCSUnitHydrograph();
        }
    }
}
