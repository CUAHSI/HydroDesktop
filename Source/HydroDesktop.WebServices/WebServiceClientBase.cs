using System;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Security.Permissions;
using System.Web.Services.Description;
using System.Data;
using System.Collections.Generic;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// The base class for a web service client
    /// </summary>
    public class WebServiceClientBase
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of the base web service client
        /// </summary>
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
        /// Gets the names of all web services on the HydroServer
        /// </summary>
        /// <param name="assembly">The web service proxy assembly</param>
        /// <returns>The list of the service names</returns>
        protected IList<string> GetServiceNames(Assembly assembly)
        {
            return new List<string> { "wateroneflow" };
        }

        ///// <summary>
        ///// Gets an array of all public properties associated with the
        ///// specified type
        ///// </summary>
        ///// <param name="typeName">Name of the type</param>
        ///// <returns>an array of properties (propertyInfo objects)</returns>
        //protected PropertyInfo[] GetPropertyInfo(string typeName)
        //{
        //    if (_assembly == null) return null;

        //    //return _assembly.GetType(typeName).GetProperties();

        //    object obj = _assembly.CreateInstance(typeName);
        //    if (obj == null) return null;

        //    Type tp = obj.GetType();
        //    return tp.GetProperties();
        //}

        /// Creates a table with an appropriate schema for storing the results of a
        /// web method. The table has the same number of columns as the number of 
        /// public properties of the return type.
        /// <summary>
        /// <param name="properties">The array of public properties of the return type</param>
        /// <param name="typeName">Name of the return type</param>
        /// <returns>An empty data table</returns>
        /// </summary>
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
            //asmxURL = asmxURL.ToLower();
            //asmxURL = asmxURL.Trim();

            //if (asmxURL.EndsWith("?wsdl"))
            //{
            //    asmxURL = asmxURL.Replace("?wsdl", "");
            //}

            //_assembly = CreateDynamicAssembly(asmxURL);

            //if (_assembly == null)
            //{
            //    throw new System.Net.WebException
            //        ("The Uri " + asmxURL + " is not a valid SOAP web service uri");
            //}

            //IList<string> serviceNames = GetServiceNames(_assembly);
            //if (serviceNames.Count == 0)
            //{
            //    throw new System.Net.WebException
            //        ("The WSDL " + asmxURL + " does not have any valid SOAP web services");
            //}

            //string serviceName = serviceNames[0];
            //_webService = _assembly.CreateInstance(serviceName);
            //if (_webService == null)
            //{
            //    throw new System.Net.WebException
            //        ("The WSDL " + asmxURL + " does not have a web service '" + serviceName + "'");
            //}      
        }
        #endregion
    }
}
