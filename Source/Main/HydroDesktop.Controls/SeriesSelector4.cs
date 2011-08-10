using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using HydroDesktop.Data.Plugins;
using HydroDesktop.ObjectModel;

namespace HydroDesktopControls
{
    /// <summary>
    /// The SeriesSelector3 control enables the filtering and selection
    /// of data series in the table view, graph view and other plug-ins.
    /// </summary>
    public partial class SeriesSelector4 : UserControl
    {
        #region Variable

        private IHydroDatabase _hydroDatabase;
       
        //Private Six Criterion Tables
        private DataTable _themeTable;
        private DataTable _siteTable;
        private DataTable _variableTable;
        private DataTable _methodTable;
        private DataTable _sourceTable;
        private DataTable _qcLevelTable;

        //Private SQL Expression Builder
        private StringBuilder strSql = new StringBuilder();        

        //when the series checkbox is checked or unchecked
        public event SeriesEventHandler SeriesChecked = null;
        public event SeriesEventHandler SeriesUnchecked = null;

        //when the series is selected (not checked but highlighted)
        public event SeriesEventHandler SeriesSelected = null;

        //when the filter criterion is changed
        public event EventHandler FilterChanged = null;

        
        // the currently used filter expression
        private string _queryFilter = "";

        //private int _selectedSeriesId = 0;

        private bool _ignoreCheckEvent = false;

        private bool _settingFilter = false;

        private List<int> _checkedIDList = new List<int>();
        
        #endregion

        #region Property
        
        /// <summary>
        /// The table with detailed properties of all series
        /// </summary>
        public DataTable Table
        {
            get { return _hydroDatabase.SelectionManager.SelectionTable; }
        }

        /// <summary>
        /// The SQL Filter expression currently used to filter the 
        /// displayed series checkboxes in this control
        /// </summary>
        public string FilterExpression
        {
            get { return _hydroDatabase.SelectionManager.FilterExpression; }
        }

        /// <summary>
        /// Gets the seriesID of the currently highlighted (selected) item in the 
        /// checked list box. Notice: selected doesn't correspond to checked.
        /// </summary>
        public int SelectedSeriesID
        {
            get
            {
                int index = checkedSeriesList.SelectedIndex;

                if (index < 0) return 0;

                object item = checkedSeriesList.Items[index];
                int seriesID = GetSeriesID(item);
                return seriesID;
                
            }
        }

        #endregion

