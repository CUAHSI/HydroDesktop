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
    public class UnitsTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapUnit()
        {
            int id = random.Next();
          
            string unitsType = "Velocity";

            new PersistenceSpecification<Unit>(Session)
                .CheckProperty(p => p.Name, "Unit" + id)
                .CheckProperty(p => p.Abbreviation, "ft/s")
                .CheckProperty(p => p.UnitsType, unitsType)

                .VerifyTheMappings();
        }

        public static Unit CreateUnit()
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

        //[Test]
        //public void CanMapUnitValue()
        //{
        //    int id = random.Next(); 
            
        //    UnitTypeCv unitType = new UnitTypeCv();
        //    unitType.Name = "Time";
        //    Unit unit = new Unit
        //                    {
        //                        Name = "day",
        //                        Abbreviation = "d",
        //                        Type = unitType
        //                    };
        //    new PersistenceSpecification<UnitValue>(Session)
        //          .CheckProperty(p => p.Unit, unit)
                 
        //           .CheckProperty(p => p.Value, Convert.ToDecimal(id))

        //           .VerifyTheMappings();
        //}
    }
}