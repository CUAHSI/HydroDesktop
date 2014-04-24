using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Data.ODMCVServiceClient.Infrastructure.SOAP
{
    class ODMCVSoapServiceClient : ServiceReference.ODMCV.ODMCVServiceSoapClient, IODMCVServiceClient
    {
        protected ODMCVSoapServiceClient()
        {
        }

        public static ODMCVSoapServiceClient CreateNew()
        {
            return new ODMCVSoapServiceClient();
        }

        #region Implementation of IODMCVServiceClient

        public new IEnumerable<Unit> GetUnits()
        {
            var units = base.GetUnits();
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
                        yield return unit;
                    }
                }
            }
        }

        #endregion
    }
}
