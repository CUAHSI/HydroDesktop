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
using HydroDesktop.Interfaces;

namespace HydroDesktop.Controls
{
    /// <summary>
    /// The SeriesSelector3 control enables the filtering and selection
    /// of data series in the table view, graph view and other plug-ins.
    /// </summary>
    public partial class SeriesSelector3 : UserControl
    {
        #region Variable

        private string txtSQLQuery = "";

        //Private Six Criterion List
        
        //private ArrayList themeList = new ArrayList();
        //private ArrayList siteList = new ArrayList();
        //private ArrayList variableList = new ArrayList();
        //private ArrayList methodList = new ArrayList();
        //private ArrayList sourceList = new ArrayList();
        //private ArrayList QCLevelList = new ArrayList();

        //Private Six Criterion Tables
        private DataTable _themeTable;
        private DataTable _siteTable;
        private DataTable _variableTable;
        private DataTable _methodTable;
        private DataTable _sourceTable;
        private DataTable _qcLevelTable;

        private string criterionType = "";

        //Private SQL Expression Builder
        private StringBuilder strSql = new StringBuilder();
        
        //Which Series is checked
        public int CheckedSeriesID = 0;
        public string CheckedSeriesName = "";
        public bool CheckedSeriesState = false;

        public int _selectedSeriesId = 0;

        //the list of checked series IDs
        public ArrayList CheckedIDList = new ArrayList();

        //the list of checked series names
        public ArrayList CheckedSeriesNameList = new ArrayList();

        //when the series checkbox is checked or unchecked
        public event ItemCheckEventHandler SeriesCheck = null;

        //when the filter criterion is changed
        public event EventHandler CriterionChanged = null;

        //when swith to different Tab, filter option(All or Simple) can be synchronized
        private string _filterOption = "";
        private string _flterContent = "";
        private string _selectionPath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            "selection.txt");
        //private bool _initialOptionSimple =false;
        //public bool SwitchOption = false;

        // to build a table of all series for the whole class
        private DataTable _table = new DataTable();

        // the currently used filter expression
        private string _queryFilter = "";
        
        #endregion

        #region Property
        /// <summary>
        /// Allow the user to check multiple series
        /// </summary>
        public bool MultipleCheck { get; set; }

