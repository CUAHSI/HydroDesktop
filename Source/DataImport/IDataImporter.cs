namespace DataImport
{
    /// <summary>
    /// Common interface for Data Import class
    /// </summary>
    public interface IDataImporter
    {
        /// <summary>
        /// Filter in open file dialog
        /// </summary>
        string Filter { get; }

        /// <summary>
        /// Get that file can be imported by current data importer.
        /// </summary>
        /// <param name="pathToFile">Path to file with data.</param>
        /// <returns></returns>
        bool CanImportFromFile(string pathToFile);

        /// <summary>
        /// Import data from file
        /// </summary>
        /// <param name="pathToFile">Path to file with data.</param>
        void Import(string pathToFile);
    }
}