// Purpose: Read input files for ET and use dbwriter to have sqlite data
// Author: Mehmet Ercan (mehmetbercan@gmail.com)
// Advisor: Jonathan L. Goodall (goodall@sc.edu)
// History: Created (05-03-2011)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SMW;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Wrapper;

namespace csvfileReader
{
    public class WR : SMW.Wrapper
    {
        public string output_quantity_1;
        public string output_quantity_2;
        public string output_quantity0;
        public string output_quantity1;
        public string output_quantity2;
        public string output_quantity3;
        public string output_quantity4;
        public string output_quantity5;
        public string output_quantity6;
        public string output_quantity7;
        public string output_quantity8;
        public string output_quantity9;
        public string output_elementset_1;
        public string output_elementset_2;
        public string output_elementset0;
        public string output_elementset1;
        public string output_elementset2;
        public string output_elementset3;
        public string output_elementset4;
        public string output_elementset5;
        public string output_elementset6;
        public string output_elementset7;
        public string output_elementset8;
        public string output_elementset9;

        Dictionary<ITime, List<double[]>> output = new Dictionary<ITime, List<double[]>>();

        //defining data storage list
        List<List<double[]>> Allwtmp = new List<List<double[]>>();

        public override void Finish()
        {
            
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string configFile = null;
            string data_dir = null;

            //Get Config file directory from .omi file
            if (properties.ContainsKey("ConfigFile"))
                configFile = (string)properties["ConfigFile"];
            //Get wtmp files directory from .omi file
            if (properties.ContainsKey("DataFolder"))
                data_dir = (string)properties["DataFolder"];

            //Set variables
            SetVariablesFromConfigFile(configFile);
            SetValuesTableFields();

            //get output exchange item attributes
            int num_outputs = this.GetOutputExchangeItemCount();
            OutputExchangeItem output_1 = this.GetOutputExchangeItem(num_outputs - 12);
            output_elementset_1 = output_1.ElementSet.ID;
            output_quantity_1 = output_1.Quantity.ID;
            OutputExchangeItem output_2 = this.GetOutputExchangeItem(num_outputs - 11);
            output_elementset_2 = output_2.ElementSet.ID;
            output_quantity_2 = output_2.Quantity.ID;
            OutputExchangeItem output = this.GetOutputExchangeItem(num_outputs - 10);
            output_elementset0 = output.ElementSet.ID;
            output_quantity0 = output.Quantity.ID;
            OutputExchangeItem output1 = this.GetOutputExchangeItem(num_outputs - 9);
            output_elementset1 = output1.ElementSet.ID;
            output_quantity1 = output1.Quantity.ID;
            OutputExchangeItem output2 = this.GetOutputExchangeItem(num_outputs - 8);
            output_elementset2 = output2.ElementSet.ID;
            output_quantity2 = output2.Quantity.ID;
            OutputExchangeItem output3 = this.GetOutputExchangeItem(num_outputs - 7);
            output_elementset3 = output3.ElementSet.ID;
            output_quantity3 = output3.Quantity.ID;
            OutputExchangeItem output4 = this.GetOutputExchangeItem(num_outputs - 6);
            output_elementset4 = output4.ElementSet.ID;
            output_quantity4 = output4.Quantity.ID;
            OutputExchangeItem output5 = this.GetOutputExchangeItem(num_outputs - 5);
            output_elementset5 = output5.ElementSet.ID;
            output_quantity5 = output5.Quantity.ID;
            OutputExchangeItem output6 = this.GetOutputExchangeItem(num_outputs - 4);
            output_elementset6 = output6.ElementSet.ID;
            output_quantity6 = output6.Quantity.ID;
            OutputExchangeItem output7 = this.GetOutputExchangeItem(num_outputs - 3);
            output_elementset7 = output7.ElementSet.ID;
            output_quantity7 = output7.Quantity.ID;
            OutputExchangeItem output8 = this.GetOutputExchangeItem(num_outputs - 2);
            output_elementset8 = output8.ElementSet.ID;
            output_quantity8 = output8.Quantity.ID;
            OutputExchangeItem output9 = this.GetOutputExchangeItem(num_outputs - 1);
            output_elementset9 = output9.ElementSet.ID;
            output_quantity9 = output9.Quantity.ID;

            ////////////specifiying shp file/////////////
            //Get the input and output element sets from the SMW
            ElementSet out_elem = (ElementSet)this.Outputs[0].ElementSet;
            //Set some ElementSet properties
            out_elem.ElementType = OpenMI.Standard.ElementType.XYPoint;



            //Get all the values from .wtmp file
            string[] wtmDir = Directory.GetFiles(data_dir, "*wtmp");
            string line;
            for (int f = 0; f < wtmDir.Length; f++)
            {
                //defining average arrays for whole data[0] and months[1],[2]....[12]
                double[] AvTemp = new double[13];
                double[] AvDew = new double[13];
                double[] AvTmax = new double[13];
                double[] AvTmin = new double[13];
                double[] AvWnd = new double[13];

                List<double[]> wtmp = new List<double[]>();
                StreamReader data = new StreamReader(wtmDir[f]);
                line = data.ReadLine(); //Read first info line...
                line = data.ReadLine(); //Read Second line...
                double[] StationInfo = new double[4];
                StationInfo[0] = Convert.ToDouble(line.Split(',')[11]); //Read StationID from second line
                StationInfo[1] = Convert.ToDouble(line.Split(',')[3]); //Read Latitude from second line
                StationInfo[2] = Convert.ToDouble(line.Split(',')[5]); //Read Longitude from second line
                StationInfo[3] = Convert.ToDouble((line.Split(',')[1]).Substring(0, (line.Split(',')[1]).Length - 1));//(line.Split(',')[1]);for the tab at the end of elevation //Read Elevation from second line

                ////////////specifiying shp file (continues)/////////////
                //Create elements 
                Element e1 = new Element();
                Vertex v1 = new Vertex(StationInfo[2], StationInfo[1], StationInfo[3]);
                e1.AddVertex(v1);
                out_elem.AddElement(e1);

                
                line = data.ReadLine(); //Read Third info line...
                while ((line = data.ReadLine()) != null)
                {
                    string[] column = line.Split(',');
                    double[] values = new double[10];
                    values[0] = Convert.ToDouble(column[0]); //date
                    values[1] = Convert.ToDouble(column[1]); //Temperature
                    values[2] = Convert.ToDouble(column[2]); //Dew Point Temperature
                    values[3] = Convert.ToDouble(column[3]); //Maximum Temperature
                    values[4] = Convert.ToDouble(column[4]); //Minumum Temperature
                    values[5] = Convert.ToDouble(column[5]); //Wind Speed
                    wtmp.Add(values);
                }


                #region Getting avarage values for missing ones and replacing them


                //Get whole average values
                double SumTemp = 0; double st = 0; double SumDew = 0; double sd = 0; double SumTmax = 0;
                double stmx = 0; double SumTmin = 0; double stmn = 0; double SumWnd = 0; double sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                    if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                    if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                    if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                    if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                }
                AvTemp[0] = SumTemp / st;
                AvDew[0] = SumDew / sd;
                AvTmax[0] = SumTmax / stmx;
                AvTmin[0] = SumTmin / stmn;
                AvWnd[0] = SumWnd / sw;

                //Check if enough data is available for the gage
                if (st < 350 || sd < 350 || stmx < 350 || stmn < 350 || sw < 350)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }

                //Get Monthly average values
                //January
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "01")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[1] = SumTemp / st; } else { AvTemp[1] = AvTemp[0]; }
                if (sd != 0) { AvDew[1] = SumDew / sd; } else { AvDew[1] = AvDew[0]; }
                if (stmx != 0) { AvTmax[1] = SumTmax / stmx; } else { AvTmax[1] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[1] = SumTmin / stmn; } else { AvTmin[1] = AvTmin[0]; }
                if (sw != 0) { AvWnd[1] = SumWnd / sw; } else { AvWnd[1] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "01")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[1]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[1]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[1]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[1]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[1]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }

                //February
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "02")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[2] = SumTemp / st; } else { AvTemp[2] = AvTemp[0]; }
                if (sd != 0) { AvDew[2] = SumDew / sd; } else { AvDew[2] = AvDew[0]; }
                if (stmx != 0) { AvTmax[2] = SumTmax / stmx; } else { AvTmax[2] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[2] = SumTmin / stmn; } else { AvTmin[2] = AvTmin[0]; }
                if (sw != 0) { AvWnd[2] = SumWnd / sw; } else { AvWnd[2] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "02")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[2]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[2]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[2]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[2]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[2]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //March
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "03")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[3] = SumTemp / st; } else { AvTemp[3] = AvTemp[0]; }
                if (sd != 0) { AvDew[3] = SumDew / sd; } else { AvDew[3] = AvDew[0]; }
                if (stmx != 0) { AvTmax[3] = SumTmax / stmx; } else { AvTmax[3] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[3] = SumTmin / stmn; } else { AvTmin[3] = AvTmin[0]; }
                if (sw != 0) { AvWnd[3] = SumWnd / sw; } else { AvWnd[3] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "03")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[3]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[3]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[3]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[3]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[3]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //April
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "04")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[4] = SumTemp / st; } else { AvTemp[4] = AvTemp[0]; }
                if (sd != 0) { AvDew[4] = SumDew / sd; } else { AvDew[4] = AvDew[0]; }
                if (stmx != 0) { AvTmax[4] = SumTmax / stmx; } else { AvTmax[4] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[4] = SumTmin / stmn; } else { AvTmin[4] = AvTmin[0]; }
                if (sw != 0) { AvWnd[4] = SumWnd / sw; } else { AvWnd[4] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "04")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[4]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[4]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[4]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[4]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[4]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //May
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "05")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[5] = SumTemp / st; } else { AvTemp[5] = AvTemp[0]; }
                if (sd != 0) { AvDew[5] = SumDew / sd; } else { AvDew[5] = AvDew[0]; }
                if (stmx != 0) { AvTmax[5] = SumTmax / stmx; } else { AvTmax[5] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[5] = SumTmin / stmn; } else { AvTmin[5] = AvTmin[0]; }
                if (sw != 0) { AvWnd[5] = SumWnd / sw; } else { AvWnd[5] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "05")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[5]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[5]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[5]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[5]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[5]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //Jun
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "06")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[6] = SumTemp / st; } else { AvTemp[6] = AvTemp[0]; }
                if (sd != 0) { AvDew[6] = SumDew / sd; } else { AvDew[6] = AvDew[0]; }
                if (stmx != 0) { AvTmax[6] = SumTmax / stmx; } else { AvTmax[6] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[6] = SumTmin / stmn; } else { AvTmin[6] = AvTmin[0]; }
                if (sw != 0) { AvWnd[6] = SumWnd / sw; } else { AvWnd[6] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "06")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[6]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[6]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[6]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[6]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[6]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //July
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "07")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[7] = SumTemp / st; } else { AvTemp[7] = AvTemp[0]; }
                if (sd != 0) { AvDew[7] = SumDew / sd; } else { AvDew[7] = AvDew[0]; }
                if (stmx != 0) { AvTmax[7] = SumTmax / stmx; } else { AvTmax[7] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[7] = SumTmin / stmn; } else { AvTmin[7] = AvTmin[0]; }
                if (sw != 0) { AvWnd[7] = SumWnd / sw; } else { AvWnd[7] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "07")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[7]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[7]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[7]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[7]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[7]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //Agust
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "08")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[8] = SumTemp / st; } else { AvTemp[8] = AvTemp[0]; }
                if (sd != 0) { AvDew[8] = SumDew / sd; } else { AvDew[8] = AvDew[0]; }
                if (stmx != 0) { AvTmax[8] = SumTmax / stmx; } else { AvTmax[8] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[8] = SumTmin / stmn; } else { AvTmin[8] = AvTmin[0]; }
                if (sw != 0) { AvWnd[8] = SumWnd / sw; } else { AvWnd[8] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "08")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[8]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[8]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[8]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[8]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[8]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //September
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "09")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[9] = SumTemp / st; } else { AvTemp[9] = AvTemp[0]; }
                if (sd != 0) { AvDew[9] = SumDew / sd; } else { AvDew[9] = AvDew[0]; }
                if (stmx != 0) { AvTmax[9] = SumTmax / stmx; } else { AvTmax[9] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[9] = SumTmin / stmn; } else { AvTmin[9] = AvTmin[0]; }
                if (sw != 0) { AvWnd[9] = SumWnd / sw; } else { AvWnd[9] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "09")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[9]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[9]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[9]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[9]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[9]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //Octember
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "10")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[10] = SumTemp / st; } else { AvTemp[10] = AvTemp[0]; }
                if (sd != 0) { AvDew[10] = SumDew / sd; } else { AvDew[10] = AvDew[0]; }
                if (stmx != 0) { AvTmax[10] = SumTmax / stmx; } else { AvTmax[10] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[10] = SumTmin / stmn; } else { AvTmin[10] = AvTmin[0]; }
                if (sw != 0) { AvWnd[10] = SumWnd / sw; } else { AvWnd[10] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "10")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[10]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[10]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[10]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[10]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[10]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //November
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "11")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[11] = SumTemp / st; } else { AvTemp[11] = AvTemp[0]; }
                if (sd != 0) { AvDew[11] = SumDew / sd; } else { AvDew[11] = AvDew[0]; }
                if (stmx != 0) { AvTmax[11] = SumTmax / stmx; } else { AvTmax[11] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[11] = SumTmin / stmn; } else { AvTmin[11] = AvTmin[0]; }
                if (sw != 0) { AvWnd[11] = SumWnd / sw; } else { AvWnd[11] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "11")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[11]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[11]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[11]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[11]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[11]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                //December
                SumTemp = 0; st = 0; SumDew = 0; sd = 0; SumTmax = 0;
                stmx = 0; SumTmin = 0; stmn = 0; SumWnd = 0; sw = 0;
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "12")
                    {
                        if (wtmp[i][1] != -999.0) { SumTemp = SumTemp + (wtmp[i][1]); st++; }
                        if (wtmp[i][2] != -999.0) { SumDew = SumDew + (wtmp[i][2]); sd++; }
                        if (wtmp[i][3] != -999.0) { SumTmax = SumTmax + (wtmp[i][3]); stmx++; }
                        if (wtmp[i][4] != -999.0) { SumTmin = SumTmin + (wtmp[i][4]); stmn++; }
                        if (wtmp[i][5] != -999.0) { SumWnd = SumWnd + (wtmp[i][5]); sw++; }
                    }
                }
                if (st != 0) { AvTemp[12] = SumTemp / st; } else { AvTemp[12] = AvTemp[0]; }
                if (sd != 0) { AvDew[12] = SumDew / sd; } else { AvDew[12] = AvDew[0]; }
                if (stmx != 0) { AvTmax[12] = SumTmax / stmx; } else { AvTmax[12] = AvTmax[0]; }
                if (stmn != 0) { AvTmin[12] = SumTmin / stmn; } else { AvTmin[12] = AvTmin[0]; }
                if (sw != 0) { AvWnd[12] = SumWnd / sw; } else { AvWnd[12] = AvWnd[0]; }

                //Replacing Average values for the missing ones
                for (int i = 0; i < wtmp.Count; i++)//Count gives number of rows 
                {
                    if (Convert.ToString(wtmp[i][0]).Substring(4, 2) == "12")
                    {
                        if (wtmp[i][1] == -999.0) { wtmp[i][1] = AvTemp[12]; }
                        if (wtmp[i][2] == -999.0) { wtmp[i][2] = AvDew[12]; }
                        if (wtmp[i][3] == -999.0) { wtmp[i][3] = AvTmax[12]; }
                        if (wtmp[i][4] == -999.0) { wtmp[i][4] = AvTmin[12]; }
                        if (wtmp[i][5] == -999.0) { wtmp[i][5] = AvWnd[12]; }
                    }
                }
                //Check if enough data is available for the gage
                if (st < 25 || sd < 25 || stmx < 25 || stmn < 25 || sw < 25)
                {
                    StationInfo[0] = -999.0;//station id
                    StationInfo[1] = -999.0;//latitude
                    StationInfo[2] = -999.0;//longitude
                    StationInfo[3] = -999.0;//elevation
                }


                #endregion
                wtmp.Add(AvTmin); //The last forth is averagerage Min Temperature (Allwtmp[f][(Allwtmp[f].Count() - 4)][0, 1, ....,12])
                wtmp.Add(AvTmax); //The last thirth is averagerage Max Temperature (Allwtmp[f][(Allwtmp[f].Count() - 3)][0, 1, ....,12])
                wtmp.Add(StationInfo); //The one before last one is Station information (Allwtmp[f][(Allwtmp[f].Count() - 2)][0, 1, ....,3])
                wtmp.Add(AvTemp); //The last one is averagerage Temperature (Allwtmp[f][(Allwtmp[f].Count() - 1)][0, 1, ....,12])
                Allwtmp.Add(wtmp);
            }


            ////////////specifiying shp file (continues)/////////////
            //Giving shp file properties to other output exchange items.
            ElementSet TempMinNext = (ElementSet)this.Outputs[0].ElementSet;
            TempMinNext.ElementType = ElementType.XYPoint;
            TempMinNext.Elements = out_elem.Elements;
            ElementSet MonAvTemp = (ElementSet)this.Outputs[1].ElementSet;
            MonAvTemp.ElementType = ElementType.XYPoint;
            MonAvTemp.Elements = out_elem.Elements;
            ElementSet stID = (ElementSet)this.Outputs[2].ElementSet;
            stID.ElementType = ElementType.XYPoint;
            stID.Elements = out_elem.Elements;
            ElementSet lat = (ElementSet)this.Outputs[3].ElementSet;
            lat.ElementType = ElementType.XYPoint;
            lat.Elements = out_elem.Elements;
            ElementSet lon = (ElementSet)this.Outputs[4].ElementSet;
            lon.ElementType = ElementType.XYPoint;
            lon.Elements = out_elem.Elements;
            ElementSet elev = (ElementSet)this.Outputs[5].ElementSet;
            elev.ElementType = ElementType.XYPoint;
            elev.Elements = out_elem.Elements;
            ElementSet temp = (ElementSet)this.Outputs[6].ElementSet;
            temp.ElementType = ElementType.XYPoint;
            temp.Elements = out_elem.Elements;
            ElementSet tempd = (ElementSet)this.Outputs[7].ElementSet;
            tempd.ElementType = ElementType.XYPoint;
            tempd.Elements = out_elem.Elements;
            ElementSet tempmx = (ElementSet)this.Outputs[8].ElementSet;
            tempmx.ElementType = ElementType.XYPoint;
            tempmx.Elements = out_elem.Elements; 
            ElementSet tempmn = (ElementSet)this.Outputs[9].ElementSet;
            tempmn.ElementType = ElementType.XYPoint;
            tempmn.Elements = out_elem.Elements; 
            ElementSet winds = (ElementSet)this.Outputs[10].ElementSet;
            winds.ElementType = ElementType.XYPoint;
            winds.Elements = out_elem.Elements; 
            ElementSet date1 = (ElementSet)this.Outputs[11].ElementSet;
            date1.ElementType = ElementType.XYPoint;
            date1.Elements = out_elem.Elements;


        }

        public override bool PerformTimeStep()
        {



            /////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////Give values to Dbwriter to prepare sqlite file//////////////
            /////////////////////////////////////////////////////////////////////////////////////////


            //get current time
            DateTime currTime = CalendarConverter.ModifiedJulian2Gregorian(((TimeStamp)this.GetCurrentTime()).ModifiedJulianDay);
            string current_time = Convert.ToString(currTime).Split(' ')[0];

            //get required line from the data
            int i = 0; //required line
            for (int k = 0; k < (Allwtmp[0].Count - 2); k++) //(Allwtmp[f].Count **- 2**) because last two list for Average montly temperatures and StID
            {
                string mo = Convert.ToString(Convert.ToUInt32(Convert.ToString(Allwtmp[0][k][0]).Substring(4, 2)));
                string day = Convert.ToString(Convert.ToUInt32(Convert.ToString(Allwtmp[0][k][0]).Substring(6, 2)));
                string year = Convert.ToString(Convert.ToUInt32(Convert.ToString(Allwtmp[0][k][0]).Substring(0, 4)));
                string date = (mo + "/" + day + "/" + year);
                if (date == current_time) { i = k; break; }
            }

            //Looping all the stations
            List<double[]> StIDET = new List<double[]>();
            for (int f = 0; f < (Allwtmp.Count()); f++)
            {
                //Getting Calculated Evapotranspiration(mm/d)
                double[] PET1 = new double[12];
                PET1[0] = Allwtmp[f][(Allwtmp[f].Count() - 2)][0]; //StationID
                PET1[1] = Allwtmp[f][(Allwtmp[f].Count() - 2)][1]; //Latitude
                PET1[2] = Allwtmp[f][(Allwtmp[f].Count() - 2)][2]; //Longitude
                PET1[3] = Allwtmp[f][(Allwtmp[f].Count() - 2)][3]; //elevation(m)
                PET1[4] = Allwtmp[f][i][1];                        //Temperature(C)
                PET1[5] = Allwtmp[f][i][2];                        //Dew Point Temperature(C)
                PET1[6] = Allwtmp[f][i][3];                        //Maximum Temperature(C)
                PET1[7] = Allwtmp[f][i][4];                        //Minimum Temperature(C)
                PET1[8] = Allwtmp[f][i][5];                        //wind speed (m/s)
                PET1[9] = Allwtmp[f][i][0];                        //Date
                PET1[10] = Allwtmp[f][i + 1][4];                   //Minimum Temperature(C) for next day
                //Monthly mean dailly range of air temperature(C)
                int mnth = Convert.ToInt32(Convert.ToString(Allwtmp[f][i][0]).Substring(4, 2));
                PET1[11] = (Allwtmp[f][(Allwtmp[f].Count() - 3)][mnth]) - (Allwtmp[f][(Allwtmp[f].Count() - 4)][mnth]);
                //Add Values
                StIDET.Add(PET1);
            }

            //Get an array result for exchange with SMW
            double[] StID = new double[StIDET.Count()];
            double[] Lat = new double[StIDET.Count()];
            double[] Lon = new double[StIDET.Count()];
            double[] Elev = new double[StIDET.Count()];
            double[] Temp = new double[StIDET.Count()];
            double[] TempDew = new double[StIDET.Count()];
            double[] TempMax = new double[StIDET.Count()];
            double[] TempMin = new double[StIDET.Count()];
            double[] Wind = new double[StIDET.Count()];
            double[] Date = new double[StIDET.Count()];
            double[] MinTfND = new double[StIDET.Count()]; //Minumum Temperature for next day
            double[] MmdTR = new double[StIDET.Count()]; //MonthlyMeanDailyTempRange

            for (int ff = 0; ff < StIDET.Count(); ff++)
            {
                StID[ff] = StIDET[ff][0];
                Lat[ff] = StIDET[ff][1];
                Lon[ff] = StIDET[ff][2];
                Elev[ff] = StIDET[ff][3];
                Temp[ff] = StIDET[ff][4];
                TempDew[ff] = StIDET[ff][5];
                TempMax[ff] = StIDET[ff][6];
                TempMin[ff] = StIDET[ff][7];
                Wind[ff] = StIDET[ff][8];
                Date[ff] = StIDET[ff][9];
                MinTfND[ff] = StIDET[ff][10];
                MmdTR[ff] = StIDET[ff][11];
            }


            //set values
            this.SetValues(output_quantity0, output_elementset0, new ScalarSet(StID));
            this.SetValues(output_quantity1, output_elementset1, new ScalarSet(Lat));
            this.SetValues(output_quantity2, output_elementset2, new ScalarSet(Lon));
            this.SetValues(output_quantity3, output_elementset3, new ScalarSet(Elev));
            this.SetValues(output_quantity4, output_elementset4, new ScalarSet(Temp));
            this.SetValues(output_quantity5, output_elementset5, new ScalarSet(TempDew));
            this.SetValues(output_quantity6, output_elementset6, new ScalarSet(TempMax));
            this.SetValues(output_quantity7, output_elementset7, new ScalarSet(TempMin));
            this.SetValues(output_quantity8, output_elementset8, new ScalarSet(Wind));
            this.SetValues(output_quantity9, output_elementset9, new ScalarSet(Date));
            this.SetValues(output_quantity_1, output_elementset_1, new ScalarSet(MinTfND));
            this.SetValues(output_quantity_2, output_elementset_2, new ScalarSet(MmdTR));

            //this.SetValues(output_quantity2, output_elementset2, new ScalarSet(StID));


            output.Add(this.GetCurrentTime(), StIDET); ;

            //set advance model trough time 
            this.AdvanceTime();

            return true;
        }
    }



}






