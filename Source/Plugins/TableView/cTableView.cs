using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using HydroDesktop.Interfaces;
using HydroDesktop.Database;
using HydroDesktop.Configuration;

namespace TableView
{
    public partial class cTableView : UserControl
    {
        #region privateDelaration
        //Store the series already selected
        private ArrayList sriesList = new ArrayList();
        private bool sequenceSwitch = true;

        private readonly ISeriesSelector _seriesSelector;

        //the table of data values
        private DataTable _dataValuesTable = new DataTable();
        private bool _seriesCheckState;

        #endregion

        #region Constructor
        public cTableView(ISeriesSelector seriesSelector)
        {
            InitializeComponent();
            //to access the map and database elements
            _seriesSelector = seriesSelector;

            ShowAllFieldsinSequence();
            
           // bindingSource1.DataSource = _dataValuesTable;
           // dataViewSeries.DataSource = bindingSource1;

            dataGridViewNavigator1.PageChanged += dataGridViewNavigator1_PageChanged;

            //the SeriesChecked event
            _seriesSelector.SeriesCheck += SeriesSelector_SeriesCheck;
            _seriesSelector.Refreshed += _seriesSelector_Refreshed;
        }
        
        #endregion

        #region Methods

        void dataGridViewNavigator1_PageChanged(object sender, PageChangedEventArgs e)
        {
            dataViewSeries.DataSource = e.DataTable;
        }

        private static DbOperations GetDbOperations()
        {
            return new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
        }

        private void ShowAllFieldsinSequence()
        {
            string whereClause;
            if (_seriesSelector.CheckedIDList.Length == 0)
            {
                whereClause = "1 = 0";
            }
            else
            {
                var sb = new StringBuilder("SeriesID in (");
                foreach (var id in _seriesSelector.CheckedIDList)
                    sb.AppendFormat(" {0},", id);
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                whereClause = sb.ToString();
            }

            var dbTools = GetDbOperations();
            var dataQuery = "SELECT ValueID, SeriesID, DataValue, LocalDateTime, UTCOffset, CensorCode FROM DataValues WHERE " + whereClause;
            var countQuery = "select count(*) from DataValues WHERE " + whereClause;
            dataGridViewNavigator1.Initialize(dbTools, dataQuery, countQuery);
        }

