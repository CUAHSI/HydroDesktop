using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Interface for Variables Repository
    /// </summary>
    public interface IVariablesRepository : IRepository<Variable>
    {
        /// <summary>
        /// Insert Variable
        /// </summary>
        /// <param name="variable">Variable to insert</param>
        void AddVariable(Variable variable);

        /// <summary>
        /// Update Variable
        /// </summary>
        /// <param name="variable">Variable to update</param>
        void Update(Variable variable);

        /// <summary>
        /// Check that variable already exists in the database
        /// </summary>
        /// <param name="site">Variable to check.</param>
        /// <returns>True - if Variable exists, False - otherwise.</returns>
        bool Exists(Variable site);
    }
}