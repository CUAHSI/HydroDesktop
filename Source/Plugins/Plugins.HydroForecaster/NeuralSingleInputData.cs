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
    class NeuralSingleInputData:ITool
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

        #endregion

        #region ITool Members

        public string Author
        {
            get { return "Neural Nework Single Input Source Data"; }
        }

        public string Category
        {
            get { return "Forecasting"; }
        }

        public string Description
        {
            get { return "Forecasting Neural Nework Input Source Data"; }
        }

        public bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            string StreamFlow = _inputParameters[0].Value.ToString();

            string destinationfileName = _outputParameters[0].Value.ToString();
            Execute(StreamFlow, destinationfileName, cancelProgressHandler);
            return true;
        }

        public bool Execute(string StreamflowFile, string CSVDestination, ICancelProgressHandler cancelProgressHandler)
        {
         
            DataTable tblStreamFlowTable = new DataTable();

            tblStreamFlowTable = ParseCSVFile(StreamflowFile);

            DataTable InputTable = new DataTable();

            InputTable = LagTable(tblStreamFlowTable);

            DataTable2CSV(InputTable, CSVDestination, " ");

            for (int j = 0; j < InputTable.Rows.Count; j++)
            {
                cancelProgressHandler.Progress("", Convert.ToInt32((Convert.ToDouble(j) / Convert.ToDouble(InputTable.Rows.Count)) * 100), InputTable.Rows[j][0].ToString() + ":" + InputTable.Rows[j][1].ToString());
                if (cancelProgressHandler.Cancel)
                    return false;
            }

            return true;
        }


        public DataTable LagTable(DataTable dtStreamFlow)
        {
            DataTable dtInputSource = new DataTable();

        
            dtInputSource.Columns.Add("StreamFlowT");
            dtInputSource.Columns.Add("StreamFlowT-1");
            dtInputSource.Columns.Add("StreamFlowT-2");
            dtInputSource.Columns.Add("StreamFlowT-3");
            dtInputSource.Columns.Add("StreamFlowT-4");
            dtInputSource.Columns.Add("StreamFlowT-5");
            dtInputSource.Columns.Add("StreamFlowT-6");

            //here I add the number of records from data source
            DataRow drlayers = dtInputSource.NewRow();
            drlayers[0] = Convert.ToInt32(dtStreamFlow.Rows.Count - 1);
            dtInputSource.Rows.Add(drlayers);

            //Here I add siteId and Variable Id
            DataRow SiteId = dtInputSource.NewRow();
            SiteId[0] = (Convert.ToDouble(dtStreamFlow.Rows[0][0]));
            dtInputSource.Rows.Add(SiteId);

            //Here I add siteId and Variable Id
            DataRow VariId = dtInputSource.NewRow();
            VariId[0] = (Convert.ToDouble(dtStreamFlow.Rows[0][1]));
            dtInputSource.Rows.Add(VariId);           

          
            //Here I add first date from the data source
            DataRow drDate = dtInputSource.NewRow();
            drDate[0] = Convert.ToDateTime(dtStreamFlow.Rows[1][0]);
            dtInputSource.Rows.Add(drDate);
                    

            for (int l = 1; l < dtStreamFlow.Rows.Count; l++)
            {
                DataRow dr = dtInputSource.NewRow();

                double SFT1 = (Convert.ToDouble(dtStreamFlow.Rows[l][1]));
                //StreamFlow Records
                dr[0] = SFT1;

                dr[1] = (Convert.ToDouble(dtStreamFlow.Rows[l][2]));

                dr[2] = (Convert.ToDouble(dtStreamFlow.Rows[l][3]));

                dr[3] = Convert.ToDouble(dtStreamFlow.Rows[l][4]);

                dr[4] = (Convert.ToDouble(dtStreamFlow.Rows[l][5]));

                dr[5] = (Convert.ToDouble(dtStreamFlow.Rows[l][6]));

                dr[6] = (Convert.ToDouble(dtStreamFlow.Rows[l][7]));

                dtInputSource.Rows.Add(dr);
            }

            return dtInputSource;
        }




        public System.Drawing.Bitmap HelpImage
        {
            get { return null; }
        }

        public string HelpText
        {
            get { return "Based on the Normalized data this tool generates input data for Neural Network Tool"; }
        }

        public string HelpUrl
        {
            get { return "Neural NetworkNeural Nework Input Source Data"; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return null; }
        }

        public void Initialize()
        {
            _inputParameters = new Parameter[1];

           _inputParameters[0] = new FileParam("Select the Normalized source file - Streamflow");

            _outputParameters = new Parameter[1];

            _outputParameters[0] = new FileParam("Neural Network Input Normalized Data");
        }

        public Parameter[] InputParameters
        {
            get { return _inputParameters; }
        }

        public string Name
        {
            get { return "Neural Nework Single Input Source Data"; }
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
            get { return "Neural Nework Input Source Data"; }
        }

        public string UniqueName
        {
            get { return "Neural Nework Single Input Source Data"; }
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
