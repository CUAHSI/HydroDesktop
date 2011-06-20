using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Database.Tests
{
    public class TestHelpers
    {
        public static Random random = new Random();
        
        public static DateTime LateDateTime()
        {
            return new DateTime(2099, 01,01);
        }

        public static DateTime MinSqlDateTime()
        {
            return new DateTime(1753, 01, 01);
        }

        public static double RandomLatitude()
        {
            return Math.Round((random.NextDouble() * 180 - 90), 7);
        }

        public static double RandomLongitude()
        {
            return Math.Round((random.NextDouble() * 360 - 180),7);
        }

        public static DateTime CurrentDateTime()
        {
            return DateTime.Now.Date + new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
    }
}
