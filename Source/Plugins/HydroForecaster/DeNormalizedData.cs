//********************************************************************************************************
// Product Name: MapWindowTools.LagTool
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Spatial.Tools;
using System.Spatial.Tools.Param;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;

namespace HydroForecaster
{
    class DeNormalizedData:ITool 
    {
        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;

        #region Private Methods

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

        private void DataTable2CSV(DataTable table, string filename, string seperateChar)
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

        public List<double> GetMinMaxValue(DataTable dt1, ICancelProgressHandler cancelProgressHandler)
        {
            int counter = 0;

            int totalrows = dt1.Rows.Count;

            while (counter < dt1.Rows.Count)
            {
                double temp;
                if (counter != totalrows - 1)
                {
                    if (Convert.ToDouble(dt1.Rows[counter][1]) > Convert.ToDouble(dt1.Rows[counter + 1][1]))
                    {
                        temp = Convert.ToDouble(dt1.Rows[counter][1]);
                        dt1.Rows[counter][1] = dt1.Rows[counter + 1][1];
                        dt1.Rows[counter + 1][1] = temp;
                        counter = 0;
                        continue;
                    }
                }
                counter++;                
                //cancelProgressHandler.Progress("",counter /(counter -1)*(counter -1) * 100,dt1.Rows[counter][1].ToString());
               
                
            }

            double Min = Convert.ToDouble(dt1.Rows[1][1]);
            double Max = Convert.ToDouble(dt1.Rows[dt1.Rows.Count - 1][1]);

            List<double> MinMax = new List<double>();
            MinMax.Add(Min);
            MinMax.Add(Max);

            return MinMax;

        }

        public DataTable DenormalizedData(DataTable dt, List<double> MinMax)
        {
            DataTable dtDenormalizedTable = new DataTable();

            dtDenormalizedTable.Columns.Add("DateTime");
            dtDenormalizedTable.Columns.Add("ForecastingValue");

            DataRow SiteId = dtDenormalizedTable.NewRow();
            SiteId[0] = (Convert.ToDouble(dt.Rows[dt.Rows.Count-1][0])); //this is siteId
            SiteId[1] = (Convert.ToDouble(dt.Rows[dt.Rows.Count-1][1])); //this VariableId
            dtDenormalizedTable.Rows.Add(SiteId);

            //I just change the i value from 7 to 1.

            double MinimumValue = MinMax[0];
            double MaximumValue = MinMax[1];
            double Denominator = MaximumValue - MinimumValue;

            for (int i = 0; i < dt.Rows.Count-2; i++)
            {
                DataRow dr = dtDenormalizedTable.NewRow();
                dr[0] = dt.Rows[i][0];
                double row1 = Convert.ToDouble(dt.Rows[i][1]);
                double finalValue = (row1 * Denominator) + MinimumValue;
                dr[1] = finalValue;
                    //(((Convert.ToDouble(dt.Rows[i][1])) * (MinMax[1] - MinMax[0]))) + MinMax[0];
                //dr[1] = ((Convert.ToDouble(dt.Rows[i][1])) + MinMax[0]) * (MinMax[1] - MinMax[0]);
                dtDenormalizedTable.Rows.Add(dr);
            }

            return dtDenormalizedTable;
        }

        #endregion

        #region ITool Members

        public string Author
        {
            get { return "Neural Network Denormalized Data"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "Forecasting Neural Network Denormalized Data"; }
        }

        public bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            string streamFlowSource = _inputParameters[0].Value.ToString();
            string forecastingSource = _inputParameters[1].Value.ToString();

            string destinationfileName = _outputParameters[0].Value.ToString();
            Execute(forecastingSource, destinationfileName, streamFlowSource,cancelProgressHandler);
            return true;
        }

        public bool Execute(string ForecastingResults, string ResultsSave,string StreamFlowSource, ICancelProgressHandler cancelProgressHandler)
        {
           DataTable tblForecastingResult= new DataTable();
           DataTable tblStreamFlowSource = new DataTable();

            tblForecastingResult = ParseCSVFile(ForecastingResults);
            tblStreamFlowSource = ParseCSVFile(StreamFlowSource);

            List<double> MinMax=new List<double>();
            MinMax = GetMinMaxValue(tblStreamFlowSource, cancelProgressHandler);

            DataTable dtDeNormalizedValue = new DataTable();

            dtDeNormalizedValue = DenormalizedData(tblForecastingResult, MinMax);

            DataTable2CSV(dtDeNormalizedValue, ResultsSave, ",");

            for (int j = 0; j < dtDeNormalizedValue.Rows.Count; j++)
            {
                cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(dtDeNormalizedValue.Rows.Count)) * 100), dtDeNormalizedValue.Rows[j][0].ToString() + ":" + dtDeNormalizedValue.Rows[j][1].ToString());
                if (cancelProgressHandler.Cancel)
                    return false;
            }

             return true;
        }

        public DataTable LagTable(DataTable dt)
        {
            DataTable dtLagTable = new DataTable();

            dtLagTable.Columns.Add("Date");
            dtLagTable.Columns.Add("t");
            dtLagTable.Columns.Add("t-1");
       
            for (int l = 1; l < dt.Rows.Count; l++)
            {
                DataRow dr = dtLagTable.NewRow();

                dr[0] = dt.Rows[l][0];
            
                dr[1] = (Convert.ToDouble(dt.Rows[l][1]));
                dr[2] = (Convert.ToDouble(dt.Rows[l - 1][1]));

               dtLagTable.Rows.Add(dr);
            }

            return dtLagTable;
        }




        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "This tool helps to denormalize the data"; }
        }

        public string HelpUrl
        {
            get { return "Denormalized Data"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return null; }
        }

        public void Initialize()
        {
            _inputParameters = new Parameter[2];

            _inputParameters[0] = new FileParam("Select the Streamflow source table");

            _inputParameters[1] = new FileParam("Select the Forecasting Results table");

            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Denormalized Data");
        }

        public Parameter[] InputParameters
        {
            get { return _inputParameters; }
        }

        public string Name
        {
            get { return "Data Denormalization"; }
        }

        public Parameter[] OutputParameters
        {
            get { return _outputParameters; }
        }

        public void ParameterChanged(Parameter sender)
        {

        }

        public string ToolTip
        {
            get { return "Neural Network Denormalized Data"; }
        }

        public string UniqueName
        {
            get { return "Neural Network Denormalized Data"; }
        }

        public Version Version
        {
            get { return (new Version(1, 0, 0, 0)); }
        }

        public string WorkingPath
        {
            set { }
        }

        #endregion
    }
}
