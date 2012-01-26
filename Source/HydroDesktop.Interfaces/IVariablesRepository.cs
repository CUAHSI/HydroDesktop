using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Variables Repository
    /// </summary>
    public interface IVariablesRepository
    {
        /// <summary>
        /// Get all
        /// </summary>
        /// <returns>Data Table with all data</returns>
        DataTable GetAll();

        /// <summary>
        /// Get <see cref="Variable"/> by ID
        /// </summary>
        /// <param name="id">VariableID</param>
        /// <returns>Instance of <see cref="Variable"/> or null, if entity not found.</returns>
        Variable GetByID(long id);

        /// <summary>
        /// Create copy of variable and save it to database
        /// </summary>
        /// <param name="source">Source Variable</param>
        /// <returns>Created Variable</returns>
        Variable CreateCopy(Variable source);

        /// <summary>
        /// Insert Variable
        /// </summary>
        /// <param name="variable">Variable to insert</param>
        void Insert(Variable variable);

        /// <summary>
        /// Update Variable
        /// </summary>
        /// <param name="variable">Variable to update</param>
        void Update(Variable variable);
    }
}