        #region Constructor
        public SeriesSelector4()
        {
            InitializeComponent();
            
            if (DesignMode) return;

            try
            {
                this.radAll.Click += new System.EventHandler(radAll_Click);
                this.radComplex.Click += new System.EventHandler(this.radComplex_Click);
                this.radSimple.Click += new System.EventHandler(this.radSimple_Click);

                //load the table of all available series
                try
                {
                    DataTable table = _hydroDatabase.SelectionManager.SelectionTable;
                }
                catch
                {
                    _hydroDatabase = new HydroDatabase(Config.DataRepositoryConnectionString);

                }

                Initialize();

                checkedSeriesList.MouseDown += new MouseEventHandler(checkedSeriesList_MouseDown);
                _hydroDatabase.SelectionManager.SeriesChecked += new SeriesEventHandler(SelectionManager_SeriesChecked);
                _hydroDatabase.SelectionManager.SeriesUnchecked += new SeriesEventHandler(SelectionManager_SeriesUnchecked);
                _hydroDatabase.SelectionManager.FilterCriterionChanged += new EventHandler(SelectionManager_FilterChanged);
            }
            catch { }
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// When a series filter is changed in a different control
        /// --> We need to ensure that the correct checkbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectionManager_FilterChanged(object sender, EventArgs e)
        {
            _settingFilter = true;
            
            string filterEx = _hydroDatabase.SelectionManager.FilterExpression;
            if (filterEx == "")
            {
                radSimple.Checked = false;
                radComplex.Checked = false;
                radAll.Checked = true;

                //Set checkedSeriesList Location and size
                Point listLocation = new Point();
                listLocation.X = 6;
                listLocation.Y = cbBoxCriterion.Top;
                checkedSeriesList.Location = listLocation; //new Point(6, 34);
                checkedSeriesList.Height = groupBox1.Height - 25;
                checkedSeriesList.HorizontalScrollbar = true;

                //Re-add the check boxes
                AddSeriesCheckBoxes();
                OnFilterChanged();
            }
            else
            {
                //simple or complex filter
                if (filterEx.Contains("ThemeID=") || filterEx.Contains("SiteID=") || filterEx.Contains("VariableID=") ||
                    filterEx.Contains("MethodID=") || filterEx.Contains("SourceID=") || filterEx.Contains("QualityControlLevelID="))
                {
                    //simple filter
                    radAll.Checked = false;
                    radComplex.Checked = false;
                    radSimple.Checked = true;

                    //Fill the cbBoxCriterion with 6 items
                    cbBoxCriterion.Items.Clear();
                    cbBoxCriterion.Items.Add("Please select a filter criterion");
                    cbBoxCriterion.Items.Add("Themes");
                    cbBoxCriterion.Items.Add("Site");
                    cbBoxCriterion.Items.Add("Variable");
                    cbBoxCriterion.Items.Add("Method");
                    cbBoxCriterion.Items.Add("Source");
                    cbBoxCriterion.Items.Add("QCLevel");

                    //Set checkedSeriesList Location and size
                    checkedSeriesList.Location = new System.Drawing.Point(6, 90); ;
                    checkedSeriesList.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;

                    //Re-select the filter criterion
                    string criterion = filterEx.Substring(0, filterEx.IndexOf("="));

                    switch (criterion)
                    {
                        case "ThemeID":
                            cbBoxCriterion.SelectedIndex = 1;
                            break;
                        case "SiteID":
                            cbBoxCriterion.SelectedIndex = 2;
                            break;
                        case "VariableID":
                            cbBoxCriterion.SelectedIndex = 3;
                            break;
                        case "MethodID":
                            cbBoxCriterion.SelectedIndex = 4;
                            break;
                        case "SourceID":
                            cbBoxCriterion.SelectedIndex = 5;
                            break;
                        case "QualityControlLevelID":
                            cbBoxCriterion.SelectedIndex = 6;
                            break;
                    }
                    
                    //re-select the filter value
                    string filterValue = filterEx.Substring(filterEx.IndexOf("=") + 1);
                    
                    //remove the leading quotes from the filter expression
                    if (filterValue.EndsWith("'"))
                    {
                        filterValue = filterValue.Substring(0, filterValue.Length - 1);
                    }
                    if (filterValue.StartsWith("'"))
                    {
                        filterValue = filterValue.Substring(1);
                    }

                    for(int i=0; i< cbBoxContent.Items.Count; i++)
                    {
                        DataRowView drv = cbBoxContent.Items[i] as DataRowView;
                        if (drv != null)
                        {
                            string id = drv.Row[0].ToString();
                            if (id == filterValue)
                            {
                                cbBoxContent.SelectedIndex = i;
                                break;
                            }
                        }
                    }


                    AddSeriesCheckBoxes();
                    OnFilterChanged();
                }
                else
                {
                    //otherwise, it's the complex filter used
                }
            }
            _settingFilter = false;
        }


        /// <summary>
        /// When a series is checked in a different control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event arguments (these contain the seriesID)</param>
        void SelectionManager_SeriesUnchecked(object sender, SeriesEventArgs e)
        {
            if (_ignoreCheckEvent) return;

            for(int i=0; i< checkedSeriesList.Items.Count; i++)
            {          
                if (checkedSeriesList.GetItemChecked(i) == true)
                {
                    object item = checkedSeriesList.Items[i];
                    int seriesID = GetSeriesID(item);
                    if (seriesID == e.CheckedSeriesID)
                    {
                        checkedSeriesList.SetItemChecked(i, false);
                        //OnSeriesUnchecked(seriesID);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// When a series is checked in the current control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event arguments which contain the series ID</param>
        void SelectionManager_SeriesChecked(object sender, SeriesEventArgs e)
        {
            if (_ignoreCheckEvent) return;

            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                if (checkedSeriesList.GetItemChecked(i) == false)
                {
                    object item = checkedSeriesList.Items[i];
                    int seriesID = GetSeriesID(item);
                    if (seriesID == e.CheckedSeriesID)
                    {
                        checkedSeriesList.SetItemChecked(i, true);
                        //OnSeriesChecked(seriesID); --> don't dispatch this event: it occurs in the CheckedSeriesList_ItemCheck only
                        break;
                    }
                }
            }

        }


        // Right - click on the series selector control
        void checkedSeriesList_MouseDown(object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right)
            {
                System.Drawing.Point pt = new System.Drawing.Point();
                pt.X = e.X;
                pt.Y = e.Y;
                checkedSeriesList.SelectedIndex = checkedSeriesList.IndexFromPoint(pt);

            }
        }

        // Select All radio button is clicked
        void radAll_Click(object sender, EventArgs e)
        {
            try
            {

                if (radAll.Checked == true)
                {
                   
                    Point listLocation = new Point();
                    listLocation.X = 6;
                    listLocation.Y = cbBoxCriterion.Top;
                    checkedSeriesList.Location = listLocation; //new Point(6, 34);
                    checkedSeriesList.Height = groupBox1.Height - 25;
                    checkedSeriesList.HorizontalScrollbar = true;

                    //Set Other Filter unchecked
                    radSimple.Checked = false;
                    radComplex.Checked = false;

                    //all series
                    _queryFilter = "";

                    AddSeriesCheckBoxes();

                    //notify the selectionManager
                    _hydroDatabase.SelectionManager.SetFilter(FilterTypes.All, "");
                }
            }
            catch { }
        }

        #endregion

        #region Method

        /// <summary>
        /// Gets the list of SeriesIDs that are currently checked in the Series Selector control
        /// </summary>
        /// <returns>The list of checked Series IDs</returns>
        public IList<int> GetCheckedIDList()
        {
            return _hydroDatabase.SelectionManager.CheckedSeriesIDs;
        }

        /// <summary>
        /// Gets the list of SeriesIDs that are currently visible in the SeriesSelector control
        /// </summary>
        /// <returns>The list of visible Series IDs</returns>
        public IList<int> GetVisibleIDList()
        {
            return _hydroDatabase.SelectionManager.VisibleSeriesIDs;
        }

        /// <summary>
        /// Gets the list of all SeriesIDs in the data source of the SeriesSelector control
        /// </summary>
        /// <returns>The full list of Series IDs (includes the unchecked and currently invisible series)</returns>
        public IList<int> GetAllSeriesIDs()
        {
            return _hydroDatabase.SelectionManager.AllSeriesIDs;
        }

        private void JudgeFilterOption()
        {
            string filterEx = _hydroDatabase.SelectionManager.FilterExpression;
            if (filterEx == "")
            {
                radSimple.Checked = false;
                radComplex.Checked = false;
                radAll.Checked = true;

                //Set checkedSeriesList Location and size
                Point listLocation = new Point();
                listLocation.X = 6;
                listLocation.Y = cbBoxCriterion.Top;
                checkedSeriesList.Location = listLocation; //new Point(6, 34);
                checkedSeriesList.Height = groupBox1.Height - 25;
                checkedSeriesList.HorizontalScrollbar = true;

                //Re-add the check boxes
                AddSeriesCheckBoxes();
            }
            else
            {
                //simple or complex filter
                if (filterEx.Contains("ThemeID=") || filterEx.Contains("SiteID=") || filterEx.Contains("VariableID=") ||
                    filterEx.Contains("MethodID=") || filterEx.Contains("SourceID=") || filterEx.Contains("QualityControlLevelID="))
                {
                    //simple filter
                    radAll.Checked = false;
                    radComplex.Checked = false;
                    radSimple.Checked = true;

                    //Fill the cbBoxCriterion with 6 items
                    cbBoxCriterion.Items.Clear();
                    cbBoxCriterion.Items.Add("Please select a filter criterion");
                    cbBoxCriterion.Items.Add("Themes");
                    cbBoxCriterion.Items.Add("Site");
                    cbBoxCriterion.Items.Add("Variable");
                    cbBoxCriterion.Items.Add("Method");
                    cbBoxCriterion.Items.Add("Source");
                    cbBoxCriterion.Items.Add("QCLevel");

                    //Set checkedSeriesList Location and size
                    checkedSeriesList.Location = new System.Drawing.Point(6, 90); ;
                    checkedSeriesList.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;

                    //Re-select the filter criterion
                    string criterion = filterEx.Substring(0, filterEx.IndexOf("="));

                    switch (criterion)
                    {
                        case "ThemeID":
                            cbBoxCriterion.SelectedIndex = 1;
                            break;
                        case "SiteID":
                            cbBoxCriterion.SelectedIndex = 2;
                            break;
                        case "VariableID":
                            cbBoxCriterion.SelectedIndex = 3;
                            break;
                        case "MethodID":
                            cbBoxCriterion.SelectedIndex = 4;
                            break;
                        case "SourceID":
                            cbBoxCriterion.SelectedIndex = 5;
                            break;
                        case "QualityControlLevelID":
                            cbBoxCriterion.SelectedIndex = 6;
                            break;
                    }

                    //re-select the filter value
                    string filterValue = filterEx.Substring(filterEx.IndexOf("=") + 1);

                    //remove the leading quotes from the filter expression
                    if (filterValue.EndsWith("'"))
                    {
                        filterValue = filterValue.Substring(0, filterValue.Length - 1);
                    }
                    if (filterValue.StartsWith("'"))
                    {
                        filterValue = filterValue.Substring(1);
                    }

                    for (int i = 0; i < cbBoxContent.Items.Count; i++)
                    {
                        DataRowView drv = cbBoxContent.Items[i] as DataRowView;
                        if (drv != null)
                        {
                            string id = drv.Row[0].ToString();
                            if (id == filterValue)
                            {
                                cbBoxContent.SelectedIndex = i;
                                break;
                            }
                        }
                    }


                    AddSeriesCheckBoxes();
                    //TODO: in AddSeriesCheckBoxes, also resume the checked or unchecked state
                }
                else
                {
                    //otherwise, it's the complex filter used
                }
            }
        }           

        private void OnSeriesChecked(int seriesID)
        {
            
            if (SeriesChecked != null)
            {
                SeriesChecked(this, new SeriesEventArgs(seriesID));
            }
        }

        private void OnSeriesUnchecked(int seriesID)
        {           
            if (SeriesUnchecked != null)
            {
                SeriesUnchecked(this, new SeriesEventArgs(seriesID));
            }
        }

        private void OnFilterChanged()
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, null);
            }
        }

