using System.Reflection;
using NUnit.Framework;

namespace HydroDesktop.DataDownload.Tests
{
    static class TestHelper
    {
        public static void AssertPropertyExists<T>(string propertyName)
        {
            Assert.NotNull(
              typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public),
              "Class {0} doesn't have property with name: {1}", typeof(T).FullName,
              propertyName);
        }

    }
}
