using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database;
using HydroDesktop.Database.Map;
using HydroDesktop.Database.Map.MetadataCache;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    [TestFixture]
    public class VariableTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanSaveVariable1()
        {
            int id = random.Next();
            Variable variable1 = CreateVariable(id);

            id += 1;
            Variable variable2 = CreateVariable(id);

            Assert.AreNotEqual(variable1, variable2, "Variable1 and Variable2 should not be equal");
            Assert.AreEqual(variable1.TimeUnit, variable2.TimeUnit, "The time units of both variables should be equal");

            //important: does not occur automatically.
            //the common reference needs to be checked and set
            variable1.TimeUnit = variable2.TimeUnit;

            Session.SaveOrUpdate(variable1);
            Session.SaveOrUpdate(variable2);

            var fromDb1 = Session.Get <Variable>(variable1.Id);
            var fromDb2 = Session.Get <Variable>(variable2.Id);

            Assert.AreNotEqual(variable1, variable2, "Variable1 and Variable2 should not be equal");
            Assert.AreEqual(variable1.TimeUnit, variable2.TimeUnit, "The time units of both variables should be equal");
        }

        [Test]
        public void CanSaveVariable2()
        {
            int id = random.Next();
            Variable variable1 = CreateVariable(id);

            id += 1;
            Variable variable2 = CreateVariable(id);

            Assert.AreNotEqual(variable1, variable2, "Variable1 and Variable2 should not be equal");
            Assert.AreEqual(variable1.TimeUnit, variable2.TimeUnit, "The time units of both variables should be equal");

            //important: does not occur automatically.
            //the common reference needs to be checked and set
            variable1.TimeUnit = variable2.TimeUnit;

            DataServiceInfo service = Session.Get<DataServiceInfo>(1L);
            
            Session.SaveOrUpdate(variable1);
            Session.SaveOrUpdate(variable2);

            var fromDb1 = Session.Get<Variable>(variable1.Id);
            var fromDb2 = Session.Get<Variable>(variable2.Id);

            Assert.AreNotEqual(variable1, variable2, "Variable1 and Variable2 should not be equal");
            Assert.AreEqual(variable1.TimeUnit, variable2.TimeUnit, "The time units of both variables should be equal");
        }

        public static Variable CreateVariable(int identifer)
        {
            int id = random.Next();

            Unit variableUnit = new Unit("cubic feet per second", "discharge", "cfs");
            Unit timeUnit = new Unit("day", "time", "d");

            Variable variable = new Variable();
            variable.Code = "code:" + id;
            variable.DataType = "dataType:" + id;
            variable.GeneralCategory = "generalCategory:" + id;
            variable.IsCategorical = false;
            variable.IsRegular = true;
            variable.Name = "name:" + id;
            variable.NoDataValue = -9999.0;
            variable.SampleMedium = "sampleMedium:" + id;
            variable.Speciation = "speciation:" + id;
            variable.TimeSupport = 1.0;
            variable.TimeUnit = timeUnit;
            variable.ValueType = "valueType:" + id;
            variable.VariableUnit = variableUnit;

            return variable;
        }

        public static Variable CreateVariable(DataServiceInfo service, int identifier)
        {
            Variable v = CreateVariable(identifier);
            //v.DataService = service;
            return v;
        }
    }
}