        private void OnSelectedIndexChanged(int seriesID)
        {
            if (this.SeriesSelected != null)
            {
                SeriesSelected(this, new SeriesEventArgs(seriesID));
            }
        }


        /// <summary>
        /// Gets or sets the database management control associated with the SeriesSelector control
        /// </summary>
        public IHydroDatabase Database
        {
            get { return _hydroDatabase; }
            set 
            {
                string oldConnectionString = _hydroDatabase.ConnectionString;
                
                _hydroDatabase = value;
                if (value != null)
                {
                    //if (_hydroDatabase.ConnectionString != oldConnectionString)
                    //{
                    //    _hydroDatabase.SelectionManager.RefreshAllSeries();
                    //}
                    
                    //remove pre-existing event handlers
                    _hydroDatabase.SelectionManager.SeriesChecked -= SelectionManager_SeriesChecked;
                    _hydroDatabase.SelectionManager.SeriesUnchecked -= SelectionManager_SeriesUnchecked;
                    _hydroDatabase.SelectionManager.FilterCriterionChanged -= SelectionManager_FilterChanged;

                    //attach the new event handlers
                    _hydroDatabase.SelectionManager.SeriesChecked += new SeriesEventHandler(SelectionManager_SeriesChecked);
                    _hydroDatabase.SelectionManager.SeriesUnchecked += new SeriesEventHandler(SelectionManager_SeriesUnchecked);
                    _hydroDatabase.SelectionManager.FilterCriterionChanged += new EventHandler(SelectionManager_FilterChanged);  
                }
            }
        }