        private void ShowJustValuesinParallel()
        {
            bindingSource1.DataSource = _dataValuesTable;
            dataViewSeries.DataSource = bindingSource1;

            //var sequenceSwitch = rbSequence.Checked;

            // Judge whether switch from ShowAllFieldsinSequence Option or not
            // not switch from ShowAllFieldsinSequence Option
            if (sequenceSwitch == false)
            {
                //If UnChecked, Delete the Series
                if (_seriesCheckState == false) // max
                {
                    DbOperations dbTools = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                    string sqlQuery = "SELECT UnitsName, SiteName, VariableName FROM DataSeries " +
                        "INNER JOIN Variables ON Variables.VariableID = DataSeries.VariableID " +
                        "INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID " +
                        "INNER JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = " + _seriesSelector.SelectedSeriesID;

                    DataTable seriesNameTable = dbTools.LoadTable("table", sqlQuery);
                    DataRow row1 = seriesNameTable.Rows[0];
                    string siteName = Convert.ToString(row1[1]);
                    string variableName = Convert.ToString(row1[2]);
                    string unCheckedName = siteName + _seriesSelector.SelectedSeriesID.ToString() + " * " + variableName;

                    for (int i = 1; i < dataViewSeries.Columns.Count; i++)
                    {
                        if (dataViewSeries.Columns[i].Name == unCheckedName)
                        {
                            dataViewSeries.Columns.RemoveAt(i);
                            int seriesID = _seriesSelector.SelectedSeriesID;
                            sriesList.Remove(seriesID.ToString());
                            i--;
                        }
                    }
                    if (dataViewSeries.Columns.Count == 1)
                    {
                        dataViewSeries.DataSource = null;
                        dataViewSeries.Columns.Clear();
                    }
                }
                //If Checked, Add the Series
                else
                {
                    //Get Fields SeriesID,LocalDateTime,DataValue FROM DataValues
                    DbOperations dbTools = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                    DataTable tblSeries = new DataTable();
                    StringBuilder SQLString = new StringBuilder();
                    if (_seriesSelector.CheckedIDList.Length == 1)
                    {
                        SQLString.Append("SELECT SeriesID,LocalDateTime,DataValue FROM DataValues WHERE SeriesID = ");
                        SQLString.Append(_seriesSelector.CheckedIDList[0]);
                        SQLString.Append(" ORDER By LocalDateTime");
                    }
                    else
                    {
                        SQLString.Append("SELECT SeriesID,LocalDateTime,DataValue FROM DataValues WHERE SeriesID IN ( ");
                        foreach (int seriesID in _seriesSelector.CheckedIDList)
                        {
                            SQLString.Append(seriesID);
                            SQLString.Append(",");
                        }
                        SQLString.Remove(SQLString.Length - 1, 1);
                        SQLString.Append(")");
                        SQLString.Append(" ORDER By SeriesID, LocalDateTime");
                    }
                    tblSeries = dbTools.LoadTable("DataValues", SQLString.ToString());
                    //Get each SeriesID
                    int startNum = 0;
                    if (sriesList.Count == _seriesSelector.CheckedIDList.Length)
                    {
                        sriesList.Clear();
                    }
                    else
                    {
                        startNum = sriesList.Count;
                    }

                    for (int k = startNum; k < _seriesSelector.CheckedIDList.Length; k++)
                    {
                        int seriesID = Convert.ToInt32(_seriesSelector.CheckedIDList[k].ToString());
                        if (!sriesList.Contains(seriesID.ToString()))
                        {
                            string expression;
                            expression = "SeriesID= " + seriesID.ToString();
                            DataRow[] foundRows;
                            // Use the Select method to find all rows matching the filter.
                            foundRows = tblSeries.Select(expression);
                            //Add A Column for a new series

                            string sqlQuery = "SELECT UnitsName, SiteName, VariableName FROM DataSeries " +
                                "INNER JOIN Variables ON Variables.VariableID = DataSeries.VariableID " +
                                "INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID " +
                                "INNER JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = " + seriesID;

                            DataTable seriesNameTable = dbTools.LoadTable("table", sqlQuery);
                            DataRow row1 = seriesNameTable.Rows[0];
                            string unitsName = Convert.ToString(row1[0]);
                            string siteName = Convert.ToString(row1[1]);
                            string variableName = Convert.ToString(row1[2]);
                            //If LocalDateTime Column Already Exists, Only Add DataValues
                            if (dataViewSeries.Columns.Contains("DateTime"))
                            {
                                DataGridViewTextBoxColumn sriesColumn = new DataGridViewTextBoxColumn();
                                sriesColumn.HeaderText = siteName + " * " + seriesID.ToString() + "\r\n" + variableName + "\r\n" + unitsName;
                                sriesColumn.Name = siteName + seriesID.ToString() + " * " + variableName;

                                //sriesColumn.Width = 400;
                                sriesColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                                bool seriesExit=false;
                                for (int i = 0; i < dataViewSeries.Columns.Count; i++)
                                {
                                    if (dataViewSeries.Columns[i].HeaderText== sriesColumn.HeaderText)
                                    {
                                        seriesExit = true;
                                        break;
                                    }
                                }
                                if (seriesExit==false)
                                {
                                    this.dataViewSeries.Columns.Add(sriesColumn);
                                    //Get all the sriesDataTime
                                    ArrayList sriesDataTime = new ArrayList();
                                    for (int i = 0; i < this.dataViewSeries.Rows.Count - 1; i++)
                                    {
                                        sriesDataTime.Add(this.dataViewSeries.Rows[i].Cells["DateTime"].Value.ToString());
                                    }
                                    for (int i = 0; i < foundRows.Length; i++)//foreach (DataRow r in tblSeries.Rows)
                                    {
                                        int idS = sriesDataTime.IndexOf(foundRows[i][1].ToString());
                                        if (idS != -1)
                                        {
                                            this.dataViewSeries.Rows[idS].Cells[sriesColumn.Name].Value = foundRows[i][2].ToString();
                                        }
                                        else
                                        {
                                            int n = dataViewSeries.Rows.Add();
                                            dataViewSeries.Rows[n].Cells[0].Value = foundRows[i][1].ToString();
                                            dataViewSeries.Rows[n].Cells[sriesColumn.Name].Value = foundRows[i][2].ToString();
                                        }
                                    }
                                    for (int i = 0; i < this.dataViewSeries.Rows.Count - 1; i++)
                                    {
                                        for (int j = 0; j < this.dataViewSeries.Columns.Count; j++)
                                        {
                                            if (this.dataViewSeries.Rows[i].Cells[j].EditedFormattedValue.ToString() == "")
                                                this.dataViewSeries.Rows[i].Cells[j].Value = "";
                                        }
                                    }
                                }
                            }
                            //Add LocalDateTime & DataValues
                            else
                            {
                                dataViewSeries.ColumnCount = 2;
                                // Set the column header names.
                                dataViewSeries.Columns[0].Name = "DateTime";
                                dataViewSeries.Columns[0].HeaderText = "DateTime\r\nUnit";
                                //dataViewSeries.Columns[0].Width = 150;
                                dataViewSeries.Columns[1].HeaderText = siteName + " * " + seriesID.ToString() + "\r\n" + variableName + "\r\n" + unitsName;
                                dataViewSeries.Columns[1].Name = siteName + seriesID.ToString() + " * " + variableName;

                                //dataViewSeries.Columns[1].Width = 400;
                                // Populate the rows.
                                for (int i = 0; i < foundRows.Length; i++)//foreach (DataRow r in tblSeries.Rows)
                                {
                                    string[] row = new string[] { foundRows[i][1].ToString(), foundRows[i][2].ToString() };
                                    dataViewSeries.Rows.Add(row);
                                }
                            }
                            sriesList.Add(seriesID.ToString());
                        }
                    }
                }
                this.dataViewSeries.RowsDefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            }
            else //Switch from ShowAllFieldsinSequence Option
            {
                //reset 
                
                //MessageBox.Show("sequenceSwitch == true");
                sriesList.Clear();
                DbOperations dbTools =
                       new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                DataTable tblSeries = new DataTable();
                //Construct SQL
                StringBuilder SQLString = new StringBuilder();
                if (_seriesSelector.CheckedIDList.Length == 1)
                {
                    SQLString.Append("SELECT SeriesID,LocalDateTime,DataValue FROM DataValues WHERE SeriesID = ");
                    SQLString.Append(_seriesSelector.CheckedIDList[0]);
                }
                else
                {
                    SQLString.Append("SELECT SeriesID,LocalDateTime,DataValue FROM DataValues WHERE SeriesID IN ( ");
                    foreach (int seriesID in _seriesSelector.CheckedIDList)
                    {
                        SQLString.Append(seriesID);
                        SQLString.Append(",");
                    }
                    SQLString.Remove(SQLString.Length - 1, 1);
                    SQLString.Append(")");
                }
                tblSeries = dbTools.LoadTable("DataValues", SQLString.ToString());
                sequenceSwitch = false;
                for (int k = 0; k < _seriesSelector.CheckedIDList.Length; k++)
                {
                    int seriesID = Convert.ToInt32(_seriesSelector.CheckedIDList[k].ToString());
                    if (!sriesList.Contains(seriesID.ToString()))
                    {
                        string expression;
                        expression = "SeriesID= " + seriesID.ToString();
                        DataRow[] foundRows;
                        // Use the Select method to find all rows matching the filter.
                        foundRows = tblSeries.Select(expression);

                        string sqlQuery = "SELECT UnitsName, SiteName, VariableName FROM DataSeries " +
                            "INNER JOIN Variables ON Variables.VariableID = DataSeries.VariableID " +
                            "INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID " +
                            "INNER JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = " + seriesID;

                        DataTable seriesNameTable = dbTools.LoadTable("table", sqlQuery);
                        DataRow row1 = seriesNameTable.Rows[0];
                        string unitsName = Convert.ToString(row1[0]);
                        string siteName = Convert.ToString(row1[1]);
                        string variableName = Convert.ToString(row1[2]);

                        if (!dataViewSeries.Columns.Contains("DateTime"))
                        {
                            dataViewSeries.ColumnCount = 2;
                            dataViewSeries.Columns[0].Name = "DateTime";
                            dataViewSeries.Columns[0].HeaderText = "DateTime\r\nUnit";
                            //dataViewSeries.Columns[0].Width = 150;
                            dataViewSeries.Columns[1].HeaderText = siteName + " * " + seriesID.ToString() + "\r\n" + variableName + "\r\n" + unitsName;
                            dataViewSeries.Columns[1].Name = siteName + seriesID.ToString() + " * " + variableName;

                            dataViewSeries.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            //dataViewSeries.Columns[1].Width = 400;

                            // Populate the DateTime rows.
                            for (int i = 0; i < foundRows.Length; i++)//foreach (DataRow r in tblSeries.Rows)
                            {
                                string[] row = new string[] { foundRows[i][1].ToString(), foundRows[i][2].ToString() };
                                dataViewSeries.Rows.Add(row);
                            }
                        }
                        else
                        {
                            //Add A Column for a new series
                            DataGridViewTextBoxColumn sriesColumn = new DataGridViewTextBoxColumn();
                            sriesColumn.HeaderText = siteName + " * " + seriesID.ToString() + "\r\n" + variableName + "\r\n" + unitsName;
                            sriesColumn.Name = siteName + seriesID.ToString() + " * " + variableName;

                            //sriesColumn.Width = 400;
                            this.dataViewSeries.Columns.Add(sriesColumn);
                            //Get all the sriesDataTime
                            ArrayList sriesDataTime = new ArrayList();
                            for (int i = 0; i < this.dataViewSeries.Rows.Count - 1; i++)
                            {
                                sriesDataTime.Add(this.dataViewSeries.Rows[i].Cells["DateTime"].Value.ToString());
                            }
                            for (int i = 0; i < foundRows.Length; i++)//foreach (DataRow r in tblSeries.Rows)
                            {
                                int idS = sriesDataTime.IndexOf(foundRows[i][1].ToString());
                                if (idS != -1)
                                {
                                    this.dataViewSeries.Rows[idS].Cells[sriesColumn.Name].Value = foundRows[i][2].ToString();
                                }
                                else
                                {
                                    int n = dataViewSeries.Rows.Add();
                                    dataViewSeries.Rows[n].Cells[0].Value = foundRows[i][1].ToString();
                                    dataViewSeries.Rows[n].Cells[sriesColumn.Name].Value = foundRows[i][2].ToString();
                                }
                            }
                            for (int i = 0; i < this.dataViewSeries.Rows.Count - 1; i++)
                            {
                                for (int j = 0; j < this.dataViewSeries.Columns.Count; j++)
                                {
                                    if (this.dataViewSeries.Rows[i].Cells[j].EditedFormattedValue.ToString() == "")
                                        this.dataViewSeries.Rows[i].Cells[j].Value = "";
                                }
                            }
                        }
                        sriesList.Add(seriesID.ToString());
                    }
                }
            }
        }
        #endregion

