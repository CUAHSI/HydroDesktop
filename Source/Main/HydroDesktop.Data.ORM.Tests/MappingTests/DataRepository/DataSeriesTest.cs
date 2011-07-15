using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NHibernate;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.Database.Map;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
   // this class provides helpers, and calls the helpers further down

    public class DataSeriesTest : FixtureBase
    {
     
        public static Series CreateSeries(int identifier)
        {
            Series series = new Series();
            
            series.Site = CreateSite(identifier);
            series.Variable = CreateVariable(identifier);
            
            series.ValueCount = 0;
            series.BeginDateTime = DateTime.Now.AddYears(-10);
            series.BeginDateTimeUTC = series.BeginDateTime.ToUniversalTime();
            series.EndDateTime = DateTime.Now;
            series.EndDateTimeUTC = series.EndDateTime.ToUniversalTime();

            series.Method = Method.Unknown;
            series.Source = Source.Unknown;
            series.QualityControlLevel = QualityControlLevel.Unknown;

            return series;
        }

        public static void SaveSeries(ISession session, Series series)
        {
            SaveSite(session, series.Site);

            SaveVariable(session, series.Variable);
            session.Save(series);
        }

        public static Site CreateSite(int identifier)
        {
            return SitesTest.CreateSite(identifier);
        }

        public static void SaveSite(ISession session, Site site)
        {
            session.Save(site.SpatialReference);
            session.Save(site);
            ;
            
        }
        
        public static Variable CreateVariable(int identifier)
        {
            Variable variable = VariableTest.CreateVariable(identifier);
            return variable;
        }

        public static void SaveVariable(ISession session, Variable variable)
        {
            session.Save(variable.TimeUnit);
            session.Save(variable.VariableUnit);
            session.Save(variable);
            ;

        }

        public static void CreateDataServiceInfo(int identifier)
        {
            
        }
    }
}