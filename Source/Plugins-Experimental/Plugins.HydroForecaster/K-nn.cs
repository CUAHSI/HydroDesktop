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
    class K_nn : ITool
    {
        #region Private members

        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private DataTable tblDataTable;

        decimal forecastingValue;
        private string _workingPath;

        private Decimal SumX;
        private Decimal SumY;
        private Decimal SumSqrX;
        private Decimal SumXY;

        #endregion

        #region Private Methods

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

        #endregion

        /// <summary>
        /// This function gives the nearest neighbour value
        /// </summary>
        /// <param name="QT">QT has all the values from history data</param>
        /// <param name="KValue">User enterd neighbour value</param>
        /// <returns>neighbour value</returns>
        private List<double> NeighbourValue(List<double> QT,int KValue)
        {
            
            List<double> difference = new List<double>();

            List<double> neighbourValue = new List<double>();

            double LastValue;                     
          
            LastValue = QT[QT.Count - 1];

            //find the difference between actual value and last date value
            for (int j = 0; j < QT.Count; j++)
            {
                difference.Add(QT[j] - LastValue);
            }

            //sort the difference list array
            int counter = 0;
            double temp1;
            while (counter < difference.Count - 1)
            {

                if (difference[counter] > difference[counter + 1])
                {
                    temp1 = difference[counter];
                    difference[counter] = difference[counter + 1];
                    difference[counter + 1] = temp1;
                    counter = 0;
                    continue;
                }
                counter += 1;
            }

            //find the nearest neighbour values
            for (int t = 0; t < difference.Count; t++)
            {
                neighbourValue.Add(difference[t] + LastValue);
            }

            return neighbourValue;
        }

        /// <summary>
        /// This function gives the neighbour date
        /// </summary>
        /// <param name="neighbourValue">neighbourvalue from the history data Note: Still we haven't got the previous neighbour record</param>
        /// <param name="dt1">Data table</param>
        /// <param name="k">User enterd K value</param>
        /// <returns>Return the Dates which are next dates of the neighbour data value</returns>
        private List<DateTime> GetNeighbourDates(List<double> neighbourValue, DataTable dt1, int k)
        {
            List<DateTime> neighbourValueDate = new List<DateTime>();

            List<DateTime> forecastingDate = new List<DateTime>();

            int KValue = k;

            //get the date from nearest neighbour value dates
            for (int y = 0; y < dt1.Rows.Count; y++)
            {
                for (int j = 0; j < KValue; j++)
                {
                    if (neighbourValue[j] == Convert.ToDouble(dt1.Rows[y][1]))
                    {
                        if (neighbourValueDate.Count == 0)
                        {
                            neighbourValueDate.Add(Convert.ToDateTime(dt1.Rows[y][0]));
                        }
                        else
                        {
                            for (int n = neighbourValueDate.Count - 1; n <= neighbourValueDate.Count; n++)
                            {
                                if (n > neighbourValueDate.Count || n == 0)
                                {
                                    if (Convert.ToDateTime(neighbourValueDate[n]) != Convert.ToDateTime(dt1.Rows[y][0]))
                                    {
                                        neighbourValueDate.Add(Convert.ToDateTime(dt1.Rows[y][0]));
                                    }
                                }

                            }
                        }
                    }
                }
            }

            //add the one dat with nearest neighbour date to get the next day value
            for (int l = 0; l < neighbourValueDate.Count; l++)
            {
                DateTime neighbourvaluedate = neighbourValueDate[l];
                forecastingDate.Add(neighbourvaluedate.AddDays(1));

            }
                return forecastingDate;
        }

        /// <summary>
        /// This function will give the next day value of the neighbour date
        /// </summary>
        /// <param name="dt">Datatable which has the history data</param>
        /// <param name="dt1"></param>
        /// <returns></returns>
        private List<double> GetNextDayValue(DataTable dt, List<DateTime> dt1)
        {
            List<DateTime> forecastingDate = dt1;
            List<double> IDWValue = new List<double>();

            //get the next day value from historic data
            for (int n = 0; n < forecastingDate.Count; n++)
            {
                for (int m = 0; m < dt.Rows.Count; m++)
                {
                    if (Convert.ToDateTime(dt.Rows[m][0]) == forecastingDate[n])
                    {
                        IDWValue.Add(Convert.ToDouble(dt.Rows[m][1]));
                    }
                }
            }
            return IDWValue;
        }

        /// <summary>
        /// Give the interpolated value
        /// </summary>
        /// <param name="neighbourValue"></param>
        /// <param name="FinalValue"></param>
        /// <returns></returns>
       private double IDW1(List<double> neighbourValue, double FinalValue)
        {
            double distance = 0;
            double totalDistance = 0;

            double weight = 0;
            double Value = 0;

            for (int i = 0; i < neighbourValue.Count; i++)
            {
                distance = Math.Abs(neighbourValue[i] - FinalValue);
                weight += neighbourValue[i] / distance;
                totalDistance += 1 / distance;
            }

            Value = weight / totalDistance;
            return Value;
        }

        private double KnnForecastingValue(List<double> QT, int KValue, DataTable dt)
        {
            List<double> getNeighbourValue = new List<double>();

            List<DateTime> getNeighbourDates = new List<DateTime>();

            double forecastingValue = 0D;

            getNeighbourValue = NeighbourValue(QT,KValue);

            getNeighbourDates = GetNeighbourDates(getNeighbourValue, dt, KValue);
                        
            forecastingValue = IDW1(GetNextDayValue(dt, getNeighbourDates), QT[QT.Count - 1]);

            return forecastingValue;
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
            get { return "K-nn Model"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "K-nn Model Tool"; }
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
                      
            List<double> QT = new List<double>();
          
            List<DateTime> Date = new List<DateTime>();

            int counter=0;

            int K = 0;

            K = Convert.ToInt32(_inputParameters[1].Value.ToString());

            for (int j = 1; j < tblDataTable.Rows.Count; j++)
            {
                try
                {
                     //This array helps to store the Q of the t day
                    QT.Add(Convert.ToDouble(tblDataTable.Rows[j][1]));
                   
                                     
                    //This is array is used to store the dates
                    Date.Add(Convert.ToDateTime(tblDataTable.Rows[j][0]));
                    
                    counter +=1;
                    
                    cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(tblDataTable.Rows.Count)) * 100), tblDataTable.Rows[j][1].ToString());
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


            double forecastingValue = 0D;

            DateTime ForecastingDate;

            System.TimeSpan diffResult = Date[counter - 1].Subtract(Convert.ToDateTime(_inputParameters[2].Value.ToString()));

            int numberofdays = diffResult.Days *-1;

            for (int h = 0; h < numberofdays; h++)
            {
                DataRow dr = dt1.NewRow();
                
                if (forecastingValue == 0D)
                {
                    forecastingValue = KnnForecastingValue(QT, K,tblDataTable);
                    QT.Add(forecastingValue);
                    Date.Add(Date[counter - 1]);
                    dr[0] =Convert.ToDouble(forecastingValue);
                    dr[1] = Date[counter - 1];
                    dt1.Rows.Add(dr);
                   
                }
                else
                {
                    
                    ForecastingDate = Date[counter - 1].AddDays(h);
                    forecastingValue = KnnForecastingValue(QT, K,tblDataTable); 
                    QT.Add(forecastingValue);
                    Date.Add(ForecastingDate);
                    dr[0] = Convert.ToDouble(forecastingValue);
                    dr[1] = ForecastingDate;
                    dt1.Rows.Add(dr);
                  
                }
            }



            if (dt1.Rows.Count > 0)
            {
                DataTable2CSV(dt1, CSVDestination, ",");

                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    //DataTable2CSV(dt1, CSVDestination, ",");
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

        }

        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "Knn Tool Help"; }
        }

        public string HelpUrl
        {
            get { return "Knn Model"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return (null); }
        }

        void ITool.Initialize()
        {
            _inputParameters = new Parameter[3];
            _inputParameters[0] = new FileParam("Select the lag table");
            _inputParameters[1] = new StringParam("Enter the neighbour values");
            _inputParameters[2] = new DateTimeParam("Upto which date you need to forecast");
          

            _outputParameters = new Parameter[1];
            _outputParameters[0] = new FileParam("Forecasting Values");
        }


        public string Name
        {
            get { return "K nearest neighbour Model"; }
        }


        void ITool.ParameterChanged(Parameter sender)
        {
            return;
        }

        public string ToolTip
        {
            get { return "Knn Model"; }
        }

        public string UniqueName
        {
            get { return "Knn Tool"; }
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
