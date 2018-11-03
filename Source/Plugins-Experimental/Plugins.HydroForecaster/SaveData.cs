using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database;
using System.Data;
using System.Data.SQLite;
using System.Spatial.Tools.Param;
using System.Spatial.Tools;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace HydroForecaster
{
    class SaveData:ITool
    {
        #region "Private methods"

        #region SQLITE Related

      private  SQLiteConnection sqlite_con;

       public string SetSQLITECon(String Databasepath)
        {
            sqlite_con = new System.Data.SQLite.SQLiteConnection("Data Source=" + Databasepath + ";New=False;Compress=True;Version=3");
            return sqlite_con.ConnectionString.ToString();
        }

        #endregion


        //This method is used to store the data to SQLITE Database
        private bool StoreData(string siteId, string VariableId, string MethodName, DataTable ForecastingValue,string Connectionstring)
        {
            DataTable dtInput=new DataTable();
            dtInput.Columns.Add("DateTime");
            dtInput.Columns.Add("DataValue");
            for (int i = 1; i < ForecastingValue.Rows.Count; i++)
            {
                DataRow dr=dtInput.NewRow();
                dr[0]=Convert.ToDateTime(ForecastingValue.Rows[i][0]);
                dr[1]=Convert.ToDouble(ForecastingValue.Rows[i][1]);  
                 dtInput.Rows.Add(dr);
            }

            RepositoryManagerSQL manager = new RepositoryManagerSQL(DatabaseTypes.SQLite, Connectionstring);
            manager.SaveSeries(Convert.ToInt32(siteId), Convert.ToInt32(VariableId), MethodName, "ForecastingResult", dtInput);
                        
            return true;

        }

        #region CSV File Related

        private DataTable ParseCSV(string inputString)
        {

            DataTable dt = new DataTable();

            // declare the Regular Expression that will match versus the input string
            Regex re = new Regex("((?<field>[^\",\\r\\n]+)|\"(?<field>([^\"]|\"\")+)\")(,|(?<rowbreak>\\r\\n|\\n|$))");

            ArrayList colArray = new ArrayList();
            ArrayList rowArray = new ArrayList();

            int colCount = 0;
            int maxColCount = 0;
            string rowbreak = "";
            string field = "";

            MatchCollection mc = re.Matches(inputString);

            foreach (Match m in mc)
            {

                // retrieve the field and replace two double-quotes with a single double-quote
                field = m.Result("${field}").Replace("\"\"", "\"");

                rowbreak = m.Result("${rowbreak}");

                if (field.Length > 0)
                {
                    colArray.Add(field);
                    colCount++;
                }

                if (rowbreak.Length > 0)
                {

                    // add the column array to the row Array List
                    rowArray.Add(colArray.ToArray());

                    // create a new Array List to hold the field values
                    colArray = new ArrayList();

                    if (colCount > maxColCount)
                        maxColCount = colCount;

                    colCount = 0;
                }
            }

            if (rowbreak.Length == 0)
            {
                // this is executed when the last line doesn't
                // end with a line break
                rowArray.Add(colArray.ToArray());
                if (colCount > maxColCount)
                    maxColCount = colCount;
            }

            // convert the row Array List into an Array object for easier access
            Array ra = rowArray.ToArray();
            Array ss;

            for (int t = 0; t < 1; t++)
            {
                // convert the column Array List into an Array object for easier access
                ss = (Array)(ra.GetValue(t));


                // create the columns for the table
                for (int i = 0; i < ss.Length; i++)
                    dt.Columns.Add(ss.GetValue(i).ToString());
            }

            for (int i = 1; i < ra.Length; i++)
            {
                // create a new DataRow
                DataRow dr = dt.NewRow();

                // convert the column Array List into an Array object for easier access
                Array ca = (Array)(ra.GetValue(i));

                // add each field into the new DataRow
                for (int j = 0; j < ca.Length; j++)
                    dr[j] = ca.GetValue(j);

                // add the new DataRow to the DataTable
                dt.Rows.Add(dr);
            }

            // in case no data was parsed, create a single column
            if (dt.Columns.Count == 0)
                dt.Columns.Add("NoData");

            return dt;
        }

        private DataTable ParseCSVFile(string path)
        {

            string inputString = "";

            // check that the file exists before opening it
            if (File.Exists(path))
            {

                StreamReader sr = new StreamReader(path);
                inputString = sr.ReadToEnd();
                sr.Close();

            }

            return ParseCSV(inputString);
        }

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

        #endregion

        #region Class Variable
        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private string _workingPath;
        #endregion

        #region ITool Members

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
            get { return "Save the Forecasting Records in to Hydrodesktop value"; }
        }

        public bool Execute(ICancelProgressHandler CancelProgressHandler)
        {
            string connectiondatabase = _inputParameters[0].Value.ToString();
            string forecastingResult = _inputParameters[1].Value.ToString();

            if (Execute(connectiondatabase,forecastingResult, CancelProgressHandler) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Execute(string connectiondatabase,string forecastingResult, ICancelProgressHandler cancelProgressHandler)
        {
            DataTable dtForecatingResult = new DataTable();
            dtForecatingResult = ParseCSVFile(forecastingResult);
           string siteId=Convert.ToString(dtForecatingResult.Rows[0][0]);
           string  VariableId = Convert.ToString(dtForecatingResult.Rows[0][1]);
           string connectionstring = SetSQLITECon(connectiondatabase);
           if (StoreData(siteId, VariableId, "NeuralNetworkForecasting@ISU", dtForecatingResult, connectionstring) == true)
            {
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
            get { return "This tool helps to store the forecasting results to Hyderdesktop's SQLITE database"; }
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
            _inputParameters = new Parameter[2];
            _inputParameters[0] = new FileParam("Select the Hydrodesktop SQLITE database");
            _inputParameters[1] = new FileParam("Select Final Forecasting Results");
           
            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Final Forecasting Result");
        }

        public Parameter[] InputParameters
        {
            get { return _inputParameters; }
        }

        public string Name
        {
            get { return ("Save forecasting data"); }
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
            get { return ("Save forecasting result in to Hydrodesktop database"); }
        }

        public string UniqueName
        {
            get { return ("Save forecasting Data"); }
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
            //if (sender == _inputParameters[0])
            //{
            //    if (BuildConnection() == true)
            //    {

            //     }
            //}
            return;
        }


        #endregion

    
    }
}
