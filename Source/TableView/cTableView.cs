using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Interfaces;
using HydroDesktop.Database;
using HydroDesktop.Configuration;

namespace TableView
{
    public partial class cTableView : UserControl
    {
        #region Fields

        private readonly ISeriesSelector _seriesSelector;
        private bool _needToRefresh;

        #endregion

        #region Constructor

        public cTableView(ISeriesSelector seriesSelector)
        {
            if (seriesSelector == null) throw new ArgumentNullException("seriesSelector");
            Contract.EndContractBlock();

            InitializeComponent();

            _seriesSelector = seriesSelector;

            dataGridViewNavigator1.PageChanged += dataGridViewNavigator1_PageChanged;

            _seriesSelector.SeriesCheck += seriesSelector_Refreshed;
            _seriesSelector.Refreshed += seriesSelector_Refreshed;
            Disposed += OnDisposed;
            VisibleChanged += OnVisibleChanged;
        }

        private void OnDisposed(object sender, EventArgs eventArgs)
        {
            _seriesSelector.SeriesCheck -= seriesSelector_Refreshed;
            _seriesSelector.Refreshed -= seriesSelector_Refreshed;
            Disposed -= OnDisposed;
        }

        #endregion

        #region Properties

        private TableViewMode _viewMode;
        public TableViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                UpdateViewMode();
            }
        }

        /// <summary>
        /// Path to current database
        /// </summary>
        public string DatabasePath { get; private set; }

        #endregion

        #region Private methods
        
        private void UpdateViewMode()
        {
            if (String.IsNullOrEmpty(Settings.Instance.DataRepositoryConnectionString)) return;

            try
            {
                switch (ViewMode)
                {
                    case TableViewMode.SequenceView:
                        ShowAllFieldsinSequence();
                        break;
                    case TableViewMode.JustValuesInParallel:
                        ShowJustValuesinParallel();
                        break;
                }
            }
            catch (InvalidOperationException) // this throws by DataGridViewNavigator if it work not finished
            {
                _needToRefresh = true;
            }
        }

        private void UpdateDatabasePath()
        {
            if (Settings.Instance.DataRepositoryConnectionString != null)
            {
                DatabasePath = SQLiteHelper.GetSQLiteFileName(Settings.Instance.DataRepositoryConnectionString);
            }
        }

        private void dataGridViewNavigator1_PageChanged(object sender, PageChangedEventArgs e)
        {
            if (!Visible)
            {
                _needToRefresh = true;
                return;
            }

            if (_needToRefresh)
            {
                _needToRefresh = false;
                UpdateViewMode();
                return;
            }

            dataViewSeries.DataSource = e.DataTable;

            if (ViewMode == TableViewMode.JustValuesInParallel)
            {
                // Update columns headers
                var dataSeriesRepo = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
                var columnDateTime = dataViewSeries.Columns["DateTime"];
                Debug.Assert(columnDateTime != null);
                columnDateTime.HeaderText = "DateTime" + Environment.NewLine + "Unit";
                foreach (var id in _seriesSelector.CheckedIDList)
                {
                    var seriesNameTable = dataSeriesRepo.GetUnitSiteVarForFirstSeries(id);
                    var row1 = seriesNameTable.Rows[0];
                    var unitsName = Convert.ToString(row1[0]);
                    var siteName = Convert.ToString(row1[1]);
                    var variableName = Convert.ToString(row1[2]);

                    var columnD_id = dataViewSeries.Columns["D" + id];
                    Debug.Assert(columnD_id != null);
                    columnD_id.HeaderText = siteName + " * " + id + Environment.NewLine +
                                            variableName + Environment.NewLine +
                                            unitsName;
                }
            }
        }

        private void ShowAllFieldsinSequence()
        {
            dataGridViewNavigator1.Initialize(new FieldsInSequenceGetter(_seriesSelector.CheckedIDList));
        }

        private void ShowJustValuesinParallel()
        {
            dataGridViewNavigator1.Initialize(new ValuesInParallelGetter(_seriesSelector.CheckedIDList));
        }
        
        private void OnVisibleChanged(object sender, EventArgs eventArgs)
        {
            if (!Visible) return;
            if (_needToRefresh)
            {
                _needToRefresh = false;
                RefreshTableView();
            }
        }

        private void RefreshTableView()
        {
            if (!Visible)
            {
                _needToRefresh = true;
                return;
            }

            UpdateViewMode();
            UpdateDatabasePath();
        }

        private void seriesSelector_Refreshed(object sender, EventArgs e)
        {
            RefreshTableView();
        }

        private void cTableView_Load(object sender, EventArgs e)
        {
            dataViewSeries.ColumnHeadersVisible = true;
            dataViewSeries.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;
            
            UpdateDatabasePath();
        }

        /// <summary>
        /// Remove the column header border in the Aero theme in Vista,
        /// but keep it for other themes such as standard and classic.
        /// </summary>
        private static DataGridViewHeaderBorderStyle ProperColumnHeadersBorderStyle
        {
            get
            {
                return (SystemFonts.MessageBoxFont.Name == "Segoe UI")
                           ? DataGridViewHeaderBorderStyle.None
                           : DataGridViewHeaderBorderStyle.Raised;
            }
        }

        #endregion

        private class ValuesInParallelGetter : IPagedTableGetter
        {
            private readonly IDataValuesRepository _dataValuesRepository;
            private readonly IList<int> _selectedIds;

            public ValuesInParallelGetter(IEnumerable<int> selectedIds)
            {
                _dataValuesRepository = RepositoryFactory.Instance.Get<IDataValuesRepository>();
                _selectedIds = selectedIds.Select(s => s).ToList();
            }

            public DataTable GetTable(int valuesPerPage, int currentPage)
            {
                return _dataValuesRepository.GetTableForJustValuesInParallel(_selectedIds, valuesPerPage, currentPage);
            }

            public long GetTotalCount()
            {
                return _dataValuesRepository.GetCountForJustValuesInParallel(_selectedIds);
            }
        }

        private class FieldsInSequenceGetter : IPagedTableGetter
        {
            private readonly IDataValuesRepository _dataValuesRepository;
            private readonly IList<int> _selectedIds;

            public FieldsInSequenceGetter(IEnumerable<int> selectedIds)
            {
                _dataValuesRepository = RepositoryFactory.Instance.Get<IDataValuesRepository>();
                _selectedIds = selectedIds.Select(s => s).ToList();
            }

            public DataTable GetTable(int valuesPerPage, int currentPage)
            {
                return _dataValuesRepository.GetTableForAllFieldsInSequence(_selectedIds, valuesPerPage, currentPage);
            }

            public long GetTotalCount()
            {
                return _dataValuesRepository.GetCountForAllFieldsInSequence(_selectedIds);
            }
        }
    }

    public enum TableViewMode
    {
        [Description("Show All Fields in Sequence")]
        SequenceView,
        [Description("Show Just Values in Parallel")]
        JustValuesInParallel
    }
}
