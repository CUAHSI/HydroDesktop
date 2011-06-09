using System;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security.Permissions;
using System.Web.Services.Description;
using System.Data;
using System.Collections.Generic;
using log4net;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// The base class for a web service client
    /// </summary>
    public class WebServiceClientBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        #region Protected Variables
        
        /// <summary>
        /// the dynamically-created assembly containing web
        /// methods and proxy objects
        /// </summary>
        protected Assembly _assembly = null;

        /// <summary>
        /// proxy class containing methods exposed
        /// by the web service
        /// </summary>
        protected object _webService = null;

        #endregion

        #region Constructor

        public WebServiceClientBase() { }

        /// <summary>
        /// Creates a new instance of the web service client
        /// </summary>
        /// <param name="asmxURL">The URL of the web service</param>
        public WebServiceClientBase(string asmxURL)
        {
            CheckWebService(asmxURL);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Dynamically instantiates an assembly that contains the web services and 
        /// associated types
        /// </summary>
        /// <param name="_assemblyxUrl"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        protected Assembly CreateDynamicAssembly(string asmxUrl)
        {
            try
            {
                asmxUrl = asmxUrl.ToLower().Trim();
                if (asmxUrl.Contains("?wsdl"))
                {
                    asmxUrl = asmxUrl.Replace("?wsdl", "");
                }
                
                Uri uri = new Uri(asmxUrl);
                
                string assemblyPath = AssemblyBuilder.GetAssemblyFilePath(uri);
                
                //building assembly from file was causing some errors -
                //always re-build the assembly from the WSDL for now.
                return AssemblyBuilder.BuildAssemblyFromWSDL(uri);
                
                //if (System.IO.File.Exists(assemblyPath))
                //{
                //    return AssemblyBuilder.BuildAssemblyFromFile(assemblyPath);
                //}
                //else
                //{
                //    return AssemblyBuilder.BuildAssemblyFromWSDL(uri);
                //}     
            }
            catch (Exception ex)
            {
               log.Info("Build from assembely returned error ",ex);
                return null;
            }
        }

        protected IList<string> GetServiceNames(Assembly assembly)
        {
            List<string> serviceNames = new List<string>();
            
            // see what service types are available
            Type[] types = assembly.GetExportedTypes();

            // and save them
            foreach (Type type in types)
            {
                if (type.BaseType.Name == "SoapHttpClientProtocol")
                {
                    serviceNames.Add(type.FullName);
                }
            }
            return serviceNames;
        }

        /// <summary>
        /// Gets an array of all public properties associated with the
        /// specified type
        /// </summary>
        /// <param name="typeName">Name of the type</param>
        /// <returns>an array of properties (propertyInfo objects)</returns>
        protected PropertyInfo[] GetPropertyInfo(string typeName)
        {
            if (_assembly == null) return null;

            //return _assembly.GetType(typeName).GetProperties();

            object obj = _assembly.CreateInstance(typeName);
            if (obj == null) return null;

            Type tp = obj.GetType();
            return tp.GetProperties();
        }

        /// Creates a table with an appropriate schema for storing the results of a
        /// web method. The table has the same number of columns as the number of 
        /// public properties of the return type.
        /// </summary>
        /// <param name="properties">The array of public properties of the return type</param>
        /// <param name="typeName">Name of the return type</param>
        /// <returns>An empty data table</returns>
        protected DataTable CreateResultTable(PropertyInfo[] properties, string typeName)
        {
            DataTable table = new DataTable(typeName);
            foreach (PropertyInfo pi in properties)
            {
                DataColumn col = new DataColumn(pi.Name, pi.PropertyType);
                table.Columns.Add(col);
            }
            return table;
        }

        /// <summary>
        /// writes the web service call results to a data table
        /// (the table must have the same column names as the property names)
        /// </summary>
        /// <param name="resultObj">the object returned by the web service</param>
        /// <param name="properties">the array of public properties of the result object</param>
        /// <param name="table">the data table where results are written</param>
        protected void WriteResults(Object resultObj, PropertyInfo[] properties, DataTable table)
        {
            Array arr = resultObj as Array;
            if (arr != null)
            {
                foreach (object obj in arr)
                {
                    DataRow row = table.NewRow();

                    foreach (PropertyInfo pi in properties)
                    {
                        object val = pi.GetValue(obj, null);
                        if (val != null)
                        {
                            try
                            {
                                row[pi.Name] = val;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Row could not be added: " + ex.Message);
                            }
                        }
                    }
                    table.Rows.Add(row);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will check the web service.
        /// It will throw an exception when the web service is
        /// an invalid web service.
        /// </summary>
        /// <param name="asmxURL"></param>
        public void CheckWebService(string asmxURL)
        {
            asmxURL = asmxURL.ToLower();
            asmxURL = asmxURL.Trim();

            if (asmxURL.EndsWith("?wsdl"))
            {
                asmxURL = asmxURL.Replace("?wsdl", "");
            }

            _assembly = CreateDynamicAssembly(asmxURL);

            if (_assembly == null)
            {
                throw new System.Net.WebException
                    ("The Uri " + asmxURL + " is not a valid SOAP web service uri");
            }

            IList<string> serviceNames = GetServiceNames(_assembly);
            if (serviceNames.Count == 0)
            {
                throw new System.Net.WebException
                    ("The WSDL " + asmxURL + " does not have any valid SOAP web services");
            }

            string serviceName = serviceNames[0];
            _webService = _assembly.CreateInstance(serviceName);
            if (_webService == null)
            {
                throw new System.Net.WebException
                    ("The WSDL " + asmxURL + " does not have a web service '" + serviceName + "'");
            }      
        }
        
        /// <summary>
        /// Calls a web method from the current web service
        /// </summary>
        /// <param name="methodName">name of the web method (case-sensitive)</param>
        /// <param name="parameters">an array of the method's input parameters</param>
        /// <returns>the web method result object</returns>
        public object CallWebMethod(string methodName, object[] parameters)
        {
            MethodInfo mi = _webService.GetType().GetMethod(methodName);
            return mi.Invoke(_webService, parameters);
        }

        #endregion
    }
}
