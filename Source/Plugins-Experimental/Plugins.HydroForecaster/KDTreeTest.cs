using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWindow.Tools;
using MapWindow.Tools.Param;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
namespace HydroForecaster
{
    class KDTreeTest:ITool
    {
      
        private void Test()
        {
            for (int i = 1; i < 100; i++)
            {
               
            }
        }       

        #region Private members

        private Parameter[] _inputParameters;
        private Parameter[] _outputParameters;
        private DataTable tblDataTable;
        private string _workingPath;

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

        private double KDTreeTestForecastingValue(List<double> QPreviousT, List<double> QT, int KValue)
        {
            MapWindow.Analysis.Topology.KDTree.KDTree KD = new MapWindow.Analysis.Topology.KDTree.KDTree(2);

            double[] Corordinates = new double[2];

            for (int i = 1; i < QT.Count; i++)
            {
                //X Coordinate  for Previous Day records [t-1]
                Corordinates[0] = QPreviousT[i];
                //Y Coordinate for Current Day records [t]
                Corordinates[1] = QT[i];

                object Val = (object)(Corordinates);

                KD.Insert(Corordinates, Val);
            }

            //This is guess X coordinate value,which means the one day before the forecasting day value
            double LastdayValue = QT[QT.Count - 1];

            double PreviousDay = QPreviousT[QPreviousT.Count - 1];

            //This is guess Y coordinate value,which means the forecasting day value
            double GuessValue = LastdayValue +((LastdayValue - PreviousDay)/2);

            double[] Finalcheck = new double[2];
            //X Coordinate 
            Finalcheck[0] = LastdayValue;

            //Y Coordinate
            Finalcheck[1] = GuessValue;

            object[] val = KD.Nearest(Finalcheck, KValue);


            double XDistance;
            double XSqrDistance;
            double YDistance;
            double YSqrDistance;

            List<double> finalDistnace = new List<double>();
            List<double> finalWeights = new List<double>();
            double weight=0;
            double distance=0;

            for (int j = 0; j < val.Length; j++)
            {
                double[] val2 = (double[])(val[j]);

                XDistance = LastdayValue - val2[0];
                XSqrDistance = XDistance * XDistance;
                YDistance = GuessValue - val2[1];
                finalWeights.Add(val2[1]);
                YSqrDistance = YDistance * YDistance;
                finalDistnace.Add(Math.Sqrt(XSqrDistance + YSqrDistance));
            }

          

            for (int k = 0; k < finalDistnace.Count; k++)
            {
                if (finalDistnace[k] == 0)
                {
                    weight += finalWeights[k] / 1.0;
                    distance += 1 / 1.0;
                }
                else
                {
                    weight += finalWeights[k] / finalDistnace[k];
                    distance += 1 / finalDistnace[k];
                }
            }

            double forecastingValue = weight / distance;

          //  KD.Delete(Corordinates);

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
            get { return "KDTreeTest Model"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "KDTreeTest Model Tool"; }
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

            List<double> QPreviousT = new List<double>();

            List<DateTime> Date = new List<DateTime>();

            int counter = 0;

            int K = 0;

            K = Convert.ToInt32(_inputParameters[1].Value.ToString());

            for (int j = 0; j < tblDataTable.Rows.Count; j++)
            {
                try
                {
                    //This array helps to store the Q of the t day
                    QT.Add(Convert.ToDouble(tblDataTable.Rows[j][1]));

                    //This array is used to store the value of the t-1 day
                    QPreviousT.Add(Convert.ToDouble(tblDataTable.Rows[j][2]));

                    Date.Add(Convert.ToDateTime(tblDataTable.Rows[j][0]));  
                    //cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(tblDataTable.Rows.Count)) * 100), tblDataTable.Rows[j][1].ToString());
                    //if (cancelProgressHandler.Cancel)
                    //    return false;
                }

                catch
                {

                }

            }
            double ForecastingValue = 0D;


         

            DataTable dt1 = new DataTable();

            dt1.Columns.Add("Forecasting Values");

            dt1.Columns.Add("Date");

            int numberofrecords= tblDataTable.Rows.Count;

            DateTime ForecastingDate;

            System.TimeSpan diffResult = Date[numberofrecords -1].Subtract(Convert.ToDateTime(_inputParameters[2].Value.ToString()));

            int numberofdays = diffResult.Days * -1;

            for (int h = 0; h < numberofdays; h++)
            {
                DataRow dr = dt1.NewRow();

                if (ForecastingValue == 0D)
                {
                    ForecastingValue = KDTreeTestForecastingValue(QPreviousT, QT, K);
                    QT.Add(ForecastingValue);
                    QPreviousT.Add(QT[QT.Count - 2]);
                    ForecastingDate = Date[numberofrecords - 1].AddDays(h);
                    Date.Add(ForecastingDate);
                    dr[0] = Convert.ToDouble(ForecastingValue);
                    dr[1] = Date[numberofrecords - 1];
                    dt1.Rows.Add(dr);

                }
                else
                {

                    ForecastingDate = Date[numberofrecords - 1].AddDays(h);
                    ForecastingValue = KDTreeTestForecastingValue(QPreviousT, QT, K);
                    QT.Add(ForecastingValue);
                    QPreviousT.Add(QT[QT.Count - 2]);
                    Date.Add(ForecastingDate);
                    dr[0] = Convert.ToDouble(ForecastingValue);
                    dr[1] = ForecastingDate;
                    dt1.Rows.Add(dr);

                }
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

        }

        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "KDTreeTest Tool Help"; }
        }

        public string HelpURL
        {
            get { return "KDTreeTest Model"; }
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
            get { return "KD Tree Test Model"; }
        }


        void ITool.ParameterChanged(Parameter sender)
        {
            return;
        }

        public string ToolTip
        {
            get { return "KDTreeTest Model"; }
        }

        public string UniqueName
        {
            get { return "KDTreeTest Tool"; }
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
