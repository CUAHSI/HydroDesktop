using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database;
using HydroDesktop.Database.Map;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    [TestFixture]
    [Category("Mapping.Data")]
    public class VariableTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapVariable()
        {
            int id = random.Next();

            Unit variableUnit = UnitsTest.CreateUnit();
            Unit timeUnit = UnitsTest.CreateTimeUnit();
            
            new PersistenceSpecification<Variable>(Session)
                .CheckProperty(p => p.Name, "VariableName" + id)
                .CheckProperty(p => p.Code, "Prefix:VariableCode" + id)
                .CheckProperty(p => p.DataType, "DataType" + id)
                .CheckProperty(p => p.SampleMedium, "Water")
                .CheckProperty(p => p.Speciation, "Speciation" + id)
                .CheckProperty(p => p.ValueType, "ValueType" + id)
                .CheckProperty(p => p.GeneralCategory, "Surface Water")
                .CheckProperty(p => p.IsCategorical, false)
                .CheckProperty(p => p.IsRegular, false)
                .CheckProperty(p => p.NoDataValue, -9999.0)
                .CheckProperty(p => p.TimeSupport, 1.0)
                .CheckReference(p => p.TimeUnit, timeUnit)
                .CheckReference(p => p.VariableUnit, variableUnit)
                .VerifyTheMappings();
        }

        [Test]
        public void CanMapVariable1()
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
            Session.Save(variable1.TimeUnit);
            Session.Save(variable1.VariableUnit);
            Session.Save(variable2.VariableUnit);

            Session.SaveOrUpdate(variable1);
            Session.SaveOrUpdate(variable2);

            var fromDb1 = Session.Get <Variable>(variable1.Id);
            var fromDb2 = Session.Get <Variable>(variable2.Id);

            Assert.AreNotEqual(variable1, variable2, "Variable1 and Variable2 should not be equal");
            Assert.AreEqual(variable1.TimeUnit, variable2.TimeUnit, "The time units of both variables should be equal");
        }

        public static Variable CreateVariable(int identifer)
        {
            int id = random.Next();

            Unit variableUnit = UnitsTest.CreateUnit();
            Unit timeUnit = UnitsTest.CreateTimeUnit();

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

        /// <summary>
        /// Returns one of three units. This adds some random, while avoiding a units table that is one per variable/series
        /// </summary>
        /// <returns></returns>
        public static Unit CreateUnit()
        {
            int id = random.Next(0,2);
            Unit u =  CreateRandomUnit(id);
            switch (id)
            {
                case  0:
                    {
                        u = DischargeTestUnit();
                        break;
                    }
                case 1:
                    {
                        u = PercentTestUnit();
                        break;
                    }
                case 2:
                    {
                        u = TemperatureTestUnit();
                        break;
                    }
                default:
                    {
                        break; 
                    }
            }

            return u;
        }

        public static Unit DischargeTestUnit()
        {
            int id = random.Next();
            Unit u = new Unit();
            u.Abbreviation = "CFS";
            u.Name = "Cubic Feet per Second";
            u.UnitsType = "flow";

            return u;
        }
        public static Unit PercentTestUnit()
        {
            int id = random.Next();
            Unit u = new Unit();
            u.Abbreviation = "%" ;
            u.Name = "Percent";
            u.UnitsType = "distance";

            return u;
        }
        public static Unit TemperatureTestUnit()
        {
            int id = random.Next();
            Unit u = new Unit();
            u.Abbreviation = "abbr_" + id;
            u.Name = "UnitName" + id;
            u.UnitsType = "distance";

            return u;
        }

        public static Unit CreateRandomUnit(int identifer)
        {
            int id = random.Next();
            Unit u = new Unit();
            u.Abbreviation = "abbr_" + id;
            u.Name = "UnitName" + id;
            u.UnitsType = "distance";

            return u;
        }
        public static Unit CreateTimeUnit()
        {
            int id = random.Next();
            Unit u = new Unit();
            u.Abbreviation = "d";
            u.Name = "day";
            u.UnitsType = "time";

            return u;
        }
    }
}