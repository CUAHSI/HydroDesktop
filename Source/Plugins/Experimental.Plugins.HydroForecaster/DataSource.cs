//********************************************************************************************************
// Product Name: MapWindowTools.DataSource
// Description:  This tool is used to get the data from SQLITE database.
//
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is Toolbox.dll for the MapWindow 4.6/6 ToolManager project
//
// The Initializeializeial Developer of this Original Code is Teva Veluppillai. Created in 2010 March.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
// 
//********************************************************************************************************

#region NameSpace
using System;
using System.Spatial.Geometries;
using System.Spatial.Tools;
using System.Spatial.Data;
using System.Spatial.Tools.Param;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
#endregion

namespace MapWindowTools.Forecaster
{
    class DataSource : ITool
    {
        #region Class Variable
        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private DataTable Sites;
        private DataTable Variables;
        private string _workingPath;
        public string[] SiteId;
        public string[] VariableId;
        public string VarId;
        #endregion

        #region ITool Members

        //New comment added by Teva to test hg commit
        public string Author
        {
            get { return "MapWindow Development Team"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "This tool helps to get the data from Hydrodesktop database.User needs to select the hydrodesktop database,thereafter user can select the required site and variable to get the data. Date range also helps to control the range of download data. Data store in CSV file format."; }
        }

        public bool Execute(ICancelProgressHandler CancelProgressHandler)
        {
            string fileName = _outputParameters[0].Value.ToString();

            if (Execute(fileName, CancelProgressHandler) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Execute(string fileName, ICancelProgressHandler cancelProgressHandler)
        {
            string SeriesId = GetSeriesId();
            DataTable dataValues = GetDataValues(SeriesId);
            DateTime startDate = Convert.ToDateTime(_inputParameters[3].Value);
            DateTime endDate = Convert.ToDateTime(_inputParameters[4].Value);

            DataTable UpdatedDatatable = new DataTable();
            UpdatedDatatable.Columns.Add("DateTime");
            UpdatedDatatable.Columns.Add("Value");

            DataRow siteId = UpdatedDatatable.NewRow();
            siteId[0] = SiteId[0].ToString();
            siteId[1] = VarId; //Note this is variable Code
            UpdatedDatatable.Rows.Add(siteId);

            for (int k = 0; k < dataValues.Rows.Count; k++)
            {
               
                if (startDate < Convert.ToDateTime(dataValues.Rows[k][0].ToString()) && endDate > Convert.ToDateTime(dataValues.Rows[k][0].ToString()))
                {
                    DataRow dr = UpdatedDatatable.NewRow();
                    dr[0] = dataValues.Rows[k][0].ToString();
                    dr[1] = dataValues.Rows[k][1].ToString();
                    UpdatedDatatable.Rows.Add(dr);
                }
            }

            if (UpdatedDatatable.Rows.Count > 1)
            {
                DataTable2CSV(UpdatedDatatable, fileName, ",");
                
                for (int j = 1; j < dataValues.Rows.Count; j++)
                {
                    
                    cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(dataValues.Rows.Count)) * 100), dataValues.Rows[j][0].ToString());
                    if (cancelProgressHandler.Cancel)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "This tool helps to get the data from Hydrodesktop database.User needs to select the hydrodesktop database,thereafter user can select the required site and variable to get the data. Date range also helps to control the range of download data. Data store in CSV file format."; }
        }

