using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HydroDesktop.WebServices;
using System.Reflection;

namespace HydroDesktop.WebServices.Tests
{
    [TestFixture]
    public class BuildAssemblyTest
    {
        
        [Test]
        [Ignore("Name is random so test will not work")]
        public void CanBuildAssembly()
        {
            Uri uri = new Uri(@"http://river.sdsc.edu/WaterOneFlow/NWIS/Data.asmx");
            Assembly asm = AssemblyBuilder.BuildAssemblyFromWSDL(uri);
            
            Assert.AreEqual(asm.Location, AssemblyBuilder.GetAssemblyFilePath(uri),
                "the assembly path should be same as the uri.");
        }
    }
}
