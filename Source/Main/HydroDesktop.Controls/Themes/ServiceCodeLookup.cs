using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Controls.Themes
{
    public static class ServiceCodeLookup
    {
        /// <summary>
        /// Helper class to fix issues with site code network prefix / service code 
        /// mismatch
        /// </summary>
        /// <param name="siteCodePrefix">the site code network prefix (like NWISIID:)</param>
        /// <returns>the correct service code</returns>
        public static string GetServiceCode(string siteCodePrefix)
        {
            switch (siteCodePrefix)
            {
                case "NLDAS_MOS0125_H":
                    return "NLDAS_MOS0125_H";
                case "GW_EDWARDS":
                    return "GW_EDWARDS";
                default:
                    return siteCodePrefix;
            }
        }
    }
}
