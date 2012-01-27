using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Methods Repository
    /// </summary>
    public interface IMethodsRepository
    {
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="methodDescription">Method description</param>
        /// <param name="methodLink">Method link</param>
        /// <param name="methodID">Method ID, may be null</param>
        /// <returns>MethodID of inserted method</returns>
        int InsertMethod(string methodDescription, string methodLink, int? methodID = null);

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="methodID">Method ID</param>
        /// <param name="methodDescription">Method description</param>
        /// <param name="methodLink">Method link</param>
        void UpdateMethod(int methodID, string methodDescription, string methodLink);

        /// <summary>
        /// Get Data Table with all methods
        /// </summary>
        /// <returns>Data Table with all methods</returns>
        DataTable GetAllMethods();


        /// <summary>
        /// Get MethodID by methodDescription
        /// </summary>
        /// <param name="methodDescription">Method Description</param>
        /// <returns>MethodID or null.</returns>
        int? GetMethodID(string methodDescription);

        /// <summary>
        /// Get method by ID
        /// </summary>
        /// <param name="methodID">Method ID</param>
        /// <returns>Method or null.</returns>
        Method GetMethod(int methodID);
    }
}