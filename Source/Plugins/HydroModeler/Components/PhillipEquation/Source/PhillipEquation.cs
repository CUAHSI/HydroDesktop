using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;
using SMW;
using SharpMap.Layers;
using SharpMap.Data.Providers;
using SharpMap.Data;
namespace PhillipEquation
{
    public class PhillipEquation : SMW.Wrapper
    {
        public override void Finish()
        {
            throw new NotImplementedException();
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            //Get config file path defined in sample.omi
            string configFile = (string)properties["ConfigFile"];
        }

        public override bool PerformTimeStep()
        {
            throw new NotImplementedException();
        }
    }
}
