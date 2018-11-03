using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Extensions;

namespace HydroDesktop.Plugins.MainPlugin
{
    public class SampleProjectInfo : ISampleProject
    {
        public string Name { get; set; }

        public string AbsolutePathToProjectFile { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }
    }
}
