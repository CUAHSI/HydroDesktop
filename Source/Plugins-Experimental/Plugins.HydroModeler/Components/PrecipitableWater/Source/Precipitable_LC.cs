using System;
using System.Collections.Generic;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;

namespace PrecipitableWater
{
    class Precipitable_LC : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        public Precipitable_LC()
        {
            _engineApiAccess = new PrecipMethod();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new PrecipMethod();
        }
    }
}