        #region Event
        /// <summary>
        /// cTableView Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cTableView_Load(object sender, EventArgs e)
        {
            //populate the series selector control
            dataViewSeries.ColumnHeadersVisible = true;
            rbSequence.Checked = true;
            dataViewSeries.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;

            lblDatabase.Text = GetSQLitePath(Settings.Instance.DataRepositoryConnectionString);
        }

        private string GetSQLitePath(string sqliteConnString)
        {
            return SQLiteHelper.GetSQLiteFileName(sqliteConnString);
        }

        /// <summary>
        /// Remove the column header border in the Aero theme in Vista,
        /// but keep it for other themes such as standard and classic.
        /// </summary>
        static DataGridViewHeaderBorderStyle ProperColumnHeadersBorderStyle
        {
            get
            {
                return (SystemFonts.MessageBoxFont.Name == "Segoe UI") ?
                    DataGridViewHeaderBorderStyle.None :
                    DataGridViewHeaderBorderStyle.Raised;
            }
        }

        /// <summary>
        /// Populate the Table when Series is Checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SeriesSelector_SeriesCheck(object sender, SeriesEventArgs e)
        {
            ShowAllFieldsinSequence();

            _seriesCheckState = e.IsChecked;
        }

        void _seriesSelector_Refreshed(object sender, EventArgs e)
        {
            ShowAllFieldsinSequence();

            //bindingSource1.DataSource = _dataValuesTable;
            //dataViewSeries.DataSource = bindingSource1;
        }

        private void rbSequence_Click(object sender, EventArgs e)
        {
            //dataViewSeries.DataSource = null;
           // dataViewSeries.Columns.Clear();
            ShowAllFieldsinSequence();
            sequenceSwitch = true;
        }

        private void rbParallel_Click(object sender, EventArgs e)
        {
            dataViewSeries.DataSource = null;
            dataViewSeries.Columns.Clear();
            ShowJustValuesinParallel();
        }

        #endregion

    }
}
