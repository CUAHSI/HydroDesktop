//--- System Assemblies ----
using System;
using System.Collections.Generic;
using System.Text;
///--- OpenMI Assemblies ----
using Oatc.OpenMI.Sdk.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;



namespace edu.SC.Models.Infiltration
{
    public class SCSAbstractionMethod_LC : LinkableEngine
    {
        public SCSAbstractionMethod_LC()
        {
            _engineApiAccess = new SCSAbstractionMethod();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new SCSAbstractionMethod();
        }
    }
}
