using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N_SR_RC
{
    class N_SR_linkableComponent : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new N_SolarRadiation.N_SR();
        }
        public N_SR_linkableComponent()
        {
            _engineApiAccess = new N_SolarRadiation.N_SR();
        }
    }
}
