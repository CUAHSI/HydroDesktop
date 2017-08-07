You can pass inputs, and multiple inputs to a NUnit unit test. Below are examples from the WaterWebServices Monitor

{{
        /// <summary>
        ///A test for RunTests
        ///</summary>
          [TestCase("http://river.sdsc.edu/wateroneflow/NWIS/DailyValues.asmx","NWIS:10263500", "NWIS:00060", "2010-01-01/2010-01-31")](TestCase(_http___river.sdsc.edu_wateroneflow_NWIS_DailyValues.asmx_,_NWIS_10263500_,-_NWIS_00060_,-_2010-01-01_2010-01-31_))
          [TestCase("http://river.sdsc.edu/wateroneflow/NWIS/UnitValues.asmx", "NWIS:10109000", "NWIS:00065", "P1D")](TestCase(_http___river.sdsc.edu_wateroneflow_NWIS_UnitValues.asmx_,-_NWIS_10109000_,-_NWIS_00065_,-_P1D_))

         public void RunTestsTest(string endpoint, string ws_SiteCode, string ws_variableCode, string ISOTimPeriod)
        {
            WaterWebSericesTester target = new WaterWebSericesTester(); // TODO: Initialize to an appropriate value
            target.Endpoint = endpoint;
           // TestResult expected = null; // TODO: Initialize to an appropriate value
            TestResult actual;
            actual = target.RunTests("TestServer",ws_SiteCode, ws_variableCode, ISOTimPeriod);

            Assert.IsTrue(actual.Working);
          //  Assert.Inconclusive("Verify the correctness of this test method.");
        }

          /// <summary>
          ///A test for RunTests
          ///</summary>
          [TestCase("http://example", "NWIS:10263500", "NWIS:00060", "2010-01-01/2010-01-31")](TestCase(_http___example_,-_NWIS_10263500_,-_NWIS_00060_,-_2010-01-01_2010-01-31_))
          [TestCase("http://river.sdsc.edu/wateroneflow/NWIS/UnitValues.asmx", "NWIS:00000", "NWIS:00065", "P1D")](TestCase(_http___river.sdsc.edu_wateroneflow_NWIS_UnitValues.asmx_,-_NWIS_00000_,-_NWIS_00065_,-_P1D_))
          [TestCase("http://river.sdsc.edu/wateroneflow/NWIS/UnitValues.asmx", "NWIS:00000", "NWIS:00065", "1D")](TestCase(_http___river.sdsc.edu_wateroneflow_NWIS_UnitValues.asmx_,-_NWIS_00000_,-_NWIS_00065_,-_1D_))
          public void BadDataTests(string endpoint, string ws_SiteCode, string ws_variableCode, string ISOTimPeriod)
          {
              WaterWebSericesTester target = new WaterWebSericesTester(); // TODO: Initialize to an appropriate value
              target.Endpoint = endpoint;
              // TestResult expected = null; // TODO: Initialize to an appropriate value
              TestResult actual;
              actual = target.RunTests("TestServer", ws_SiteCode, ws_variableCode, ISOTimPeriod);

              Assert.IsFalse(actual.Working);
              //  Assert.Inconclusive("Verify the correctness of this test method.");
          }
}}