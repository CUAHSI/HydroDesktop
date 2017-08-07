This will need to be replaced with a mock that uses Hydrodesktop interfaces. But

From the [HISCentral Agent tests](http://his.codeplex.com/SourceControl/changeset/view/18b05703bdc7#ServicesTesting%2fr-u-on%2ftrunk%2fhiscentral%2fHisAgentTests%2fHisAgentTest.cs), we have three use cases
* All servers work
* All servers fail
* one server works, others fail.
We can easily model the first two. Note this uses NUnit Mocks, and not Moq.
Before these are called, two responses, good, and bad are setup. A good response is like a response from a working HIS central, and a bad is like  a response from a failed one. In addition, an object called "oneServer" is a list of Ur's with only one url in the list. 

In this case with mock objects, we are configuring methods to respond, and return, good or bad results
 {{          mockHisCentral.Expect("Endpoint", "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx");
}}
Says expect that the Property endpoint should be  the Url in the oneServer list (could be done better).
{{
            mockHisCentral.ExpectAndReturn("runQueryServiceList", good, "OneServer");
}}
Says to expect a call to runQueryServicesList, and return the good object.

{{
    ///A test for all working
        ///</summary>
        [Test()](Test())
        public void TestAgentUningMock_allWorking()
        {

            DynamicMock mockHisCentral = new DynamicMock(typeof(IHisCentralTester));
            mockHisCentral.Expect("Endpoint", "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx");
            mockHisCentral.ExpectAndReturn("runQueryServiceList", good, "OneServer");
            mockHisCentral.ExpectAndReturn("runServicesByBox", good, "OneServer");
            mockHisCentral.ExpectAndReturn("runSeriesCatalogByBox", good, "OneServer");
            mockHisCentral.ExpectAndReturn("runSearchableConcepts", good, "OneServer");
            mockHisCentral.ExpectAndReturn("runGetWordListNitrogen", good, "OneServer");

           HISCentralAgent target = new HISCentralAgent(null);
            target.MonitorIntervalSec = -1; 
           target.Tester = (IHisCentralTester) mockHisCentral.MockInstance;
            // TODO: Initialize to an appropriate value
            //HisCentralTestResult expected = null; // TODO: Initialize to an appropriate value
            HisCentralTestResult actual;
            var alarms = target.Monitor(oneServer.AsResource());
            Assert.IsFalse(alarms.Exists(AlarmIsCriticalServiceError)); 
            Assert.IsFalse(alarms.Exists(AlarmIsServiceError)); 

            Assert.IsFalse(alarms.Exists(AlarmIsServiceAllFailed));
            IAlarm criticalAlarm = alarms.Find( AlarmIsCritial);
             Assert.IsNull(criticalAlarm);
            //  Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for all failing. Should return a ServiceAllFailed alarm
        ///</summary>
        [Test()](Test())

        public void TestAgentUningMock_allFailing()
        {

            DynamicMock mockHisCentral = new DynamicMock(typeof(IHisCentralTester));
            mockHisCentral.Expect("Endpoint","http://hiscentral.cuahsi.org/webservices/hiscentral.asmx");
            mockHisCentral.ExpectAndReturn("runQueryServiceList", bad, "OneServer");
            mockHisCentral.ExpectAndReturn("runServicesByBox", bad, "OneServer");
            mockHisCentral.ExpectAndReturn("runSeriesCatalogByBox", bad, "OneServer");
            mockHisCentral.ExpectAndReturn("runSearchableConcepts", bad, "OneServer");
            mockHisCentral.ExpectAndReturn("runGetWordListNitrogen", bad, "OneServer");

            HISCentralAgent target = new HISCentralAgent(null);
            target.MonitorIntervalSec = -1;
            target.Tester = (IHisCentralTester)mockHisCentral.MockInstance;
            // TODO: Initialize to an appropriate value
            //HisCentralTestResult expected = null; // TODO: Initialize to an appropriate value
            HisCentralTestResult actual;
            var alarms = target.Monitor(oneServer.AsResource());
            Assert.IsFalse(alarms.Exists(AlarmIsCriticalServiceError)); 
            Assert.IsFalse(alarms.Exists(AlarmIsServiceError)); 

            Assert.IsTrue(alarms.Exists(AlarmIsServiceAllFailed));
            System.Collections.Generic.List<IAlarm> criticalAlarm = alarms.FindAll(AlarmIsCritial);
            Assert.That(criticalAlarm.Count ==1); // the service error is one
            
        }
}}