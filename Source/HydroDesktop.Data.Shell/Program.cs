using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Data.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var odmcvClient = new ServiceReference.ODMCV.ODMCVServiceSoapClient())
            {
                var units = odmcvClient.GetUnits();
                PopulateUnits(units);
                odmcvClient.Close();
            }

            Console.WriteLine("Finished. Press any key to continue...");
            Console.ReadKey();
        }

        private static void PopulateUnits(string units)
        {
            var unitsRepo = RepositoryFactory.Instance.Get<IUnitsRepository>(DatabaseTypes.SQLite, SQLiteHelper.GetSQLiteConnectionString(Properties.Resources.PathToDefaultDatabase));

            using (var textReader = new StringReader(units))
            using (var xmlReader = new XmlTextReader(textReader))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement() &&
                        string.Equals(xmlReader.Name, "Record", StringComparison.OrdinalIgnoreCase))
                    {
                        var unit = new Unit();

                        while (xmlReader.Read())
                        {
                            if (xmlReader.IsStartElement())
                            {
                                if (string.Equals(xmlReader.Name, "UnitsName", StringComparison.OrdinalIgnoreCase))
                                {
                                    unit.Name = xmlReader.ReadString();
                                }
                                else if (string.Equals(xmlReader.Name, "UnitsType", StringComparison.OrdinalIgnoreCase))
                                {
                                    unit.UnitsType = xmlReader.ReadString();
                                }
                                else if (string.Equals(xmlReader.Name, "UnitsAbbreviation",
                                                       StringComparison.OrdinalIgnoreCase))
                                {
                                    unit.Abbreviation = xmlReader.ReadString();
                                }
                            }
                            else
                            {
                                if (string.Equals(xmlReader.Name, "Record", StringComparison.OrdinalIgnoreCase))
                                    break;
                            }
                        }

                        if (!String.IsNullOrEmpty(unit.Name))
                        {
                            if (unitsRepo.GetByName(unit.Name) == null)
                            {
                                unitsRepo.AddUnit(unit);
                            }
                        }
                    }
                }
            }
        }
    }
}