        public void RefreshAllSeries()
        {
            _hydroDatabase.SelectionManager.RefreshAllSeries();
            SelectionManager_FilterChanged(null, null);
            //_hydroDatabase.SelectionManager.SetFilter(FilterExpression);
            //Initialize();
            //AddSeriesCheckBoxes();
        }

        private void ShowAll()
        {       
            Point listLocation = new Point();
            listLocation.X = 6;
            listLocation.Y = cbBoxCriterion.Top;
            checkedSeriesList.Location = listLocation; //new Point(6, 34);
            checkedSeriesList.Height = groupBox1.Height - 25;
            checkedSeriesList.HorizontalScrollbar = true;
            
            //Set Other Filter unchecked
            radSimple.Checked = false;
            radComplex.Checked = false;

            //all series
            _queryFilter = "";

            AddSeriesCheckBoxes();

        }

        /// <summary>
        /// Initializes the collections and lists of controls
        /// </summary>
        private void Initialize()
        {
            string sqlTheme = "SELECT ThemeID, ThemeName FROM DataThemeDescriptions ORDER BY ThemeName";
            string sqlSite = "SELECT SiteID, SiteName FROM Sites ORDER BY SiteName";
            string sqlVariable = "SELECT VariableID, VariableName, UnitsAbbreviation " +
                "FROM Variables INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID ORDER BY VariableName";
            string sqlMethod = "SELECT MethodID, MethodDescription FROM Methods ORDER BY MethodDescription";
            string sqlSource = "SELECT SourceID, Organization FROM Sources ORDER BY Organization";
            string sqlQcLevel = "SELECT QualityControlLevelID, Definition FROM QualityControlLevels ORDER BY Definition";

            _themeTable = Config.DataRepositoryOperations.LoadTable(sqlTheme);
            _siteTable = Config.DataRepositoryOperations.LoadTable(sqlSite);
            _variableTable = Config.DataRepositoryOperations.LoadTable(sqlVariable);
            _sourceTable = Config.DataRepositoryOperations.LoadTable(sqlSource);
            _methodTable = Config.DataRepositoryOperations.LoadTable(sqlMethod);
            _qcLevelTable = Config.DataRepositoryOperations.LoadTable(sqlQcLevel);

            //set variable unit names
            foreach (DataRow row in _variableTable.Rows)
            {
                row["VariableName"] = row["VariableName"] + " (" + row["UnitsAbbreviation"] + ")";
            }

            AddFilterOptionRow(_themeTable);
            AddFilterOptionRow(_siteTable);
            AddFilterOptionRow(_variableTable);
            AddFilterOptionRow(_sourceTable);
            AddFilterOptionRow(_methodTable);
            AddFilterOptionRow(_qcLevelTable);
        }