        /// <summary>
        /// The table with detailed properties of all series
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
        }

        /// <summary>
        /// The SQL Filter expression currently used to filter the 
        /// displayed series checkboxes in this control
        /// </summary>
        public string FilterExpression
        {
            get { return _queryFilter; }
        }

        #endregion

        #region Constructor
        public SeriesSelector3()
        {
            InitializeComponent();
            MultipleCheck = true;

            if (DesignMode) return;

            try
            {
                this.radAll.Click += new System.EventHandler(radAll_Click);
                this.radComplex.Click += new System.EventHandler(this.radComplex_Click);
                this.radSimple.Click += new System.EventHandler(this.radSimple_Click);

                //load the table of all available series

                _table = GetDbManager().GetSeriesTable2();

                Initialize();

                checkedSeriesList.MouseDown += new MouseEventHandler(checkedSeriesList_MouseDown);
            }
            catch { }
        }

        private static RepositoryManagerSQL GetDbManager()
        {
            string conString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString;
            RepositoryManagerSQL manager = new RepositoryManagerSQL(new DbOperations(conString, DatabaseTypes.SQLite));
            return manager;
        }

        private static DbOperations GetDbOperations()
        {
            string conString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString;
            DbOperations operations = new DbOperations(conString, DatabaseTypes.SQLite);
            return operations;
        }

        //when a theme is deleted from the HydroDesktop DataRepository database
        void dbManager_ThemeDeleted(object sender, EventArgs e)
        {
            //MessageBox.Show("theme is deleted..");
            RepositoryManagerSQL manager = GetDbManager();
            _table = manager.GetSeriesTable2();
            _queryFilter = "";
            Initialize();
            AddSeriesCheckBoxes();

            //uncheck any series
            for (int i = 0; i < CheckedIDList.Count; i++)
            {
                CheckedSeriesID = (int)CheckedIDList[i];
                CheckedSeriesName = "";
                CheckedSeriesState = false;
                CheckedIDList.RemoveAt(i);

                //force the SeriesCheck event
                OnSeriesCheck();
            }

            RefreshSelection();
        }

        #endregion

        #region Event Handler
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
            //btnUncheckAll_Click(null, null);
            //MessageBox.Show("radAll_Click");
            if (radAll.Checked == true)
            {
                _filterOption = "All";
                try
                {
                    StreamWriter SW;
                    SW = File.CreateText(_selectionPath);
                    SW.WriteLine(_filterOption);
                    //MessageBox.Show("save All");
                    SW.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not save file to disk. Original error: "
                        + ex.Message);
                }
                try
                {
                    ShowAll();
                    OnCriterionChanged();
                }
                catch { }
            }
        }

        #endregion

        #region Method

        public void JudgeSelectOption()
        {              
            //MessageBox.Show("JudgeSelectOption");
            if (System.IO.File.Exists(_selectionPath) == true)
            {
                StreamReader SR;
                SR = File.OpenText(_selectionPath);
                _filterOption = SR.ReadLine();

                if (_filterOption != null)
                {
                    //filter option
                    if (_filterOption == "Simple")
                    {
                        //_initialOptionSimple = true;
                        radAll.Checked = false;
                        radSimple.Checked = true;
                        radComplex.Checked = false;

                        criterionType = SR.ReadLine();
                        _flterContent = SR.ReadLine();
                        SR.Close();
                        switch (criterionType)
                        {
                            case "Themes":
                                _queryFilter = "ThemeID=" + _flterContent;
                                break;
                            case "Site":
                                _queryFilter = "SiteID=" + _flterContent;
                                break;
                            case "Variable":
                                _queryFilter = "VariableID=" + _flterContent;
                                break;
                            case "Method":
                                _queryFilter = "MethodID=" + _flterContent;
                                break;
                            case "Source":
                                _queryFilter = "SourceID=" + _flterContent;
                                break;
                            case "QCLevel":
                                _queryFilter = "QualityControlLevelID=" + _flterContent;
                                break;
                        }
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
                        checkedSeriesList.Items.Clear();

                        for (int i = 0; i < cbBoxCriterion.Items.Count; i++)
                        {
                            if (cbBoxCriterion.Items[i].ToString() == criterionType)
                            {
                                cbBoxCriterion.SelectedIndex = i;
                                break;
                            }
                        }
                        cbBoxContent.SelectedIndex = Convert.ToInt32(_flterContent);
                    }
                    else
                    {
                        SR.Close();
                        radAll.Checked = true;
                        radAll_Click(null, null);
                        //radSimple.Checked = false;
                        //radComplex.Checked = false;
                        //all series
                        _queryFilter = "";
                    }
                }

            }
        }

        private void OnCriterionChanged()
        {
            if (CriterionChanged != null)
            {
                CriterionChanged(this, null);
            }
        }

        private void OnSeriesCheck()
        {
            if (SeriesCheck != null)
            {
                SeriesCheck(this, null);
            }
        }

        public void ChangeTable(DataTable tableOfSeries)
        {
            _table = tableOfSeries;
            MultipleCheck = true;
            Initialize();

            if (radAll.Checked == true)
            {
                ShowAll();
            }
        }

        public void ShowAll()
        {       
            Point listLocation = new Point();
            listLocation.X = 6;
            listLocation.Y = cbBoxCriterion.Top;
            checkedSeriesList.Location = listLocation; //new Point(6, 34);
            checkedSeriesList.Height = groupBox1.Height - 25;
            checkedSeriesList.HorizontalScrollbar = true;
            
            OnCriterionChanged();
            //Set Other Filter unchecked
            radSimple.Checked = false;
            radComplex.Checked = false;

            //all series
            _queryFilter = "";

            //refresh selection
            RefreshSelection();
        }

        /// <summary>
        /// Initializes the collections and lists of controls
        /// </summary>
        private void Initialize()
        {
            string sqlTheme = "SELECT ThemeID, ThemeName FROM DataThemeDescriptions";
            string sqlSite = "SELECT SiteID, SiteName FROM Sites";
            string sqlVariable = "SELECT VariableID, VariableName, UnitsAbbreviation " +
                "FROM Variables INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID";
            string sqlMethod = "SELECT MethodID, MethodDescription FROM Methods";
            string sqlSource = "SELECT SourceID, Organization FROM Sources";
            string sqlQcLevel = "SELECT QualityControlLevelID, Definition FROM QualityControlLevels";

            DbOperations db = GetDbOperations();
            _themeTable = db.LoadTable(sqlTheme);
            _siteTable = db.LoadTable(sqlSite);
            _variableTable = db.LoadTable(sqlVariable);
            _sourceTable = db.LoadTable(sqlSource);
            _methodTable = db.LoadTable(sqlMethod);
            _qcLevelTable = db.LoadTable(sqlQcLevel);

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

        private string _isExportdlg(DataTable SeriesList, BackgroundWorker exportdlg_worker, DoWorkEventArgs e)
        {
            if (CheckedIDList.Count == 0)
            {
                MessageBox.Show("No series are checked. Please check the series to export.");
                return "series are exported.";
            }

            ///<summary>
            /// Complete Data Export codes here if "GetExportOptionsDialog" is used.
            ///</summary>
            return "series are exported.";
        }

        private void SaveSelection(int seriesID, bool insertOrDele)
        {
            //To judge whether the Table "Selection" exist or not
            if (CheckTableExist() == false)
            {
                //Create Table "Selection"
                txtSQLQuery = "CREATE TABLE Selection(SeriesID INTEGER PRIMARY KEY NOT NULL)";
                ExecuteQuery(txtSQLQuery);            
            }
            //Insert or Delete the selected series in Table "Selection"
            if (insertOrDele == true)
            {
                txtSQLQuery = "INSERT INTO Selection(SeriesID) VALUES (" + seriesID + ")";
            }
            else
            {
                txtSQLQuery = "DELETE FROM Selection WHERE SeriesID = " + seriesID;
            }
            ExecuteQuery(txtSQLQuery); 
        }

        private void AddSeriesCheckBoxes()
        {
            checkedSeriesList.Items.Clear();
            DataRow[] matchedRows = _table.Select(_queryFilter);
            foreach (DataRow row in matchedRows)
            {
                checkedSeriesList.Items.Add((row["VariableName"]).ToString() + " * " + (row["SiteName"]).ToString() + " * ID" + (row["SeriesID"]).ToString());
            }
        }

        /// <summary>
        /// Refresh the check boxes state according to saved selection in the database
        /// </summary>
        public void RefreshSelection()
        {
            //to refresh the filter options
            Initialize();

            if (CheckTableExist() == false)
            {
                txtSQLQuery = "CREATE TABLE Selection(SeriesID INTEGER PRIMARY KEY NOT NULL)";
                ExecuteQuery(txtSQLQuery);
            }

            DbOperations operations = GetDbOperations();
            RepositoryManagerSQL manager = new RepositoryManagerSQL(operations);

            _table = manager.GetSeriesTable2();
            AddSeriesCheckBoxes();

            // if MultipleCheck is false, only select the most recently selected series
            if (MultipleCheck == false)
            {
                //delete all items from the selection table
                string commandText = "delete from selection";
                operations.ExecuteNonQuery(commandText);

                //use the last item from the CheckedIDList
                if (CheckedIDList.Count > 0)
                {
                    CheckedSeriesID = Convert.ToInt32(CheckedIDList[CheckedIDList.Count - 1]);
                    CheckedSeriesName = Convert.ToString(CheckedSeriesNameList[CheckedSeriesNameList.Count - 1]);

                    //unselect all series
                    checkedSeriesList.ClearSelected();
                    CheckedIDList.Clear();
                    CheckedSeriesNameList.Clear();

                    //if the series with checked id still exists in the database:

                    //select the last checked series
                    for (int i = 0; i < checkedSeriesList.Items.Count; i++)
                    {
                        string value = checkedSeriesList.Items[i].ToString();
                        string[] lines = Regex.Split(value, " * ID");
                        int tempID = Convert.ToInt32(lines.ElementAt(lines.Length - 1));
                        if (tempID == CheckedSeriesID)
                        {
                            checkedSeriesList.SetItemChecked(i, true);
                        }
                    }

                    CheckedIDList.Add(CheckedSeriesID);
                    CheckedSeriesNameList.Add(CheckedSeriesName);

                    //save selected series id to database
                    SaveSelection(CheckedSeriesID, true);
                }
                else
                {
                    checkedSeriesList.ClearSelected();
                }
            }
            else //MultipleCheck is true
            {
                string commandText = "select SeriesID from Selection";
                DataTable DT = GetDbOperations().LoadTable("selection", commandText);

                ArrayList allChecked = new ArrayList();
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    allChecked.Add(DT.Rows[i][0].ToString());
                }

                //uncheck any series
                for (int i = 0; i < CheckedIDList.Count; i++)
                {
                    string objCheckedID = CheckedIDList[i].ToString();
                    if (!allChecked.Contains(objCheckedID))
                    {
                        CheckedSeriesID = (int)CheckedIDList[i];
                        CheckedIDList.RemoveAt(i);
                        CheckedSeriesName = "";
                        CheckedSeriesState = false;

                        //force the SeriesCheck event
                        Debug.WriteLine("uncheck series: " + CheckedSeriesID);
                        OnSeriesCheck();
                    }
                }

                //check any new series
                for (int i = 0; i < checkedSeriesList.Items.Count; i++)
                {
                    string value = checkedSeriesList.Items[i].ToString();
                    string[] lines = Regex.Split(value, " * ID");
                    int tempID = Convert.ToInt32(lines.ElementAt(lines.Length - 1));
                    if (allChecked.Contains(tempID.ToString()))
                    {
                        checkedSeriesList.SetItemChecked(i, true);

                        if (!CheckedIDList.Contains(tempID))
                        {
                            CheckedIDList.Add(tempID);
                            CheckedSeriesID = tempID;
                            CheckedSeriesName = value;
                            CheckedSeriesState = true;

                            //force the SeriesCheck event
                            OnSeriesCheck();
                        }
                    }
                    else
                    {
                        checkedSeriesList.SetItemChecked(i, false);
                    }
                }

            }
        }

        private void ExecuteQuery(string txtQuery)
        {
            GetDbOperations().ExecuteNonQuery(txtQuery);
        }

        /// <summary>
        /// To judge whether the Table "Selection" exist or not
        /// </summary>
        /// <returns></returns>
        private bool CheckTableExist()
        {
            bool tableExists = false;
                
                string commandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";

                DataTable tableInfo = GetDbOperations().LoadTable("tableInfo", commandText);

                for (int i = 0; i < tableInfo.Rows.Count; i++)
                {
                    if (tableInfo.Rows[i][0].ToString() == "Selection")
                    {
                        tableExists = true;
                        break;
                    }
                }
                return tableExists;
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
        private void SeriesSelector3_Load(object sender, EventArgs e)
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

                OnCriterionChanged();
                //Set Other Filter unchecked
                //radSimple.Checked = false;
                //radComplex.Checked = false;
                //
                //Judge the filter option
                //
                if (System.IO.File.Exists(_selectionPath) == true)
                {
                    StreamReader SR;
                    SR = File.OpenText(_selectionPath);
                    _filterOption = SR.ReadLine();
                    
                    if (_filterOption != null)
                    {
                        //filter option
                        if (_filterOption == "Simple")
                        {
                            //_initialOptionSimple = true;
                            radAll.Checked = false;
                            radSimple.Checked = true;
                            radComplex.Checked = false;

                            criterionType = SR.ReadLine();
                            _flterContent = SR.ReadLine();
                            SR.Close();
                            switch (criterionType)
                            {
                                case "Themes":
                                    _queryFilter = "ThemeID=" + _flterContent;
                                    break;
                                case "Site":
                                    _queryFilter = "SiteID=" + _flterContent;
                                    break;
                                case "Variable":
                                    _queryFilter = "VariableID=" + _flterContent;
                                    break;
                                case "Method":
                                    _queryFilter = "MethodID=" + _flterContent;
                                    break;
                                case "Source":
                                    _queryFilter = "SourceID=" + _flterContent;
                                    break;
                                case "QCLevel":
                                    _queryFilter = "QualityControlLevelID=" + _flterContent;
                                    break;
                            }
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
                            checkedSeriesList.Items.Clear();

                            for (int i = 0; i < cbBoxCriterion.Items.Count; i++)
                            {
                                if (cbBoxCriterion.Items[i].ToString() == criterionType)
                                {
                                    cbBoxCriterion.SelectedIndex = i;
                                    break;
                                }
                            }
                            cbBoxContent.SelectedIndex = Convert.ToInt32(_flterContent);

                        }
                        else
                        {
                            SR.Close();
                            radAll.Checked = true;
                            radAll_Click(null, null);
                            //radSimple.Checked = false;
                            //radComplex.Checked = false;
                            //all series
                            _queryFilter = "";
                            
                        }
                    }
                  
                }

                if (radAll.Checked == true)
                {
                    try
                    {
                        string binariesDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string selectionPath = Path.Combine(binariesDirectory, "selection.txt");
                        StreamWriter SW;
                        SW = File.CreateText(selectionPath);
                        SW.WriteLine("All");
                        SW.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not save file to disk. Original error: "
                            + ex.Message);
                    }
                }

                AddSeriesCheckBoxes();
                //ShowAll();
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
            //btnUncheckAll_Click(null, null);
            //MessageBox.Show("simple click");
            //Call CriterionChanged Event in TableView Plug-in so as to clear the Table
            OnCriterionChanged();

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

            //Store query option in selection.txt file,which is in Binary Folder
            if (_filterOption != "Simple")
            {
                _filterOption = "Simple";
                //set default values of criterion
                //cbBoxCriterion.SelectedIndex = 0;
                //cbBoxContent.SelectedIndex = -1;
                cbBoxCriterion.SelectedIndex = 1;
                cbBoxContent.SelectedIndex = 0;

                checkedSeriesList.Items.Clear();
            }
            else
            {
                for (int i = 0; i < cbBoxCriterion.Items.Count; i++)
                {
                    if (cbBoxCriterion.Items[i].ToString() == criterionType)
                    {
                        cbBoxCriterion.SelectedIndex = i;
                        break;
                    }
                }
                cbBoxContent.SelectedIndex = Convert.ToInt32(_flterContent);
                AddSeriesCheckBoxes();
            }
        }

        /// <summary>
        /// When a Criterion Field is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBoxCriterion_SelectedIndexChanged(object sender, EventArgs e)
        {
            criterionType = cbBoxCriterion.Text;
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
            if (cbBoxContent.SelectedIndex <= 0) return;
            
            OnCriterionChanged();
            checkedSeriesList.Items.Clear();
            
            if (radSimple.Checked == true)
            {
                //simple filter
                DataRowView selectedRow = cbBoxContent.SelectedItem as DataRowView;
                string selectedID = selectedRow[0].ToString();
                switch (criterionType)
                {
                    case "Themes":
                        _queryFilter = "ThemeID=" + selectedID;
                        break;
                    case "Site":
                        _queryFilter = "SiteID=" + selectedID;
                        break;
                    case "Variable":
                        _queryFilter = "VariableID=" + selectedID;
                        break;
                    case "Method":
                        _queryFilter = "MethodID=" + selectedID;
                        break;
                    case "Source":
                        _queryFilter = "SourceID=" + selectedID;
                        break;
                    case "QCLevel":
                        _queryFilter = "QualityControlLevelID=" + selectedID;
                        break;
                }

                //if (_initialOptionSimple != true)
                //{
                    //MessageBox.Show("Save simple");
                _flterContent = cbBoxContent.SelectedIndex.ToString();

                    try
                    {
                        StreamWriter SW;
                        SW = File.CreateText(_selectionPath);
                        SW.WriteLine(_filterOption);
                        SW.WriteLine(criterionType);
                        SW.WriteLine(_flterContent);
                        SW.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not save file to disk. Original error: "
                            + ex.Message);
                    }
                    //_initialOptionSimple=false;
                //}
                
                RefreshSelection();
            }
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
            OnCriterionChanged();
            //Set Other Filter unchecked
            radSimple.Checked = false;
            radAll.Checked = false;
            checkedSeriesList.Items.Clear();

            frmComplexSelection f = new frmComplexSelection(_table);
            if (f.ShowDialog() == DialogResult.OK)
            {
                _queryFilter = f.FilterExpression;

                RefreshSelection();
            }
            //
            //Store query option in selection.txt file,which is in Binary Folder
            //
            _filterOption = "Complex";
            try
            {
                StreamWriter SW;
                SW = File.CreateText(_selectionPath);
                SW.WriteLine(_filterOption);
                SW.WriteLine(_queryFilter);
                SW.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not save file to disk. Original error: "
                    + ex.Message);
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
        /// When A Series is Selected (a series check box is checked)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 
            checkedSeriesList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
            //get the checked or unchecked item
            string checkItem = checkedSeriesList.Items[e.Index].ToString();

            //set the CheckedSeriesState property
            if (e.NewValue == CheckState.Checked)
            {
                CheckedSeriesState = true;
            }
            else
            {
                CheckedSeriesState = false;
            }

            if (checkItem != null)
            {
                if (checkItem.Contains('*'))
                {
                    try
                    {
                        CheckedSeriesName = checkItem;
                        CheckedSeriesID = Convert.ToInt32(CheckedSeriesName.Substring(CheckedSeriesName.LastIndexOf("* ID") + 4));

                            //Checked
                            if (e.NewValue == CheckState.Checked)
                            {
                                if (!CheckedIDList.Contains(CheckedSeriesID))
                                {
                                    CheckedIDList.Add(CheckedSeriesID);
                                    CheckedSeriesNameList.Add(CheckedSeriesName);
                                    //Check if Id is already exists
                                    string sqlID = "SELECT SeriesID FROM Selection";
                                    DataTable selectionTable = GetDbOperations().LoadTable(sqlID);
                                    ArrayList iDs=new ArrayList();
                                    for (int i = 0; i < selectionTable.Rows.Count; i++)
                                    {
                                        iDs.Add(selectionTable.Rows[i][0].ToString());
                                    }
                                    if(!iDs.Contains(CheckedSeriesID.ToString()))
                                        SaveSelection(CheckedSeriesID, true);
                                }
                            }
                            //Unchecked
                            else
                            {
                                CheckedIDList.Remove(CheckedSeriesID);
                                CheckedSeriesNameList.Remove(CheckedSeriesName);
                                SaveSelection(CheckedSeriesID, false);
                                //MessageBox.Show("This series is Unchecked");
                            }
                        
                    }
                    catch
                    {
                    }
                }
                //if multiple selection is disabled
                if (MultipleCheck == false)
                {
                    for (int ix = 0; ix < checkedSeriesList.Items.Count; ix++)
                    {
                        if (e.Index != ix)
                        {
                            checkedSeriesList.SetItemChecked(ix, false);
                        }
                    }
                }
            }
            OnSeriesCheck();
        }


        /// <summary>
        /// Save Series To Theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTheme f = new NewTheme(CheckedIDList);
            f.Show();
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
                DataRow[] rows = _table.Select("SeriesID=" + selectedID);
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
            if (bgwTable2Txt.IsBusy)
            {
                MessageBox.Show("The background worker is busy now, please try later.");
                return;
            }

            //All the Series Table
            DataTable allSeriesList = GetDbManager().GetSeriesListTable();
            bgwTable2Txt.RunWorkerAsync(allSeriesList);

            //Build select strings
            StringBuilder SQLString = new StringBuilder();

            if (CheckedIDList.Count == 0)
            {
                MessageBox.Show("Please select at least one series");
            }

            else
            {
                if (CheckedIDList.Count == 1)
                {
                    SQLString.Append("SeriesID = ");
                    SQLString.Append(CheckedIDList[0].ToString());
                }
                else
                {
                    foreach (int seriesID in CheckedIDList)
                    {
                        SQLString.Append("SeriesID = ");
                        SQLString.Append(seriesID.ToString());
                        SQLString.Append(" OR ");
                    }
                    SQLString.Remove(SQLString.Length - 4, 4);
                }

                //Build original datatable for data export
                
                DataTable table = new DataTable();
                DataTable exportTable = new DataTable();

                DbOperations _dboperation;
                _dboperation = GetDbOperations();
                table = allSeriesList.Clone();
                exportTable = table.Clone();

                DataRow[] foundRows = allSeriesList.Select(SQLString.ToString());

                //Export DataValues of the selected series instead of just series list!!
                for (int i = 0; i < foundRows.Length; i++)
                {
                    String list;

                    list = "SELECT ds.SeriesID, s.SiteName, dv.LocalDateTime, dv.DataValue, v.VariableName, U.UnitsName, s.SiteCode, s.Latitude, s.Longitude, " +
                        "dv.UTCOffset, dv.DateTimeUTC, v.DataType, dv.ValueAccuracy, m.MethodDescription, " +
                        "v.ValueType, q.QualityControlLevelCode, v.SampleMedium, v.GeneralCategory, S.Organization, S.SourceDescription, S.SourceLink " +
                        "FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U, Methods m, QualityControlLevels q, Sources S " +
                        "WHERE v.VariableID = ds.VariableID " +
                        "AND s.SiteID = ds.SiteID " +
                        "AND m.MethodID = ds.MethodID " +
                        "AND q.QualityControlLevelID = ds.QualityControlLevelID " +
                        "AND S.SourceID = ds.SourceID " +
                        "AND dv.SeriesID = ds.SeriesID " +
                        "AND U.UnitsID = v.VariableUnitsID " +
                        "AND ds.SeriesID = " + foundRows[i]["SeriesID"].ToString();

                    table = _dboperation.LoadTable("seriesTable", list);

                    foreach (DataRow row in table.Rows)
                    {
                        exportTable.ImportRow(row);
                    }
                }        

                //for (int i = 0; i < foundRows.Length; i++)
                //{
                //    table.ImportRow(foundRows[i]);
                //}

                HydroDesktop.ImportExport.ExportDataTableToTextFileDialog exportForm = new HydroDesktop.ImportExport.ExportDataTableToTextFileDialog(exportTable);
                exportForm.ShowDialog();
            }
        }

        #endregion

        private void checkedSeriesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedSeriesList.SelectedItem.ToString() != null)
            {
                string splitValue = checkedSeriesList.SelectedItem.ToString();
                _selectedSeriesId = Convert.ToInt32(splitValue.Substring(splitValue.LastIndexOf("* ID") + 4));
                //MessageBox.Show(_selectedSeriesId.ToString());
            }
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            //first, unCheck all visible check boxes in the checked list box
            for (int i = 0; i < checkedSeriesList.Items.Count; i++)
            {
                checkedSeriesList.SetItemChecked(i, false);
            }

            //automatically force the uncheck event for all other series
           

            ////uncheck any series
            //for (int i = 0; i < CheckedIDList.Count; i++)
            //{
            //    string objCheckedID = CheckedIDList[i].ToString();
                
            //    CheckedSeriesID = (int)CheckedIDList[i];
            //    CheckedIDList.RemoveAt(i);
            //    CheckedSeriesName = "";
            //    CheckedSeriesState = false;

            //    //force the SeriesCheck event
            //    Debug.WriteLine("uncheck series: " + CheckedSeriesID);
            //    OnSeriesCheck();   
            //}

            ////special case when the list has only one item
            //if (CheckedIDList.Count == 1)
            //{
            //    string objCheckedID = CheckedIDList[0].ToString();

            //    CheckedSeriesID = (int)CheckedIDList[0];
            //    CheckedIDList.Clear();
            //    CheckedSeriesName = "";
            //    CheckedSeriesState = false;

            //    //force the SeriesCheck event
            //    Debug.WriteLine("uncheck series: " + CheckedSeriesID);
            //    OnSeriesCheck();   

            //}

            //clear the CheckedSeriesNameList
            CheckedSeriesNameList.Clear();

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