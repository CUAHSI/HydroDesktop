using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Common.Tools;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

namespace HydroDesktop.DataDownload.DataAggregation
{
    /// <summary>
    /// Used for aggregating data values.
    /// </summary>
    internal class Aggregator
    {
        #region Fields

        private readonly AggregationSettings _settings;
        private readonly IFeatureLayer _layer;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="Aggregator"/>
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <param name="layer">Layer to update</param>
        /// <exception cref="ArgumentNullException">Raises if <paramref name="settings"/> or <paramref name="layer"/> is null.</exception>
        public Aggregator(AggregationSettings settings, IFeatureLayer layer)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (layer == null) throw new ArgumentNullException("layer");
            Contract.EndContractBlock();

            _settings = settings;
            _layer = layer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Progress Handler
        /// </summary>
        public IProgressHandler ProgressHandler { get; set; }

        #endregion

        #region Public methods

        private static DataColumn FindOrCreateColumn(DataTable dataTable, string columnName, Type columnType)
        {
            var dataColumn = dataTable.Columns.Cast<DataColumn>()
                .FirstOrDefault(column => column.ColumnName == columnName &&
                                          column.DataType == columnType);
            if (dataColumn == null)
            {
                dataColumn = new DataColumn(columnName, columnType);
                dataTable.Columns.Add(dataColumn);
            }
            return dataColumn;
        }

        /// <summary>
        /// Perform aggregiation using given settings
        /// </summary>
        public void Calculate()
        {
            int percentage = 0;
            ReportProgress(++percentage, "Starting calculation");

            // Add column to store data, if it not exists
            ReportProgress(++percentage, "Finding column to store data");
            var columnName = GetColumnName(_settings.AggregationMode);
            var columnType = typeof (double);
            var dataColumn = FindOrCreateColumn(_layer.DataSet.DataTable, columnName, columnType);
            var percAvailableColumn = FindOrCreateColumn(_layer.DataSet.DataTable, "PercAvailable", columnType);

            // Call aggregation
            ReportProgress(++percentage, "Finding series to process");
            var idsToProcess = new List<Tuple<IFeature, long>>();
            foreach (var feature in _layer.DataSet.Features)
            {
                var seriesIDValue = feature.DataRow["SeriesID"];
                if (seriesIDValue == null || seriesIDValue == DBNull.Value)
                    continue;
                var seriesID = Convert.ToInt64(seriesIDValue);
                idsToProcess.Add(new Tuple<IFeature, long>(feature, seriesID));
            }

            var repo = RepositoryFactory.Instance.Get<IDataValuesRepository>(DatabaseTypes.SQLite,
                                                                             Settings.Instance.DataRepositoryConnectionString);
            var aggregationFunction = GetSQLAggregationFunction(_settings.AggregationMode);
            var minDate = _settings.StartTime;
            var maxDate = _settings.EndTime;
            for (int i = 0; i < idsToProcess.Count; i++)
            {
                var tuple = idsToProcess[i];
                var feature = tuple.Item1;
                var seriesID = tuple.Item2;
                var value = repo.AggregateValues(seriesID, aggregationFunction, minDate, maxDate);
                feature.DataRow[dataColumn] = value;

                // Calculating PercAvailable
                var percAvailabe = repo.CalculatePercAvailable(seriesID, minDate, maxDate);
                feature.DataRow[percAvailableColumn] = percAvailabe;

                // reporting progress
                ReportProgress(percentage + (i + 1) * (98 - percentage) / idsToProcess.Count,
                               string.Format("Processed {0}/{1} series", i + 1, idsToProcess.Count));
            }

           

            // Save update data
            ReportProgress(99, "Saving data");
            if (!string.IsNullOrEmpty(_layer.DataSet.Filename))
            {
                _layer.DataSet.Save();
            }

            ReportProgress(100, "Finished");
        }

        #endregion

        #region Private methods

        private void ReportProgress(int percentage, object state)
        {
            var progressHandler = ProgressHandler;
            if (progressHandler == null) return;

            progressHandler.ReportProgress(percentage, state);
        }

        private static string GetColumnName(AggregationMode mode)
        {
            return mode.Description();
        }

        private static string GetSQLAggregationFunction(AggregationMode mode)
        {
            switch (mode)
            {
                case AggregationMode.Max:
                    return "Max";
                case AggregationMode.Min:
                    return "Min";
                case AggregationMode.Sum:
                    return "Sum";
                case AggregationMode.Avg:
                    return "Avg";
                default:
                    throw new ArgumentOutOfRangeException("Unknown AggregationMode");
            }
        }

        #endregion
    }
}