        private void AddFilterOptionRow(DataTable table)
        {
            string filterText = "Please select filter option";

            DataRow row = table.NewRow();
            row[0] = 0;
            row[1] = filterText;
            table.Rows.InsertAt(row, 0);
        }

        void checkedSeriesList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (checkedSeriesList.SelectedIndex >= 0)
            {
                object item = checkedSeriesList.Items[checkedSeriesList.SelectedIndex];
                int seriesID = GetSeriesID(item);
                OnSelectedIndexChanged(seriesID);
            }
        }


        private string _isExportdlg(DataTable SeriesList, BackgroundWorker exportdlg_worker, DoWorkEventArgs e)
        {
            if (GetCheckedIDList().Count == 0)
            {
                MessageBox.Show("No series are checked. Please check the series to export.");
                return "series are exported.";
            }

            ///<summary>
            /// Complete Data Export codes here if "GetExportOptionsDialog" is used.
            ///</summary>
            return "series are exported.";
        }

        private void AddSeriesCheckBoxes()
        {
            if (checkedSeriesList.IsDisposed)
            {
                return;
            }

            _ignoreCheckEvent = true;

            checkedSeriesList.Items.Clear();

            //add the check boxes, according to visibility in the Selection table
            foreach (DataRow row in _hydroDatabase.SelectionManager.SelectionTable.Rows)
            {
                bool isVisible = Convert.ToBoolean(row["Visible"]);
                bool isChecked = Convert.ToBoolean(row["Checked"]);

                if (isVisible == true)
                {
                    string variableName = row["VariableName"].ToString();
                    string siteName = row["SiteName"].ToString();
                    string seriesID = row["SeriesID"].ToString();
                    checkedSeriesList.Items.Add(variableName + " * " + siteName + " * ID" + seriesID, isChecked);

                    if (isChecked == true)
                    {
                        OnSeriesChecked(Convert.ToInt32(seriesID));
                    }
                }
            }

            _ignoreCheckEvent = false;

            ////To synchronize the selection with other controls
            //IList<int> allSeries = this.HydroDatabase.SelectionManager.CheckedSeriesIDs;
            //_ignoreCheckEvent = true;
            
            //for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            //{
            //    if (!checkedSeriesList.GetItemChecked(i))
            //    {
            //        int seriesID = GetSeriesID(checkedSeriesList.Items[i]);
            //        {
            //            if (allSeries.Contains(seriesID))
            //            {
            //                checkedSeriesList.SetItemChecked(i, true);
            //            }
            //        }
            //    }
            //}
            //_ignoreCheckEvent = false;
            
        }


