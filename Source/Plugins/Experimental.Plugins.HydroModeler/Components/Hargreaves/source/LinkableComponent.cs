using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hargreaves.source
{
    class LinkableComponent : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        public LinkableComponent()
        {
            _engineApiAccess = new Hargreaves.Engine();
        }
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new Hargreaves.Engine();
        }
    }
}
