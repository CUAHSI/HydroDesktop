using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Wrapper;

namespace SampleComponent2
{
    //This class wraps the SampleClass into an OpenMI compliant component!
    class SampleLinkableComponent:LinkableEngine
    {
        //Here we create a new instance of the Sample Class
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new SampleClass2();
        }
        public SampleLinkableComponent()
        {
            _engineApiAccess = new SampleClass2();
        }
    }
}
