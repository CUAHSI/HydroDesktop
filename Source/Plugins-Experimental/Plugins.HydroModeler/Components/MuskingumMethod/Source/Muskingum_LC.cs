using System;
using System.Collections.Generic;
using System.Text;

namespace edu.SC.Models.Routing
{
    public class Muskingum_LC : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        public Muskingum_LC()
        {
            _engineApiAccess = new Muskingum();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new Muskingum();
        }
    }
}
