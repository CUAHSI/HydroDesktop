//********************************************************************************************************
// Product Name: MapWindowTools.Ar(1)Model
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

namespace MapWindowTools.Analysis
{
    class AR_Test : ITool
    {
        #region Private members

        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private DataTable tblDataTable;

        double forecastingValue;
        private string _workingPath;

        private double SumX;
        private double SumY;
        private double SumSqrX;
        private double SumXY;

        #endregion

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

            for (int i = 7; i < ra.Length; i++)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ForecastingValuefunction(List<double> T,List<double> PreviousT,double value)
        {
            double[] Xd = new double[T.Count];

            double[] Yd = new double[PreviousT.Count];

            for (int k = 0; k < T.Count ; k++)
            {
                try
                {
                    //The value of Tday
                    Xd[k] = Convert.ToDouble(T[k]);

                    //The Value of T-1 Day
                    Yd[k] = Convert.ToDouble(PreviousT[k]);

                    SumX += Xd[k];

                    SumY += Yd[k];

                    SumXY += Xd[k] * Yd[k];

                    SumSqrX += Xd[k] * Xd[k];
                }
                catch
                {

                }


            }

            //Calculate m value
            double mstep1 = Convert.ToDouble(T.Count ) * SumXY;

            double mstep2 = SumX * SumY;

            double mstep3 = Convert.ToDouble(T.Count) * SumSqrX;

            double mstep4 = SumX * SumX;

            double mstep5 = mstep1 - mstep2;

            double mstep6 = mstep3 - mstep4;

            double m = mstep5 / mstep6;

            //Calculate c value

            double cstep1 = Convert.ToDouble(1.0 / T.Count);

            double cstep2 = cstep1 * SumY;

            double cstep3 = cstep1 * SumX;

            double cstep4 = m * cstep3;

            double cstep5 = cstep2 - cstep4;

            double c = cstep5;

            //equation is y=mx+c           
            //Y is Currect value
            //X is Forecasting Value
            //X= ( Y - C ) / m

            double ForecastingResult = (Convert.ToDouble(value) - c) / m;

            //    decimal forecastingvalue = m * Convert.ToDecimal(value) + c;

            return ForecastingResult;

        }

        #endregion

        #region ITool Members

        /// <summary>
        /// Gets or Sets the input paramater array
        /// </summary>
        Parameter[] ITool.InputParameters
        {
            get { return (_inputParameters); }
        }

        /// <summary>
        /// Gets or Sets the output paramater array
        /// </summary>
        Parameter[] ITool.OutputParameters
        {
            get { return (_outputParameters); }
        }


        public string Author
        {
            get { return "AR(1) Model"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "AR(1) Model Tool"; }
        }

        bool ITool.Execute(ICancelProgressHandler cancelProgressHandler)
        {
            string fileName = _inputParameters[0].Value.ToString();
            string destinationfileName = _outputParameters[0].Value.ToString();
            if (Execute(fileName, destinationfileName, cancelProgressHandler) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Execute(string CSVFilePath, string CSVDestination, ICancelProgressHandler cancelProgressHandler)
        {
            tblDataTable = ParseCSVFile(CSVFilePath);
            int numRows = tblDataTable.Rows.Count;

            DateTime lastSourceDate = Convert.ToDateTime(tblDataTable.Rows[numRows - 1][0]);

            DateTime forecastingfinalDate = Convert.ToDateTime(_inputParameters[1].Value);

            System.TimeSpan numberofDays = forecastingfinalDate.Subtract(lastSourceDate);

            int numberofdays = numberofDays.Days;

            double[] ForecastingValue = new double[numberofdays];

            List<DateTime> Date = new List<DateTime>();
          
            List<double> T = new List<double>();

            List<double> PreviousT = new List<double>();          

            for (int i = 2; i < numRows; i++)
            {
                T.Add(Convert.ToDouble(tblDataTable.Rows[i][1]));
                PreviousT.Add(Convert.ToDouble(tblDataTable.Rows[i][2]));
            }

            double lastRecordValue = Convert.ToDouble(tblDataTable.Rows[numRows-2][1]);

            for (int j = 0; j < numberofdays; j++)
            {
                try
                {                               
                    if (forecastingValue == 0)
                    {

                        forecastingValue = ForecastingValuefunction(T, PreviousT, lastRecordValue);
                        PreviousT.Add(lastRecordValue);
                        T.Add(forecastingValue);
                        ForecastingValue[j] = forecastingValue;
                        Date.Add(lastSourceDate.AddDays(1));
                    }
                    else
                    {
                        double dt = Convert.ToDouble(ForecastingValue[j - 1]);

                        forecastingValue = ForecastingValuefunction(T, PreviousT, dt);

                        PreviousT.Add(dt);

                        T.Add(forecastingValue);

                        ForecastingValue[j] = forecastingValue;

                        Date.Add(lastSourceDate.AddDays(j));
                       
                    }

                    cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(ForecastingValue[j])) * 100), ForecastingValue[j].ToString());
                    if (cancelProgressHandler.Cancel)
                       return false;
                }
                catch
                {

                }

            }

            DataTable dt1 = new DataTable();
       
            dt1.Columns.Add("Forecasting Values");

            dt1.Columns.Add("Date");

            for (int g = 0; g < ForecastingValue.Length; g++)
            {
                DataRow dr = dt1.NewRow();
             
                dr[0] = Convert.ToDouble(ForecastingValue[g]);      

                dr[1] = Date[g];

                dt1.Rows.Add(dr);

            }

            if (dt1.Rows.Count > 0)
            {
                DataTable2CSV(dt1, CSVDestination, ",");

                for (int j = 0; j < dt1.Rows.Count; j++)
                {                   
                    cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(dt1.Rows.Count)) * 100), dt1.Rows[j][0].ToString());
                    if (cancelProgressHandler.Cancel)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
            //decimal testValue = ForecastingValue(tblDataTable, 2.250420002);


        }

        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "AR(1) Tool Help"; }
        }

        public string HelpUrl
        {
            get { return "AR(1) Model"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return (null); }
        }

        void ITool.Initialize()
        {
            _inputParameters = new Parameter[2];

            _inputParameters[0] = new FileParam("Select the lag table");
            _inputParameters[1] = new DateTimeParam("Please select the forecaasting final date");


            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Forecasting Values");
        }


        public string Name
        {
            get { return "AR(1) Model"; }
        }


        void ITool.ParameterChanged(Parameter sender)
        {
            return;
        }

        public string ToolTip
        {
            get { return "AR(1) Model"; }
        }

        public string UniqueName
        {
            get { return "AR(1) Model"; }
        }

        public Version Version
        {
            get { return (new Version(1, 0, 0, 0)); }
        }

        string ITool.WorkingPath
        {
            set { _workingPath = value; }
        }

        #endregion

    }
}
