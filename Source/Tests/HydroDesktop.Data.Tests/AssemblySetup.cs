using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace HydroDesktop.Data.Tests
{
    [SetUpFixture]
    public class AssemblySetup
    {
        [SetUp]
        public void Setup()
        {
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                //If this isn't a SQLite DLL we don't want/need to execute this code.
                if (!args.Name.Contains("SQLite")) { return null; }

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Support", DotSpatial.Mono.Mono.IsRunningOnMono() ? "Mono" : "Windows");
                var assemblyPath = Path.Combine(filePath, new AssemblyName(args.Name).Name + ".dll");
                return !File.Exists(assemblyPath) ? null : Assembly.LoadFrom(assemblyPath);
            };
        }
    }
}