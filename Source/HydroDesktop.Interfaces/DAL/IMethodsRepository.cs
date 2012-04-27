using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Methods Repository
    /// </summary>
    public interface IMethodsRepository : IRepository<Method>
    {
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="methodDescription">Method description</param>
        /// <param name="methodLink">Method link</param>
        /// <returns>MethodID of inserted method</returns>
        int InsertMethod(string methodDescription, string methodLink);

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="methodID">Method ID</param>
        /// <param name="methodDescription">Method description</param>
        /// <param name="methodLink">Method link</param>
        void UpdateMethod(int methodID, string methodDescription, string methodLink);


        /// <summary>
        /// Get MethodID by methodDescription
        /// </summary>
        /// <param name="methodDescription">Method Description</param>
        /// <returns>MethodID or null.</returns>
        int? GetMethodID(string methodDescription);
    }
}