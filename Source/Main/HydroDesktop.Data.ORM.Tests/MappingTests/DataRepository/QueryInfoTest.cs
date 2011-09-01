using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    [TestFixture]
    [Category("Mapping.Data")]
    public class QueryInfoTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapQueryInfo()
        {
            int id = random.Next();

            DataServiceInfo service = CreateDataService();

            new PersistenceSpecification<QueryInfo>(Session)
                .CheckProperty(p => p.AuthenticationToken, "qyjfxyza")
                .CheckProperty(p => p.LocationParameter, "NWISDV:0001")
                .CheckProperty(p => p.VariableParameter, @"NWISDV:001/data type=average")
                .CheckProperty(p => p.BeginDateParameter, DateTime.Now.Date.AddYears(-1))
                .CheckProperty(p => p.EndDateParameter, DateTime.Now.Date)
                .CheckProperty(p => p.QueryDateTime, TestHelpers.CurrentDateTime())
                .CheckReference(p => p.DataService, service)
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveQueries()
        {
            QueryInfo query1 = CreateQueryInfo();
            QueryInfo query2 = CreateQueryInfo();
            query2.EndDateParameter = DateTime.Now.AddDays(-22);
            query2.DataService = query1.DataService;
            Session.Save(query1);
            Session.Save(query2);
            Session.Flush();

            QueryInfo query11 = Session.Get<QueryInfo>(query1.Id);
            QueryInfo query22 = Session.Get<QueryInfo>(query2.Id);

            Assert.AreNotEqual(query11, query22, "the two queries should be different.");
            Assert.AreEqual(query11.DataService, query22.DataService, 
                            "the two queries should share the same data service.");
        }

        public static DataServiceInfo CreateDataService()
        {
            int rnd = random.Next(100);
            DataServiceInfo service = new DataServiceInfo(@"http://example.com/test.asmx", "test" + rnd);
            service.ServiceCode = "nwisdv";
            service.ServiceTitle = "NWISDV";
            service.ServiceName = "WaterOneFlow";
            service.ServiceType = "web service type 1";
            service.Version = 1.0;
            service.Protocol = "SOAP";
            service.EndpointURL = @"http://example.com/test.asmx";
            service.DescriptionURL = service.EndpointURL + "?wsdl";
            service.NorthLatitude = TestHelpers.RandomLatitude();
            service.SouthLatitude = TestHelpers.RandomLatitude();
            service.EastLongitude = TestHelpers.RandomLongitude();
            service.WestLongitude = TestHelpers.RandomLongitude();
            service.Abstract = "this is the abstract";
            service.Citation = "this is the citation";
            service.ContactName = "this is contact name";
            service.ContactEmail = "mail@example.com";
            return service;
        }

        public static QueryInfo CreateQueryInfo()
        {
            int rnd = random.Next(200);
            QueryInfo query = new QueryInfo();
            query.AuthenticationToken = "xyujxsd";
            query.BeginDateParameter = DateTime.Now.Date.AddYears(-2);
            query.EndDateParameter = DateTime.Now.Date;
            query.LocationParameter = "prefix:siteCode" + rnd;
            query.VariableParameter = "variable:code" + (rnd * 2);
            query.QueryDateTime = TestHelpers.CurrentDateTime();
            query.DataService = CreateDataService();
            
            return query;
        }
    }
}