        public void SelectSeries(int seriesID)
        {
            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                string value = checkedSeriesList.Items[i].ToString();
                string[] lines = Regex.Split(value, " * ID");
                int tempID = Convert.ToInt32(lines.ElementAt(lines.Length - 1));
                if (tempID == seriesID)
                {
                    checkedSeriesList.SetItemChecked(i, true);
                }
            }
        }


        public void UnselectSeries(int seriesID)
        {
            //select the last checked series
            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                string value = checkedSeriesList.Items[i].ToString();
                string[] lines = Regex.Split(value, " * ID");
                int tempID = Convert.ToInt32(lines.ElementAt(lines.Length - 1));
                if (tempID == seriesID)
                {
                    checkedSeriesList.SetItemChecked(i, false);
                }
            }
        }

        #endregion

        #region Event
        /// <summary>
        /// Initialize the SeriesSelector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeriesSelector4_Load(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("SeriesSelector3_Load");
                //Initialize the cbBoxCriterion
                cbBoxCriterion.Items.Add("Please select a filter criterion");
                cbBoxCriterion.Items.Add("Themes");
                cbBoxCriterion.Items.Add("Site");
                cbBoxCriterion.Items.Add("Variable");
                cbBoxCriterion.Items.Add("Method");
                cbBoxCriterion.Items.Add("Source");
                cbBoxCriterion.Items.Add("QCLevel");
                radAll.Checked = true;

                Point listLocation = new Point();
                listLocation.X = 6;
                listLocation.Y = cbBoxCriterion.Top;
                checkedSeriesList.Location = listLocation; //new Point(6, 34);
                checkedSeriesList.Height = groupBox1.Height - 25;
                checkedSeriesList.HorizontalScrollbar = true;

                //Initialize the values
                Initialize();

                //Judge the filter option
                JudgeFilterOption();

                //AddSeriesCheckBoxes();
            }
            catch { }
        }

        /// <summary>
        /// When SimpleFilter RadioButton is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radSimple_Click(object sender, EventArgs e)
        {
            if (_hydroDatabase.SelectionManager.FilterExpression != "") return;
            
            //btnUncheckAll_Click(null, null);
            //MessageBox.Show("simple click");
            //Call CriterionChanged Event in TableView Plug-in so as to clear the Table
            
            //Set Other Filter RadioButton to be unchecked
            radComplex.Checked = false;
            radAll.Checked = false;

            //Fill the cbBoxCriterion with 6 items
            cbBoxCriterion.Items.Clear();
            cbBoxCriterion.Items.Add("Please select a filter criterion");
            cbBoxCriterion.Items.Add("Themes");
            cbBoxCriterion.Items.Add("Site");
            cbBoxCriterion.Items.Add("Variable");
            cbBoxCriterion.Items.Add("Method");
            cbBoxCriterion.Items.Add("Source");
            cbBoxCriterion.Items.Add("QCLevel");

            //Set checkedSeriesList Location and size
            checkedSeriesList.Location = new System.Drawing.Point(6, 90); ;
            checkedSeriesList.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;

            //set default values of criterion
            cbBoxCriterion.SelectedIndex = 1;
            cbBoxContent.SelectedIndex = 0;   
        }

        /// <summary>
        /// When a Criterion Field is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBoxCriterion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string criterionType = cbBoxCriterion.Text;
            switch (criterionType)
            {
                case "Themes":
                    cbBoxContent.DataSource = _themeTable;
                    cbBoxContent.DisplayMember = "ThemeName";
                    cbBoxContent.ValueMember = "ThemeID";
                    break;

                case "Site":
                    cbBoxContent.DataSource = _siteTable;
                    cbBoxContent.DisplayMember = "SiteName";
                    cbBoxContent.ValueMember = "SiteID";
                    break;

                case "Variable":
                    cbBoxContent.DataSource = _variableTable;
                    cbBoxContent.DisplayMember = "VariableName";
                    cbBoxContent.ValueMember = "VariableID";
                    break;

                case "Method":
                    cbBoxContent.DataSource = _methodTable;
                    cbBoxContent.DisplayMember = "MethodDescription";
                    cbBoxContent.ValueMember = "MethodID";
                    break;

                case "Source":
                    cbBoxContent.DataSource = _sourceTable;
                    cbBoxContent.DisplayMember = "Organization";
                    cbBoxContent.ValueMember = "SourceID";
                    break;

                case "QCLevel":
                    cbBoxContent.DataSource = _qcLevelTable;
                    cbBoxContent.DisplayMember = "Definition";
                    cbBoxContent.ValueMember = "QualityControlLevelID";
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        /// <summary>
        /// When a Filter Option is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBoxContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_settingFilter) return;
            
            //only proceed if the filter option is 'Simple'
            if (!radSimple.Checked) return;

            //only proceed if the criterion option is set
            if (cbBoxCriterion.SelectedIndex <= 0) return;
            
            //the first item is the 'Please select filter option' item
            if (cbBoxContent.SelectedIndex <= 0) return;
             
            //simple filter --> Select by the ID
            DataRowView selectedRow = cbBoxContent.SelectedItem as DataRowView;
            string selectedID = selectedRow[0].ToString();
            string queryFilter = "";
            string criterionType = cbBoxCriterion.Text;
            switch (criterionType)
            {
                case "Themes":
                    queryFilter = "ThemeID=" + selectedID;
                    break;
                case "Site":
                    queryFilter = "SiteID=" + selectedID;
                    break;
                case "Variable":
                    queryFilter = "VariableID=" + selectedID;
                    break;
                case "Method":
                    queryFilter = "MethodID=" + selectedID;
                    break;
                case "Source":
                    queryFilter = "SourceID=" + selectedID;
                    break;
                case "QCLevel":
                    queryFilter = "QualityControlLevelID=" + selectedID;
                    break;
            }

            //Notify other plugins about the change
            _hydroDatabase.SelectionManager.SetFilter(FilterTypes.Simple, queryFilter);
            
            
            
           
        }

        /// <summary>
        ///  When ComplexFilter RadioButton is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radComplex_Click(object sender, EventArgs e)
        {
            //btnUncheckAll_Click(null, null);
            Point listLocation = new Point();
            listLocation.X = 6;
            listLocation.Y = cbBoxCriterion.Top;
            checkedSeriesList.Location = listLocation; 
            checkedSeriesList.Height = groupBox1.Height - 25;
            checkedSeriesList.HorizontalScrollbar = true;
            //Clear Table in Plugin TreeView
            
            //Set Other Filter unchecked
            radSimple.Checked = false;
            radAll.Checked = false;
            checkedSeriesList.Items.Clear();

            frmComplexSelection f = new frmComplexSelection(_hydroDatabase.SelectionManager.SelectionTable);
            if (f.ShowDialog() == DialogResult.OK)
            {
                _queryFilter = f.FilterExpression;

            }
        }

        /// <summary>
        /// Gets or sets a boolean that is true if the listbox automatically
        /// changes the checked state when a row is selected.  If this is false,
        /// the checkbox has to be clicked manually, and selection only affects
        /// the highlighting.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets a boolean that is true if the listbox automatically changes the checked state when a row is selected.  If this is false, the checkbox has to be clicked manually, and selection only affects the highlighting.")]
        public bool CheckOnClick
        {
            get
            {
                if (checkedSeriesList == null) return true;
                return checkedSeriesList.CheckOnClick;
            }
            set
            {
                if (checkedSeriesList == null) return;
                checkedSeriesList.CheckOnClick = value;
            }

        }

        /// <summary>
        /// When a series check box is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedSeriesList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _ignoreCheckEvent = true;

            //get the checked or unchecked item
            string checkItem = checkedSeriesList.Items[e.Index].ToString();
            CheckState checkState = e.NewValue;

            if (checkItem == null) return;

            //set the SelectionManager check property
            string checkedSeriesName = checkItem;
            int checkedSeriesID = Convert.ToInt32(checkedSeriesName.Substring(checkedSeriesName.LastIndexOf("* ID") + 4));

            if (checkState == CheckState.Checked)
            {
                _hydroDatabase.SelectionManager.SetSeriesChecked(checkedSeriesID);
                if (_settingFilter == false)
                {
                    OnSeriesChecked(checkedSeriesID);
                }
            }
            else if (checkState == CheckState.Unchecked)
            {
                _hydroDatabase.SelectionManager.SetSeriesUnchecked(checkedSeriesID);
                if (_settingFilter == false)
                {
                    OnSeriesUnchecked(checkedSeriesID);
                }
            }

            _ignoreCheckEvent = false;
        }

        private int GetSeriesID(object checkedItem)
        {
            string checkedSeriesName = checkedItem.ToString();
            int checkedSeriesID = Convert.ToInt32(checkedSeriesName.Substring(checkedSeriesName.LastIndexOf("* ID") + 4));
            return checkedSeriesID;
        }


        /// <summary>
        /// Save Series To Theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //NewTheme f = new NewTheme(CheckedIDList);
            //f.Show();
        }

        /// <summary>
        /// Show the Detailed Property Info of Selected Series
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectedItem = checkedSeriesList.SelectedItem.ToString();
            try
            {
                string selectedID = selectedItem.Substring(selectedItem.LastIndexOf("* ID") + 4);
                DataRow[] rows = _hydroDatabase.SelectionManager.SelectionTable.Select("SeriesID=" + selectedID);
                if (rows.Length > 0)
                {

                    frmProperty f = new frmProperty(rows[0]);
                    f.Show();
                }
            }
            catch { }
        }

        private void bgwTable2Txt_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = _isExportdlg((DataTable) e.Argument, worker, e);
        }

        private void bgwTable2Txt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Export Failed" + e.Error.Message, "Hint",
                MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button2);
            }
        }

        /// <summary>
        /// Export the Selected Series to *.txt File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        #endregion

        

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            //first, unCheck all visible check boxes in the checked list box
            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                checkedSeriesList.SetItemChecked(i, false);
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            //option for making all series checked
            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                checkedSeriesList.SetItemChecked(i, true);
            }
        }
    }
}