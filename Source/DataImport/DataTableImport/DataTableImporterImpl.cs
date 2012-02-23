using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using HydroDesktop.Common;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.DataTableImport
{
    class DataTableImporterImpl : IImporter
    {
        public void Import(IImporterSettings settings)
        {
            int progress = 0;
            ReportProgress(progress, "Starting importing...");

            var seriesRepo = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
            var repoManager = RepositoryFactory.Instance.Get<IRepositoryManager>();

            var toImport = new List<Tuple<ColumnInfo, Series, OverwriteOptions>> ();
            foreach (var cData in settings.ColumnDatas.Where(c => c.ImportColumn && c.ColumnName != settings.DateTimeColumn))
            {
                var site = cData.Site;
                var variable = cData.Variable;
                var method = cData.Method;
                var source = cData.Source;
                var qualityControl = cData.QualityControlLevel;

                //
                OverwriteOptions options;
                if (seriesRepo.ExistsSeries(site, variable))
                {
                    using (var dialog = new ExistsSeriesQuestionDialog(site.Name, variable.Name))
                    {
                        dialog.ShowDialog();
                        options = dialog.CurrentOption;
                    }
                }
                else
                {
                    options = OverwriteOptions.Overwrite;
                }

                var series = new Series(site, variable, method, qualityControl, source);
                toImport.Add(new Tuple<ColumnInfo, Series, OverwriteOptions>(cData, series, options));
            }

            progress = 10;
            ReportProgress(progress, "Parsing values...");
            foreach (DataRow row in settings.Data.Rows)
            {
                DateTime dateTime;
                if (!DateTime.TryParse(row[settings.DateTimeColumn].ToString(), out dateTime))
                    continue;

                foreach (var tuple in toImport)
                {
                    var columnIndex = tuple.Item1.ColumnIndex;

                    Double value;
                    if (!Double.TryParse(row[columnIndex].ToString(), out value))
                        continue;

                    var series = tuple.Item2;
                    series.DataValueList.Add(new DataValue(value, dateTime, 0));
                }
            }

            progress = 15;
            ReportProgress(progress, "Saving values into local database...");

            var pStep = (int)((99.0 - progress)/toImport.Count);
            var theme = new Theme(Path.GetFileNameWithoutExtension(settings.PathToFile));
            foreach (var tuple in toImport)
            {
                repoManager.SaveSeries(tuple.Item2, theme, tuple.Item3);

                progress += pStep;
                ReportProgress(progress, "Saving values into local database...");
            }

            progress = 100;
            ReportProgress(progress, "Finished");
        }

        public IProgressHandler ProgressHandler { get; set; }

        private void ReportProgress(int percentage, object state)
        {
            var ph = ProgressHandler;
            if (ph != null)
            {
                ph.ReportProgress(percentage, state);
            }
        }
    }
}