using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Search;
using NUnit.Framework;
using NUnit.Mocks;
using System.Windows.Forms;
using HydroDesktop.Data.Plugins;

namespace SearchUnitTests
{
    

    [TestFixture()]
    public class HisCentralFailover
    {
        private DynamicMock HydroPlugInArgs ;
 
        private HydroDesktop.Search.SearchControl _searchForm;
        [SetUp]
        public void setup()
        {
            // we are not doing any map stuff, so the mock object does not need any config
            HydroPlugInArgs = new DynamicMock(typeof(IHydroPluginArgs));
 
            SplitContainer s = new SplitContainer();
            _searchForm = new SearchControl(
                (IHydroPluginArgs) HydroPlugInArgs.MockInstance,
                s);
        }

        //[TestCase("http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx")]
        //[TestCase("http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx?WSDL")]
        //public void GetWorkingHisCentral(string Url)
        //{
        //   var works= _searchForm.GetWorkingHisCentral("http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx");

        //   Assert.That(works, "Failed " + Url);
        //}

        //[TestCase("http://example.com/hiscentral/webservices/hiscentral.asmx")]
        //public void GetWorkingHisCentral_BadURL(string Url)
        //{
        //    var works = _searchForm.GetWorkingHisCentral("http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx");

        //    Assert.False(works, "Works when it should have failed " + Url);
        //}
    }
}