        public string HelpUrl
        {
            get { return "www.System.Spatial.org"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        public void Initialize()
        {
            _inputParameters = new Parameter[5];
            _inputParameters[0] = new FileParam("Select the Hydrodesktop database");
            _inputParameters[1] = new ListParam("Select the Site");
            _inputParameters[2] = new ListParam("Select the Variable");
            _inputParameters[3] = new DateTimeParam("Start Date");
            _inputParameters[4] = new DateTimeParam("End Date"); 
    
          
            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Save Data");
        }

        public Parameter[] InputParameters
        {
            get { return _inputParameters; }
        }

        public string Name
        {
            get { return ("Data Source"); }
        }

        public Parameter[] OutputParameters
        {
            get { return _outputParameters; }
        }

        public void ParameterChanged(Parameter sender)
        {
            return;
        }

        public string ToolTip
        {
            get { return ("DataSource from Hydrodesktop Tool"); }
        }

        public string UniqueName
        {
            get { return ("DataSource"); }
        }

        public Version Version
        {
            get { throw new NotImplementedException(); }
        }

        public string WorkingPath
        {
            set { _workingPath = value; }
        }

        void ITool.ParameterChanged(Parameter sender)
        {
            if (sender == _inputParameters[0])
            {
                if (BuildConnection() == true)
                {
                    //This is used to get the sites
                    Sites = Getsites();
                    //This is used to get the variables
                    Variables = GetVariables();

                    ListParam lp1 = (_inputParameters[1] as ListParam);

                    ListParam lp2 = (_inputParameters[2] as ListParam);

                    if (lp1 != null)
                    {
                        lp1.ValueList.Clear();
                        for (int i = 0; i < Sites.Rows.Count; i++)
                        {
                            lp1.ValueList.Add(Sites.Rows[i][0].ToString() + "," + Sites.Rows[i][2].ToString());
                        }
                        lp1.Value = -1;
                    }

                    if (lp2 != null)
                    {
                        lp2.ValueList.Clear();
                        for (int i = 0; i < Variables.Rows.Count; i++)
                        {
                            lp2.ValueList.Add(Variables.Rows[i][0].ToString() + "," + Variables.Rows[i][2].ToString() + "/" + "ValueType" + Variables.Rows[i][6].ToString());
                        }
                        lp2.Value = -1;
                    }


                }
            }
            return;
        }


        #endregion

        #region "Private methods"

        private bool BuildConnection()
        {

            if (_inputParameters[0].Value.ToString() != string.Empty)
            {
                string connectionString = _inputParameters[0].Value.ToString();
                SetSQLITEConnection(connectionString);
                return true;
            }
            else
            {
                return false;
            }

        }


        private DataTable Getsites()
        {
            string Query = "Select * from Sites";
            DataTable Sites = new DataTable();
            if (BuildConnection() == true)
            {
                Sites = ExecuteDataset(Query).Tables[0];
            }
            return Sites;
        }

        private DataTable GetVariables()
        {
            string Query = "Select * from Variables";
            DataTable Variables = new DataTable();
            if (BuildConnection() == true)
            {
                Variables = ExecuteDataset(Query).Tables[0];
            }
            return Variables;
        }

        private string GetSeriesId()
        {
            ListParam lp1 = _inputParameters[1] as ListParam;
            ListParam lp2 = _inputParameters[2] as ListParam;
            string SId = (lp1).ValueList[lp1.Value];
            string VId = (lp2).ValueList[lp2.Value];
            string deli = ",";
            char[] delimiter = deli.ToCharArray();
            //string[] VariableId = null;

            //string[] SiteId

                SiteId = SId.Split(new Char[] { ' ', ',', '.', ':', '\t' });

            //string[]  VariableId

                VariableId = VId.Split(new Char[] { ' ', ',', '.', ':', '\t' });

                VarId = VariableId[0].ToString();

            string Query = "Select min(SeriesId) from DataSeries where siteId=" + SiteId[0].ToString() + " and variableId=" + VariableId[0].ToString() + " group by siteid,variableId";
            string SeriesId = ExecuteSQLITESingleOutput(Query);
            return SeriesId;
        }


        private DataTable GetDataValues(string SeriesId)
        {
            DataTable DataValues = new DataTable();

            if (SeriesId != string.Empty)
            {
                string Query = "select LocalDateTime,DataValue from DataValues where SeriesID=" + SeriesId;
                if (BuildConnection() == true)
                {
                    DataValues = ExecuteDataset(Query).Tables[0];
                }
            }

            return DataValues;
        }
        #region SQLITE Related

        SQLiteConnection sqlite_con;
        SQLiteDataAdapter sqliteDA;
        System.Data.DataSet sqliteDS;
        SQLiteCommand sqlite_cmd;

        public string ExecuteSQLITESingleOutput(String inputString)
        {
            string output;
            try
            {
                if (sqlite_con == null)
                {
                    SetSQLITECon((this.InputParameters[0] as FileParam).Value.FileName);
                }
                
                if (sqlite_con.State == ConnectionState.Closed)
                {
                    sqlite_con.Open();
                }
                sqlite_cmd = sqlite_con.CreateCommand();
                sqlite_cmd.CommandText = inputString;
                if (sqlite_cmd.ExecuteScalar().ToString() == "")
                {
                    output = "";
                }
                else
                {
                    output = sqlite_cmd.ExecuteScalar().ToString();
                    sqlite_con.Close();
                }
            }
            catch
            {
                output = "";
            }

            return output;

        }

        public void SetSQLITEConnection(String ConnectionString)
        {
            string setSQLITEConnection = SetSQLITECon(ConnectionString);
            sqlite_con = new System.Data.SQLite.SQLiteConnection(setSQLITEConnection);
        }

        public string SetSQLITECon(String Databasepath)
        {
            sqlite_con = new System.Data.SQLite.SQLiteConnection("Data Source=" + Databasepath + ";New=False;Compress=True;Version=3");
            return sqlite_con.ConnectionString.ToString();
        }

        public System.Data.DataSet ExecuteDataset(string SQlQuery)
        {

            sqlite_con.Open();
            string CommandText = SQlQuery;
            sqliteDA = new SQLiteDataAdapter(CommandText, sqlite_con);
            sqliteDS = new System.Data.DataSet();
            sqliteDS.Clear();
            sqliteDA.Fill(sqliteDS);
            return sqliteDS;
        }


        #endregion

        public static void DataTable2CSV(DataTable table, string filename, string seperateChar)
        {
            StreamWriter sr = null;
            try
            {

                sr = new StreamWriter(filename);
                string seperator = "";
                StringBuilder builder = new StringBuilder();
                foreach (DataColumn col in table.Columns)
                {

                    builder.Append(seperator).Append(col.ColumnName);

                    seperator = seperateChar;
                }

                sr.WriteLine(builder.ToString());

                foreach (DataRow row in table.Rows)
                {

                    seperator = "";
                    builder = new StringBuilder();
                    foreach (DataColumn col in table.Columns)
                    {

                        builder.Append(seperator).Append(row[col.ColumnName]);
                        seperator = seperateChar;

                    }

                    sr.WriteLine(builder.ToString());

                }

            }

            finally
            {

                if (sr != null)
                {

                    sr.Close();

                }

            }

        }

        #endregion
    }
}
