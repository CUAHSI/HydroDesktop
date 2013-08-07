using System;
using HydroDesktop.Database;

namespace HydroDesktop.Data.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Updating Units...");
            var connectionString = SQLiteHelper.GetSQLiteConnectionString(Properties.Resources.PathToDefaultDatabase);
            UnitConversions.UnitConverter.UpdateDefaultUnitsFromWeb(connectionString);

            Console.WriteLine("Finished. Press any key to continue...");
            Console.ReadKey();
        }
    }
}
