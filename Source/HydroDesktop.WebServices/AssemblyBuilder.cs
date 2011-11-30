using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml;
using log4net;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// This class can be used to build an assembly from a WSDL or a file.
    /// It checks whether the assembly is a web service assembly and lists
    /// its web service classes (Potentially, one wsdl may contain multiple
    /// web services).
    /// </summary>
    public static class AssemblyBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        #region Private Methods

        /// <summary>
        /// Compiles an assembly from the proxy class provided by the ServiceDescriptionImporter.
        /// </summary>
        /// <param name="descriptionImporter"></param>
        /// <param name="assemblyPath">The assembly file path</param>
        /// <returns>An assembly that can be used to execute the web service methods.</returns>
        private static Assembly CompileAssembly(ServiceDescriptionImporter descriptionImporter, string outputPath)
        {
            log.Debug("CompileAssembly");
            // a namespace and compile unit are needed by importer
            CodeNamespace codeNamespace = new CodeNamespace();
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            codeUnit.Namespaces.Add(codeNamespace);

            ServiceDescriptionImportWarnings importWarnings = descriptionImporter.Import(codeNamespace, codeUnit);
            
            if (importWarnings == 0) // no warnings
            {
                log.Debug("CompileAssembly No Warnings");
                // create a c# compiler
                CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");

                // include the assembly references needed to compile
                string[] references = new string[2] { "System.Web.Services.dll", "System.Xml.dll" };

                CompilerParameters parameters = new CompilerParameters(references);
              
                /* Trade off 
                 * Since libraries are built dynamically, 
                 * then the might be in use, so just use a random name
                 * Then you get many dll's in the temp folder
                 * */
               // parameters.OutputAssembly = outputPath;
                parameters.GenerateInMemory = true;

                // compile into assembly
                CompilerResults results = compiler.CompileAssemblyFromDom(parameters, codeUnit);

                foreach (CompilerError oops in results.Errors)
                {
                    log.Error("Error Compiling Assembely" + oops);
                    // trap these errors and make them available to exception object
                    throw new Exception("Compilation Error Creating Assembly");
                }

                // all done....
                return results.CompiledAssembly;
            }
            else
            {
                log.Error( "CompileAssembly Invalid WSDL");
                // warnings issued from importers, something wrong with WSDL
                throw new Exception("Invalid WSDL");
            }
        }

        /// <summary>
        /// Builds the web service description importer, which allows us to generate a proxy class based on the 
        /// content of the WSDL described by the XmlTextReader.
        /// </summary>
        /// <param name="xmlreader">The WSDL content, described by XML.</param>
        /// <returns>A ServiceDescriptionImporter that can be used to create a proxy class.</returns>
        private static ServiceDescriptionImporter BuildServiceDescriptionImporter(XmlTextReader xmlreader)
        {
            // make sure xml describes a valid wsdl
            if (!ServiceDescription.CanRead(xmlreader)){
                log.Error("Invalid Web Service Description");
                throw new Exception("Invalid Web Service Description");
}
            // parse wsdl
            ServiceDescription serviceDescription = ServiceDescription.Read(xmlreader);

            // build an importer, that assumes the SOAP protocol, client binding, and generates properties
            ServiceDescriptionImporter descriptionImporter = new ServiceDescriptionImporter();
            descriptionImporter.ProtocolName = "Soap";
            descriptionImporter.AddServiceDescription(serviceDescription, null, null);
            descriptionImporter.Style = ServiceDescriptionImportStyle.Client;
            descriptionImporter.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            return descriptionImporter;
        }

        public static string GetAssemblyFilePath(Uri wsdlUri)
        {
            if (String.IsNullOrEmpty(wsdlUri.ToString()))
            {
                log.Error("Web Service Not Found");
                throw new Exception("Web Service Not Found");
            }
                
            
            string hdPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "HydroDesktop");
            if (!System.IO.Directory.Exists(hdPath))
            {
                log.Debug("Creating hdPath" + hdPath);
                System.IO.Directory.CreateDirectory(hdPath);
}
            string assemblyPath = System.IO.Path.Combine(hdPath, "assemblies");
            if (!System.IO.Directory.Exists(assemblyPath))
            {
                log.Debug("Creating assemblyPath" + assemblyPath);
                System.IO.Directory.CreateDirectory(assemblyPath);
}
            string uriString = wsdlUri.ToString();
            

            if (uriString.StartsWith(@"http://"))
                uriString = uriString.Replace(@"http://", "");
            uriString = uriString.Replace("/", "_") + ".dll";
            log.Debug("assemblyPath " + uriString);
            return System.IO.Path.Combine(assemblyPath, uriString);
        }

        /// <summary>
        /// Builds an assembly from a web service description.
        /// The assembly can be used to execute the web service methods.
        /// </summary>
        /// <param name="webServiceUri">Location of WSDL.</param>
        /// <returns>A web service assembly.</returns>
        public static Assembly BuildAssemblyFromWSDL(Uri wsdlUri)
        {
            log.Debug("BuildAssemblyFromWSDL");

            if (String.IsNullOrEmpty(wsdlUri.ToString())){
                log.Error("Web Service Not Found"); 
                throw new Exception("Web Service Not Found");
}
            XmlTextReader xmlreader = new XmlTextReader(wsdlUri.ToString());

            ServiceDescriptionImporter descriptionImporter = BuildServiceDescriptionImporter(xmlreader);

            string assemblyPath = GetAssemblyFilePath(wsdlUri);

            xmlreader.Close();

            log.Debug("BuildAssemblyFromWSDL Completed");
            return CompileAssembly(descriptionImporter, assemblyPath);
        }

        /// <summary>
        /// Builds an assembly from an existing .NET assembly dll file.
        /// </summary>
        /// <param name="assemblyFileName">The full path of the dll</param>
        /// <returns>The assembly</returns>
        public static Assembly BuildAssemblyFromFile(string assemblyFileName)
        {
            if (!System.IO.File.Exists(assemblyFileName))
            {
                log.Debug("BuildAssemblyFromFile file not found (this is ok)" + assemblyFileName);
                return null;
            }

            log.Debug("BuildAssemblyFromFile file  found " + assemblyFileName);
            return Assembly.LoadFile(assemblyFileName);
        }

        #endregion
    }
}
