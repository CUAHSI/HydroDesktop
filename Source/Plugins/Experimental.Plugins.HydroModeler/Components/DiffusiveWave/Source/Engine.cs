using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;

namespace DiffusiveWave.Source
{
    public class Engine
    {
        public double _cellsize { get; private set;}
        public double _hw;
        public int     _elementCount   { get; private set; }
        public double epsilon { get; set; }
        public double[,] elevations{get;set;}
        public double datum = 1000000000;
        public int rows { get; set; }
        public int cols { get; set; }

        public Engine(double hw)
        {
            this._hw = hw;
            this.epsilon = 0.01;
        }

        public double[] SuccessiveOverRelaxation(double[,] A, double[] b)
        {

            double[] x = new double[A.GetLength(0)];
            double[] x1 = new double[A.GetLength(0)];

            //calculate optimal relaxation factor
            double C = Math.Cos(Math.PI / A.GetLength(0)) + Math.Cos(Math.PI / A.GetLength(1));
            double w = 4 / (2 + Math.Sqrt(4 + Math.Pow(C, 2)));

            for (int k = 0; k <= 100; k++)
            {
                for (int i = 0; i <= A.GetLength(0) - 1; i++)
                {
                    double R = 0;

                    //calculate residual R
                    for (int j = 0; j <= i - 1; j++)
                        R += A[i, j] * x1[j];
                    for (int j = i + 1; j <= A.GetLength(0) - 1; j++)
                        R += A[i, j] * x[j];

                    //calculate X at time k+1
                    x1[i] = (1 - w) * x[i] + (w / A[i, i]) * (b[i] - R);
                }

                //check for convergence

                for (int i = 0; i <= A.GetLength(0) - 1; i++)
                {
                    double value = Math.Abs((x1[i] - x[i]) / x1[i]);

                    if (value > epsilon)
                        break;

                    if (i == A.GetLength(0) - 1)
                    {
                        //all values must have met convergence criteria

                        //set x(k) = x(k+1)
                        x1.CopyTo(x, 0);

                        //return result
                        return x1;
                    }
                }

                //set x(k) = x(k+1)
                x1.CopyTo(x, 0);

            }

            //convergence was not met within the maximum number of iterations
            throw new Exception("Convergence was not reached in SOR method!!!");


        }

        /// <summary>
        /// Builds the ElementSet
        /// </summary>
        /// <param name="elevation">Path to raster ASCII file (produced using ARC GIS)</param>
        /// <param name="id">ElementSet ID</param>
        /// <param name="desc">ElementSet Description</param>
        /// <returns>Element Set</returns>
        public void BuildElementSet(string elevation, string fdr, string id, string desc, out ElementSet elementset, out double[,] sox, out double[,] soy)
        {
            System.IO.StreamReader ReadElev = new System.IO.StreamReader(elevation);
            //System.IO.StreamReader ReadFdr  = new System.IO.StreamReader(fdr);

            //read element set attributes from elevation file
            string[] ElevLine = ReadElev.ReadLine().Split(' ');
            cols = Convert.ToInt32(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            rows = Convert.ToInt32(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double xlower = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double ylower = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double cellsize = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double NODATA = Convert.ToDouble(ElevLine[ElevLine.Length - 1]);
            
            ////read element set attributes from fdr file
            //string[] FdrLine = ReadFdr.ReadLine().Split(' ');
            //if (cols != Convert.ToInt32(FdrLine[FdrLine.Length - 1]))
            //    throw new Exception("Elevation and Flow Direction files do not contain the same number of columns");
            //FdrLine = ReadFdr.ReadLine().Split(' ');
            //if (rows != Convert.ToInt32(FdrLine[FdrLine.Length - 1]))
            //    throw new Exception("Elevation and Flow Direction files do not contain the same number of rows");
            ////skip the rest of the attributes
            //FdrLine = ReadFdr.ReadLine().Split(' ');
            //FdrLine = ReadFdr.ReadLine().Split(' ');
            //FdrLine = ReadFdr.ReadLine().Split(' ');
            //FdrLine = ReadFdr.ReadLine().Split(' ');
            //string fdrNoData = FdrLine[FdrLine.Length - 1];


            //set cellsize
            _cellsize = cellsize;

            //set element count
            _elementCount = rows * cols;

            //create array to hold elevations
            elevations = new double[rows, cols];
            soy = new double[rows, cols];
            sox = new double[rows, cols];

            //get x upper and yupper coordinates
            double x = xlower;
            double y = ylower + cellsize * rows;

            //define element set
            elementset = new ElementSet();
            elementset.Description = desc;
            elementset.ID = id;
            elementset.ElementType = OpenMI.Standard.ElementType.XYPoint;


            //read elevations
            for (int i = 0; i <= rows - 1; i++)
            {
                ElevLine = ReadElev.ReadLine().Split(' ');

                for (int j = 0; j <= cols - 1; j++)
                {
                    //get the elevation
                    double z = Convert.ToDouble(ElevLine[j]);

                    //create a new element and add a vertex to it
                    Element e = new Element();
                    Vertex v = new Vertex(x, y, z);
                    e.AddVertex(v);

                    //add the new element to the element set
                    elementset.AddElement(e);

                    //get the new x coordinate
                    x += cellsize;

                    //save this elevation
                    elevations[i, j] = z;

                    //save the new datum as the lowest elevation
                    if (z < datum)
                        datum = z - 2;
                }

                //get the new y coordinate, and reset the x coordinate
                y -= cellsize;
                x = xlower;
            }

            ReadElev.Close();


            //calculate slopes in the x and y directions
            for (int i = 0; i <= rows - 1; i++)
            {
                //FdrLine = ReadFdr.ReadLine().Split(' ');
                for (int j = 0; j <= cols - 1; j++)
                {

                    double SOY = 0;
                    double SOX = 0;

                    //BROKEN:  Need to use FDR too
                    #region Old Slope Calc
                    ////if its the first row
                    //if (i == 0)
                    //{
                    //    //take the difference between the (i)th and (i+1)th
                    //    SOY = elevations[i, j] - elevations[i + 1, j];

                    //    //check to see if something went wrong!
                    //    if (Math.Abs(SOY) > elevations[i, j] || Math.Abs(SOY) > elevations[i + 1, j])
                    //        SOY = 0.0;
                    //}
                    ////if its the last row
                    //else if (i == rows - 1)
                    //{
                    //    //take the difference between the (i)th and (i-1)th
                    //    SOY = elevations[i, j] - elevations[i - 1, j];

                    //    //check to see if something went wrong!
                    //    if (Math.Abs(SOY) > elevations[i, j] || Math.Abs(SOY) > elevations[i - 1, j])
                    //        SOY = 0.0;
                    //}
                    ////if its an interior row
                    //else
                    //{
                    //    //take the maximum of the two
                    //    double soy1 = elevations[i, j] - elevations[i + 1, j];
                    //    double soy2 = elevations[i, j] - elevations[i - 1, j];

                    //    if (Math.Abs(soy1) > Math.Abs(elevations[i, j]))
                    //    {
                    //        if (Math.Abs(soy2) > Math.Abs(elevations[i, j]))
                    //        {
                    //            SOY = 0.0;
                    //        }
                    //        else
                    //        {
                    //            SOY = soy2;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (Math.Abs(soy2) > Math.Abs(elevations[i, j]))
                    //        {
                    //            SOY = soy1;
                    //        }
                    //        else
                    //        {
                    //            if (Math.Abs(soy1) > Math.Abs(soy2))
                    //                SOY = soy1;
                    //            else
                    //                SOY = soy2;
                    //        }
                    //    }

                    //}

                    ////if its the first column
                    //if (j == 0)
                    //{
                    //    //take the difference between the (j)th and (j+1)th
                    //    SOX = elevations[i, j] - elevations[i, j + 1];

                    //    //check to see if something went wrong!
                    //    if (Math.Abs(SOX) > elevations[i, j] || Math.Abs(SOX) > elevations[i, j + 1])
                    //        SOX = 0.0;
                    //}
                    ////if its the last column
                    //else if (j == cols - 1)
                    //{
                    //    //take the difference between the (j)th and (j-1)th
                    //    SOX = elevations[i, j] - elevations[i, j - 1];

                    //    //check to see if something went wrong!
                    //    if (Math.Abs(SOX) > elevations[i, j] || Math.Abs(SOX) > elevations[i, j - 1])
                    //        SOX = 0.0;
                    //}
                    ////if its an interior column
                    //else
                    //{

                    //    //take the maximum of the two
                    //    double sox1 = elevations[i, j] - elevations[i, j + 1];
                    //    double sox2 = elevations[i, j] - elevations[i, j - 1];

                    //    if (Math.Abs(sox1) > Math.Abs(elevations[i, j]))
                    //    {
                    //        if (Math.Abs(sox2) > Math.Abs(elevations[i, j]))
                    //        {
                    //            SOX = 0.0;
                    //        }
                    //        else
                    //        {
                    //            SOX = sox2;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (Math.Abs(sox2) > Math.Abs(elevations[i, j]))
                    //        {
                    //            SOX = sox1;
                    //        }
                    //        else
                    //        {
                    //            if (Math.Abs(sox1) > Math.Abs(sox2))
                    //                SOX = sox1;
                    //            else
                    //                SOX = sox2;
                    //        }
                    //    }
                    //}
                    #endregion

                    //---
                    //--- get the fdr value and use it to choose x any y slopes for each element
                    //---

                    #region Slope Calc using FDR
                    //int Xdirection = 1; //denotes flow to the left
                    //int Ydirection = 1; //denotes flow uppwared
                    ////make sure the that flow direction isn't NODATA
                    //if (FdrLine[j] != fdrNoData)
                    //{

                    //    switch (Convert.ToInt32(FdrLine[j]))
                    //    {
                    //        case 1:
                    //            try { SOX = elevations[i, j] - elevations[i, j + 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            break;
                    //        case 2:
                    //            try { SOX = elevations[i, j] - elevations[i, j + 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            try { SOY = elevations[i, j] - elevations[i + 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            break;
                    //        case 4:
                    //            try { SOY = elevations[i, j] - elevations[i + 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            break;
                    //        case 8:
                    //            try { SOX = elevations[i, j] - elevations[i, j - 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            try { SOY = elevations[i, j] - elevations[i + 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i,j] - 5; }//Arbitrarily selected "5"
                    //            Xdirection = -1;
                    //            break;
                    //        case 16:
                    //            try { SOX = elevations[i, j] - elevations[i, j - 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            Xdirection = -1;
                    //            break;
                    //        case 32:
                    //            try { SOX = elevations[i, j] - elevations[i, j - 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            try { SOY = elevations[i, j] - elevations[i - 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            Xdirection = -1;
                    //            Ydirection = -1;
                    //            break;
                    //        case 64:
                    //            try { SOY = elevations[i, j] - elevations[i - 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            Ydirection = -1;
                    //            break;
                    //        case 128:
                    //            try { SOX = elevations[i, j] - elevations[i, j + 1]; }
                    //            catch (IndexOutOfRangeException) { SOX = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            try { SOY = elevations[i, j] - elevations[i - 1, j]; }
                    //            catch (IndexOutOfRangeException) { SOY = elevations[i, j] - 5; }//Arbitrarily selected "5"
                    //            Ydirection = -1;
                    //            break;
                    //    }


                    //    //HACK: If SOX or SOY is < 0, set it equal to 0.  This can happen in Cases 2,8,32,128. 
                    //    if (SOX < 0 || SOX == elevations[i,j] - Convert.ToInt32(fdrNoData)) { SOX = 0; }   //this ensures that there are no negative slopes or NoData calculations
                    //    if (SOY < 0 || SOY == elevations[i,j] - Convert.ToInt32(fdrNoData)) { SOY = 0; }
                    //}

                    ////set slope values
                    //sox[i, j] = Xdirection * SOX / cellsize;
                    //soy[i, j] = Ydirection * SOY / cellsize;
                    #endregion

                    //only calculate the slope in the positive X and positive Y directions
                    if (i != rows - 1)
                    {
                        SOY = elevations[i, j] - elevations[i + 1, j];
                    }
                    else
                    {
                        SOY = 1;
                    }

                    if (j != cols - 1)
                    {
                        SOX = elevations[i, j] - elevations[i, j + 1];
                    }
                    else
                    {
                        SOX = 1;
                    }

                    //set slope values
                    sox[i, j] = SOX / cellsize;
                    soy[i, j] = SOY / cellsize;




                }
            }
            //ReadFdr.Close();
        }

        /// <summary>
        /// Calculates the flow between the river and the flood plain
        /// </summary>
        /// <param name="stage">River Stage</param>
        /// <param name="h">Floodplain Stage for all cells</param>
        /// <returns>Flow from river into floodplain = postive values; flow from floodplain into river = negative values</returns>
        public double[] Stage2Flow(double[] stage, double[] h, double dt)
        {
            //assume broad crested weir
            double[] V = new double[_elementCount];
            double[] Q = new double[_elementCount];
            double I = 0;
            double O = 0;
            //get number of elements per row
            int rowCount = _elementCount / stage.Length;


            //HACK: should probably be 9.81
            //double g = 32.2;

            int rowID = 0;
            double Stage = stage[0];

            for (int i = 0; i <= _elementCount - 1; i++)
            {
                if (stage[i] != 0)
                {
                    
                    //set crest elevation of embankment

                    //set height of river
                    double hr = stage[i] + (elevations[rowID, 0] - datum);
                    //set height of floodplain
                    double hfp = h[i] + +(elevations[rowID, 0] - datum);
                    //set the weir height
                    double hw = _hw + elevations[rowID, 0] - datum;

                    double Sb = 0;
                    double Hr = 0;

                    //calculate Hr
                    if (hr > hfp && hr > hw)
                        Hr = (hr - hw) / (hfp - hw);
                    else if (hfp > hw && hfp > hr)
                        Hr = (hfp - hw) / (hr - hw);

                    //calculate submergence factor
                    if (Hr > 0.67)
                        Sb = 1.0 - 27.8 * Math.Pow(Hr - 0.67, 3);
                    else
                        Sb = 1.0;

                    double Cf = 0.62;  //weir discharge coefficient (broad crested)(SI)

                    if (hr > hw && hr > hfp)
                        I += Cf * Sb * Math.Pow(hr - hw, 3.0 / 2.0) * _cellsize;
                    else if (hfp > hw && hfp > hr)
                        O += Cf * Sb * Math.Pow(hfp - hw, 3.0 / 2.0) * _cellsize;

                    //Q[i] = (h[i] * _cellsize * _cellsize) / dt + (I - O);

                }
                //else
                //{
                //    Q[i] = Q[i - 1];
                //}

            }

            for (int i = 0; i <= _elementCount - 1; i++)
            {
                if (i % rowCount == 0)
                {
                    if (O < 0)
                        O *= -1;
                    if (I < 0)
                        I *= -1;
                    V[i] = (h[i] * _cellsize * _cellsize) + (I - O) * dt;
                    double flux = (V[i] - (h[i] * _cellsize * _cellsize)) / dt;
                    Q[i] = flux;
                }
 
            }
            //return V;
            return Q;
        }
    }


    public class Engine2
    {
        public double _cellsize { get; private set;}
        public double _hw;
        public int     _elementCount   { get; private set; }
        public double epsilon { get; set; }
        public double[,] _elevations{get;set;}
        public double datum = 1000000000;
        public int rows { get; set; }
        public int cols { get; set; }

        double _dt;
        public double nx { private get; set; }          //To be replaced with _nx[,]
        public double ny { private get; set; }          //To be replaced with _ny[,]
        //public double[,] _nx { private get; set; }
        //public double[,] _ny { private get; set; }

        public double[] head;
        public double[,] _sox { private get; set; }
        public double[,] _soy { private get; set; }
        public double[,] A;
        public double[] q;
        public double[,] H;
        public double[,][] _properties;

        public Engine2(double hw, double dt)
        {
            this._hw = hw;
            this.epsilon = 0.01;   
            this._dt = dt;
        }

        public double[] SuccessiveOverRelaxation()
        {

            double[] x = new double[A.GetLength(0)];
            double[] x1 = new double[A.GetLength(0)];

            int length = rows * cols;

            //calculate optimal relaxation factor
            //double C = Math.Cos(Math.PI / A.GetLength(0)) + Math.Cos(Math.PI / A.GetLength(1));
            double C = Math.Cos(Math.PI / length) + Math.Cos(Math.PI / length);
            double w = 4 / (2 + Math.Sqrt(4 + Math.Pow(C, 2)));

            for (int k = 0; k <= 100; k++)
            {
                for (int i = 0; i <= A.GetLength(0) - 1; i++)
                {
                    double R = 0;
                    //calculate residual R
                    if (i - cols >= 0)
                        R += A[i, 0] * x1[i - cols];
                    if (i - 1 >= 0)
                        R += A[i, 1] * x1[i - 1];
                    if (i + 1 <= x.Length - 1)
                        R += A[i, 3] * x[i + 1];
                    if (i + cols <= x.Length - 1)
                        R += A[i, 4] * x[i + cols];
                    
                    //calculate X at time k+1
                    x1[i] = (1 - w) * x[i] + (w / A[i, 2]) * (q[i] - R);

                    //calculate residual R
                    //for (int j = 0; j <= 1; j++)
                    //    R += A[i, j] * x1[j];
                    //for (int j = 3; j <= 4; j++)
                    //    R += A[i, j] * x[j];

                    //calculate X at time k+1
                    //x1[i] = (1 - w) * x[i] + (w / A[i, 2]) * (q[i] - R);
                }

                //check for convergence

                for (int i = 0; i <= A.GetLength(0) - 1; i++)
                {
                    double value = Math.Abs((x1[i] - x[i]) / x1[i]);

                    if (value > epsilon)
                        break;
                    
                    if (i == A.GetLength(0) - 1)
                    {
                        //all values must have met convergence criteria

                        //set x(k) = x(k+1)
                        x1.CopyTo(x, 0);

                        //return result
                        return x1;
                    }
                }

                //set x(k) = x(k+1)
                x1.CopyTo(x, 0);

            }

            //convergence was not met within the maximum number of iterations
            throw new Exception("Convergence was not reached in SOR method!!!");


        }

        private void init_properties(int rows, int cols)
        {
            //initialize the 2d array
            _properties = new double[rows, cols][];

            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {
                    //create a 3rd dimension to hold various properties
                    _properties[i, j] = new double[5];

                    //_properties[0] = elevation
                    //_properties[1] = XSlope
                    //_properties[2] = YSlope
                    //_properties[3] = Xroughness
                    //_properties[4] = Yroughness
                }
            }
        }


        /// <summary>
        /// Builds the ElementSet
        /// </summary>
        /// <param name="elevation">Path to raster ASCII file (produced using ARC GIS)</param>
        /// <param name="id">ElementSet ID</param>
        /// <param name="desc">ElementSet Description</param>
        /// <returns>Element Set</returns>
        public void BuildElementSet(string elevation, string id, string desc, out ElementSet elementset)
        {
            System.IO.StreamReader ReadElev = new System.IO.StreamReader(elevation);
            //System.IO.StreamReader ReadFdr  = new System.IO.StreamReader(fdr);

            //read element set attributes from elevation file
            string[] ElevLine = ReadElev.ReadLine().Split(' ');
            cols = Convert.ToInt32(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            rows = Convert.ToInt32(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double xlower = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double ylower = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double cellsize = Convert.ToDouble(ElevLine[ElevLine.Length - 1]); ElevLine = ReadElev.ReadLine().Split(' ');
            double NODATA = Convert.ToDouble(ElevLine[ElevLine.Length - 1]);

            //initialize the _properties array
            //[0:elevation],[1:SoX],[2:SoY],[3:Nx],[4:Ny]
            init_properties(rows, cols);

            //set cellsize
            _cellsize = cellsize;

            //set element count
            _elementCount = rows * cols;

            //initialize stiffness and source arrays
            this.A = new double[_elementCount, 5];
            //this.A = new double[_elementCount, _elementCount];
            this.q = new double[_elementCount];

            //create array to hold elevations
            _elevations = new double[rows, cols];
            _soy = new double[rows, cols];
            _sox = new double[rows, cols];

            //get x upper and yupper coordinates
            double x = xlower;
            double y = ylower + cellsize * rows;

            //define element set
            elementset = new ElementSet();
            elementset.Description = desc;
            elementset.ID = id;
            elementset.ElementType = OpenMI.Standard.ElementType.XYPoint;


            //read elevations
            for (int i = 0; i <= rows - 1; i++)
            {
                ElevLine = ReadElev.ReadLine().Split(' ');

                for (int j = 0; j <= cols - 1; j++)
                {
                    //get the elevation
                    double z = Convert.ToDouble(ElevLine[j]);

                    //create a new element and add a vertex to it
                    Element e = new Element();
                    Vertex v = new Vertex(x, y, z);
                    e.AddVertex(v);

                    //add the new element to the element set
                    elementset.AddElement(e);

                    //get the new x coordinate
                    x += cellsize;

                    //save this elevation
                    //_elevations[i, j] = z;

                    //save this elevation 
                    _properties[i, j][0] = z;

                    //save the new datum as the lowest elevation
                    if (z < datum)
                        datum = z - 2;
                }

                //get the new y coordinate, and reset the x coordinate
                y -= cellsize;
                x = xlower;
            }

            ReadElev.Close();


            //calculate slopes in the x and y directions
            for (int i = 0; i <= rows - 1; i++)
            {
                //FdrLine = ReadFdr.ReadLine().Split(' ');
                for (int j = 0; j <= cols - 1; j++)
                {
                    double SOY = 0;
                    double SOX = 0;

                    //only calculate the slope in the positive X and positive Y directions
                    if (i != rows - 1)
                    {
                        //SOY = _elevations[i, j] - _elevations[i + 1, j];
                        SOY = _properties[i, j][0] - _properties[i + 1, j][0];
                    }
                    else
                    {
                        SOY = 1;
                    }

                    if (j != cols - 1)
                    {
                        //SOX = _elevations[i, j] - _elevations[i, j + 1];
                        SOX = _properties[i, j][0] - _properties[i, j+1][0];
                    }
                    else
                    {
                        SOX = 1;
                    }

                    //set slope values
                    //_sox[i, j] = SOX / cellsize;
                    //_soy[i, j] = SOY / cellsize;

                    //save slope values
                    _properties[i, j][1] = SOX / cellsize;
                    _properties[i, j][2] = SOY / cellsize;
                }
            }
        }

        /// <summary>
        /// Calculates the flow between the river and the flood plain
        /// </summary>
        /// <param name="stage">River Stage</param>
        /// <param name="h">Floodplain Stage for all cells</param>
        /// <returns>Flow from river into floodplain = postive values; flow from floodplain into river = negative values</returns>
        public double[] Stage2Flow(double[] stage, double[] h, double dt)
        {
            //assume broad crested weir
            //double[] V = new double[_elementCount];
            double[] Q = new double[_elementCount];
            //double I = 0;
            //double O = 0;
            //get number of elements per row
            int rowCount = _elementCount / stage.Length;


            //HACK: should probably be 9.81
            //double g = 32.2;

            //int rowID = 0;
            double Stage = stage[0];

            for (int i = 0; i <= _elementCount - 1; i++)
            {
                //if (stage[i] != 0)
                //{
                    //get row and col
                    int row = ((Int32)Math.Floor((double)i / (double)cols));
                    int col = i % cols;

                    

                    //get height of river
                    double hr = stage[i] + (_properties[row, col][0] - datum);

                    //get height of floodplain
                    double hfp = h[i] + +(_properties[row, col][0] - datum);

                    //get the weir height
                    double hw = _hw + _properties[row, col][0] - datum;

                    double Sb = 0;
                    double Hr = 0;

                    //calculate Hr
                    if (hr > hfp && hr > hw)
                        Hr = (hr - hw) / (hfp - hw);
                    else if (hfp > hw && hfp > hr)
                        Hr = (hfp - hw) / (hr - hw);

                    //calculate submergence factor
                    if (Hr > 0.67)
                        Sb = 1.0 - 27.8 * Math.Pow(Hr - 0.67, 3);
                    else
                        Sb = 1.0;

                    double Cf = 0.62;  //weir discharge coefficient (broad crested)(SI)

                    if (hr > hw && hr > hfp)
                        Q[i] = Cf * Sb * Math.Pow(hr - hw, 3.0 / 2.0) * _cellsize;
                    else if (hfp > hw && hfp > hr)
                        Q[i] = -1* Cf * Sb * Math.Pow(hfp - hw, 3.0 / 2.0) * _cellsize;

                    
                //}

            }

            //for (int i = 0; i <= _elementCount - 1; i++)
            //{
            //    if (i % rowCount == 0)
            //    {
            //        if (O < 0)
            //            O *= -1;
            //        if (I < 0)
            //            I *= -1;
            //        V[i] = (h[i] * _cellsize * _cellsize) + (I - O) * dt;
            //        double flux = (V[i] - (h[i] * _cellsize * _cellsize)) / dt;
            //        Q[i] = flux;
            //    }
 
            //}
            return Q;
        }

        /// <summary>
        /// Builds Stiffness Matrix
        /// </summary>
        /// <param name="stage">River Stage</param>
        /// <param name="h">Excess Head</param>
        /// <param name="b">Flow onto FloodPlain as a result of River Stage</param>
        /// <param name="A">Stiffness Matrix</param>
        /// <param name="q">Source Term</param>
        public void CreateStiffness(double[] stage, double[] excess, double[] h, double[] inflow, int option)
        {
            
            //get the number of rows and cols from Sox (assuming that Sox and Soy are of equal size)
            //int rows = _sox.GetLength(0);
            //int cols = _soy.GetLength(1);
            
            //define some local variables
            double Cx1, Cx2, Cy1, Cy2;
            double dhdx, dhdy, dh2dx2, dh2dy2;

            H = new double[rows, cols];


            //HACK:  stage wont always exist at first element of row!!!  Should instead pass this method an array, where the
            //       stage values have already been assigned to specific locations within the matrix.


            //set the current head at each node equal to the known stage plus the head calculated in the last time step + plus the known excess
            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {
                    H[i, j] = h[j + (i * cols)];
                }
            }

            //create source array based on the fluxes between adjacent cells
            q = CreateSource(inflow, excess);


            //---                            ---//
            //--- Formulate Stiffness Matrix ---//
            //---                            ---//

            System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\Temp\\Refractored.csv",true);
            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {

                    //Structure of A ==> [0:x,y-1] [1:x-1,y] [2:x,y] [3:x+1,y] [4:x,y+1]

                    //------ Calculate the non-linear terms -------//
                    #region Calculate Non-Linear Terms Explicitly

                    //if this is the last column
                    if (j == cols - 1)
                    {
                        //assume that H[i,j+1] = H[i,j] - abs(H[i,j]-H[i,j-1])  to force flow out of the grid
                        double nextHead = H[i, j] - Math.Abs(H[i, j] - H[i, j - 1]);
                        dhdx = (nextHead - H[i, j - 1]) / (2 * _cellsize);
                        dh2dx2 = (nextHead + H[i, j - 1] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                        //dhdx = (H[i, j] - H[i, j - 1]) / (2 * _cellsize);
                        //dh2dx2 = (H[i, j] + H[i, j - 1] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }
                    //if this is the first column
                    else if (j == 0)
                    {
                        //assume that H[i,j-1] = H[i,j]
                        dhdx = (H[i, j + 1] - H[i, j]) / (2 * _cellsize);
                        dh2dx2 = (H[i, j + 1] + H[i, j] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }
                    //if this is an interior column
                    else
                    {
                        dhdx = (H[i, j + 1] - H[i, j - 1]) / (2 * _cellsize);
                        dh2dx2 = (H[i, j + 1] + H[i, j - 1] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }

                    //if this is the last row
                    if (i == rows - 1)
                    {
                        //assume that H[i,j+1] = H[i,j] - abs(H[i,j]-H[i-1,j]) to force flow out of the grid
                        double nextHead = H[i, j] - Math.Abs(H[i, j] - H[i - 1, j]);
                        dhdy = (nextHead - H[i - 1, j]) / (2 * _cellsize);
                        dh2dy2 = (nextHead + H[i - 1, j] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                        //dhdy = (H[i, j] - H[i - 1, j]) / (2 * _cellsize);
                        //dh2dy2 = (H[i, j] + H[i - 1, j] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }
                    //if this is the first row
                    else if (i == 0)
                    {
                        //assume that H[i-1,j] = H[i,j]
                        dhdy = (H[i + 1, j] - H[i, j]) / (2 * _cellsize);
                        dh2dy2 = (H[i + 1, j] + H[i, j] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }
                    //if this is an interior row
                    else
                    {
                        dhdy = (H[i + 1, j] - H[i - 1, j]) / (2 * _cellsize);
                        dh2dy2 = (H[i + 1, j] + H[i - 1, j] - 2 * H[i, j]) / (Math.Pow(_cellsize, 2));
                    }
                    #endregion

                    //-------------- Calculate Slope --------------//
                    #region Calculate X and Y Slopes

                    double XSlope, YSlope;
                    //CHECK THIS!!!: Which values of XSlope and YSlope should I use? 
                    //Using Central differences...
                    if (j == cols - 1)
                    {
                        
                        //force slope out of grid by letting _elevation[i,j+1] = _elevation[i,j]-2
                        double nextelev = _properties[i, j][0] - 2;
                        XSlope = (_properties[i, j - 1][0] - nextelev) / (2 * _cellsize);
                        //XSlope = (_elevation[i, j - 1] - _elevation[i, j]) / (2 * _cellsize);
                    }
                    else if (j == 0)
                        XSlope = (_properties[i, j][0] - _properties[i, j + 1][0]) / (2 * _cellsize);
                    else
                        XSlope = (_properties[i, j - 1][0] - _properties[i, j + 1][0]) / (2 * _cellsize);

                    if (i == rows - 1)
                    {
                        //force slope out of grid by letting _elevation[i+1,j] = _elevation[i,j]-2
                        double nextelev = +_properties[i, j][0] - 2;
                        YSlope = (_properties[i - 1, j][0] - nextelev) / (2 * _cellsize);
                        //YSlope = (_elevation[i - 1, j] - _elevation[i, j]) / (2 * _cellsize);
                    }
                    else if (i == 0)
                        YSlope = (_properties[i, j][0] - _properties[i + 1, j][0]) / (2 * _cellsize);
                    else
                        YSlope = (_properties[i - 1, j][0] - _properties[i + 1, j][0]) / (2 * _cellsize);

                    #endregion

                    //--- SET TERMS IN THE STIFFNESS MATRIX A[,]---//
                    #region Set Stiffness Terms

                    //set (i,j) term [x,y]
                    A[i * cols + j, 2] = 1;

                    //set (i, j+1) term [x+1,y]
                    if (j < cols - 1)
                    {
                        //XSlope = (_elevation[i, j] - _elevation[i, j + 1]) / _cellsize;
                        //dhdx = (H[i, j + 1] - H[i, j - 1]) / (2 * _cellsize);
                        Cx2 = (5.0 / (3.0 * nx)) * Math.Sign(XSlope) * Math.Sqrt(Math.Abs(XSlope - dhdx)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j + 1;//get the next column
                        A[row, 3] = _dt / (2 * _cellsize) * Cx2;
                    }
                    //set (i,j-1) term [x-1,y]
                    if (j - 1 >= 0)
                    {
                        //XSlope = (_elevation[i, j] - _elevation[i, j - 1]) / _cellsize;
                        Cx2 = (5.0 / (3.0 * nx)) * Math.Sign(XSlope) * Math.Sqrt(Math.Abs(XSlope - dhdx)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j - 1;//get the previous column
                        A[row, 1] = -1 * _dt / (2 * _cellsize) * Cx2;
                    }
                    //set (i+1,j) term [x,y+1]
                    if (i + 1 < rows - 1)
                    {
                        //YSlope = (_elevation[i, j] - _elevation[i + 1, j]) / _cellsize;
                        Cy2 = (5.0 / (3.0 * ny)) * Math.Sign(YSlope) * Math.Sqrt(Math.Abs(YSlope - dhdy)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j + cols;//get the column associated with the next row (current col + number of cols in elementset)
                        A[row, 4] = _dt / (2 * _cellsize) * Cy2;
                    }
                    //set (i-1,j) term [x,y-1]
                    if (i - 1 >= 0)
                    {
                        //YSlope = (_elevation[i, j] - _elevation[i - 1, j]) / _cellsize;
                        Cy2 = (5.0 / (3.0 * ny)) * Math.Sign(YSlope) * Math.Sqrt(Math.Abs(YSlope - dhdy)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j - cols;//get the column associated with the previous row (current col - number of cols in elementset)
                        A[row, 0] = -1 * _dt / (2 * _cellsize) * Cy2;
                    }

                    for (int C = 0; C <= A.GetLength(1) - 1; C++)
                        sw.Write(A[i * cols + j, C].ToString() + ",");
                    sw.Write("\n");

                    Cx1 = (1.0 / (2.0 * nx)) * Math.Sign(XSlope) * Math.Pow(Math.Abs(XSlope - dhdx), -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (1.0 / (2.0 * ny)) * Math.Sign(YSlope) * Math.Pow(Math.Abs(YSlope - dhdy), -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);



                    //catch infinity and nan terms
                    if (double.IsInfinity(Cx1) || double.IsNaN(Cx1)) { Cx1 = 0.0; }
                    if (double.IsInfinity(Cy1) || double.IsNaN(Cy1)) { Cy1 = 0.0; }

                    #endregion

                    //------- SET TERMS IN THE SOURCE ARRAY -------//
                    #region Set Source Terms

                    //multiply the flux (populated in Create Source) by dt
                    q[i * cols + j] *= this._dt;
                    //add the previous head term
                    q[i * cols + j] += head[i * cols + j] * (1 - (this._dt) * Cx1 - (this._dt) * Cy1);

                    #endregion
                }
            }
            sw.Write("--------\n");
            sw.Close();
        }
                    
        /// <summary>
        /// Calculates Inter-Cell flow for the Source Array
        /// </summary>
        /// <param name="flows">cell flows at the current timestep</param>
        /// <returns>Source Array with Inter-Cell flows</returns>
        public double[] CreateSource(double[] flows, double[] excess)
        {

            //HACK:
            //********
            //Should I subtract the flux from the outgoing cell???
            //********

            //HACK:
            //*********
            //I think that I should be checking that the head (i.e. water height + elevation) is < 0 rather than just the elevations
            //*********

            double[,] FLUX = new double[rows, cols];
            double[,] E = new double[rows, cols];
            double[,] Head = new double[rows, cols];

            //convert head from previous time step into a 2d array;
            for (int i = 0; i <= rows - 1; i++)
                for (int j = 0; j <= cols - 1; j++)
                    Head[i, j] = head[i * cols + j];

            //Create flux array as a combination of new flow and excess onto each cell
            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {
                    //create an array to hold flux
                    //BROKEN: I think this should be excess[i * cols + j] / _dt instead of (excess[i * cols + j] * _cellsize / _dt)
                    FLUX[i, j] = flows[i * cols + j] + excess[i * cols + j] / _dt;// +(Head[i, j] / _dt);
                }
            }

            //sort elevations descending
            var sorted = from x in Enumerable.Range(0, rows)
                         from y in Enumerable.Range(0, cols)
                         select new
                         {
                             X = x,
                             Y = y,
                             Value = _properties[x, y][0]
                         } into point
                         orderby point.Value descending
                         select point;

            //loop through the sorted indices
            foreach (var index in sorted)
            {
                int i = (int)index.X;
                int j = (int)index.Y;

                //slope for i+1 cell
                if (i + 1 < rows)
                {
                    if (Head[i, j] + _properties[i, j][0] > Head[i + 1, j] + _properties[i + 1, j][0])//_elevations[i + 1, j])
                    {
                        FLUX[i + 1, j] += (FLUX[i, j] / 2) / _cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }

                //slope for i-1 cell
                if (i - 1 >= 0)
                {
                    if (Head[i, j] + _properties[i, j][0] > Head[i - 1, j] + _properties[i - 1, j][0])
                    //if (Head[i, j] + _elevations[i, j] > Head[i - 1, j] + _elevations[i - 1, j])
                    {
                        FLUX[i - 1, j] += (FLUX[i, j] / 2) / _cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }

                //slope for j+1 cell
                if (j + 1 < cols)
                {
                    if (Head[i, j] + _properties[i, j][0] > Head[i, j + 1] + _properties[i, j + 1][0])
                    //if (Head[i, j] + _elevations[i, j] > Head[i, j + 1] + _elevations[i, j + 1])
                    {
                        FLUX[i, j + 1] += (FLUX[i, j] / 2) / _cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }

                //slope for j-1 cell
                if (j - 1 >= 0)
                {
                    if (Head[i, j] + _properties[i, j][0] > Head[i, j - 1] + _properties[i, j - 1][0])
                    //if (Head[i, j] + _elevations[i, j] > Head[i, j - 1] + _elevations[i, j - 1])
                    {
                        FLUX[i, j - 1] += (FLUX[i, j] / 2) / _cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }
            }

            double[] fluxes = new double[flows.Length];
            for (int i = 0; i <= FLUX.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= FLUX.GetLength(1) - 1; j++)
                {
                    fluxes[i * cols + j] = FLUX[i, j];
                }
            }
            return fluxes;

            #region old calc
            /*
        //calculate inter-cell flow
        for (int i = 0; i <= rows - 1; i++)
        {
            for (int j = 0; j <= cols - 1; j++)
            {
                //get the flux from the current timestep
                //double current_flux = flows[i * cols + j];
                double in_flux = 0;

                //slope for i+1 cell
                if (i + 1 < rows)
                {
                    if (H[i, j] + _elevation[i, j] < H[i + 1, j] + _elevation[i + 1, j])
                    {
                        in_flux += FLUX[i + 1, j] / 2;
                    }
                        //if (B[i + 1, j] > 0)
                        //{

                        //---
                        //--- From (Fiedler and Ramirez, 2000)
                        //---l
                        //add flux from river and Pe



                    //    //add inflow from known heads
                    //    //if ((_elevation[i, j] + H[i, j]) - (_elevation[i + 1, j] + H[i + 1, j]) < 0 && H[i + 1, j] > 0)
                    //if (H[i, j] + _elevation[i, j] < H[i + 1, j] + _elevation[i+1, j])
                    //    {
                    //        //HACK: In the future this should consider the difference in head elevations too
                    //        //i.e.: double head_diff = (_elevation[i, j] + H[i, j]) - (_elevation[i + 1, j] + H[i + 1, j]);

                    //        //--- add flux from Pe and Inflow
                    //        in_flux += FLUX[i + 1, j] / 2;
                    //        //subtract this flux from the higher node
                    //        //flows[(i+1) * cols + j] -= FLUX[i + 1, j] / 2;

                    //        //--- add flux from cell stage 
                    //        //Get the available head, assuming that all of it can flow onto the current cell
                    //        double head_diff = H[i, j] - H[i + 1, j];
                    //        //convert the head difference into flux
                    //        double head_flux = head_diff * _cellsize / _dt;
                    //        //apply half of it to the current cell
                    //        ////in_flux += head_flux / 2;

                    //        //subtract this from the higher node
                    //        //FLUX[i + 1, j] -= head_flux / 2;
                            

                    //        //in_flux += B[i + 1, j] / _cellsize;

                    //        //double slope = (-1) * (_elevation[i, j] - _elevation[i + 1, j]) / _cellsize * 100;
                    //        //double velocity = GetVelocity(slope, nx);
                    //        //B[i, j] += ((B[i + 1, j] + _elevation[i + 1, j]) - (current_flux + _elevation[i, j])) / _cellsize * velocity;
                    //    }
                    //}
                }
                //slope for i-1 cell
                if (i - 1 >= 0)
                {

                    if (H[i, j] + _elevation[i, j] < H[i - 1, j] + _elevation[i - 1, j])
                    {
                        in_flux += FLUX[i - 1, j] / 2;
                    }
                    //if (_elevation[i, j] - _elevation[i - 1, j] < 0)
                    //{
                        //if (B[i - 1, j] > 0)
                        //{

                        //---
                        //--- From (Fiedler and Ramirez, 2000)
                        //---
                        //add flux from river and Pe

                        //add inflow from known heads
                        //if ((_elevation[i, j] + H[i, j]) - (_elevation[i - 1, j] + H[i - 1, j]) < 0 && H[i - 1, j] > 0)
                    //if (H[i, j] + _elevation[i, j] < H[i - 1, j] + _elevation[i-1, j])
                    //    {
                    //        //add flux from Pe and Inflow
                    //        in_flux += FLUX[i - 1, j] / 2;
                    //        //subtract this flux from the higher node
                    //        //flows[(i-1) * cols + j] -= FLUX[i - 1, j] / 2;

                    //        //--- add flux from cell stage 
                    //        //Get the available head, assuming that all of it can flow onto the current cell
                    //        double head_diff = Math.Abs(H[i, j] - H[i - 1, j]);
                    //        //convert the head difference into flux
                    //        double head_flux = head_diff * _cellsize / _dt;
                    //        //apply half of it to the current cell
                    //        ////in_flux += head_flux / 2;

                    //        //subtract this from the higher node
                    //        //FLUX[i - 1, j] -= head_flux / 2;
                            

                    //        //in_flux += B[i - 1, j] / _cellsize;

                    //        //double slope = (-1) * (_elevation[i, j] - _elevation[i - 1, j]) / _cellsize * 100;
                    //        //double velocity = GetVelocity(slope, nx);
                    //        //B[i, j] += ((B[i - 1, j] + _elevation[i - 1, j]) - (current_flux + _elevation[i, j])) / _cellsize * velocity;
                    //    }
                    //}
                }
                //slope for j+1 cell
                if (j + 1 < cols)
                {

                    if (H[i, j] + _elevation[i, j] < H[i, j + 1] + _elevation[i, j + 1])
                    {
                        in_flux += FLUX[i, j + 1] / 2;
                    }

                    //if (_elevation[i, j] - _elevation[i, j + 1] < 0)
                    //{
                        //if (B[i, j + 1] > 0)
                        //{

                        //---
                        //--- From (Fiedler and Ramirez, 2000)
                        //---
                            //add flux from river and Pe

                       //add inflow from known heads
                        //if ((_elevation[i, j] + H[i, j]) - (_elevation[i, j+1] + H[i, j+1]) < 0 && H[i, j + 1] > 0 )
                    //if (H[i, j] + _elevation[i, j] < H[i, j + 1] + _elevation[i, j+1])
                    //    {
                    //        //add flux from Pe and Inflow
                    //        in_flux += FLUX[i, j + 1] / 2;
                    //        //subtract this flux from the higher node
                    //        //flows[i * cols + j + 1] -= FLUX[i, j + 1] / 2;

                    //        //--- add flux from cell stage 
                    //        //Get the available head, assuming that all of it can flow onto the current cell
                    //        double head_diff = Math.Abs(H[i, j] - H[i, j + 1]);
                    //        //convert the head difference into flux
                    //        double head_flux = head_diff * _cellsize / _dt;
                    //        //apply half of it to the current cell
                    //        ////in_flux += head_flux / 2;

                    //        //subtract this from the higher node
                    //        //FLUX[i, j + 1] -= head_flux / 2;
                        
                    //        //in_flux += B[i, j + 1] / _cellsize;

                    //        //double slope = (-1) * (_elevation[i, j] - _elevation[i, j + 1]) / _cellsize * 100;
                    //        //double velocity = GetVelocity(slope, ny);
                    //        //B[i, j] += ((B[i, j + 1] + _elevation[i, j + 1]) - (current_flux + _elevation[i, j])) / _cellsize * velocity;

                    //    }
                    //}
                }
                //slope for j-1 cell
                if (j - 1 >= 0)
                {
                    if (H[i, j] + _elevation[i, j] < H[i, j - 1] + _elevation[i, j - 1])
                    {
                        in_flux += FLUX[i, j - 1] / 2;
                    }

                    //if (_elevation[i, j] - _elevation[i, j - 1] < 0)
                    //{
                        //if (B[i, j - 1] > 0)
                        //{

                        //---
                        //--- From (Fiedler and Ramirez, 2000)
                        //---
                        //add inflow from known heads
                        //if ((_elevation[i, j] + H[i, j]) - (_elevation[i, j - 1] + H[i, j - 1]) < 0 && H[i, j - 1] > 0)
                    //if (H[i, j] + _elevation[i, j] < H[i, j - 1] + _elevation[i, j-1])
                    //    {
                    //        //add flux from Pe and Inflow
                    //        in_flux += FLUX[i, j - 1] / 2;
                    //        //subtract this flux from the higher node
                    //        //flows[i * cols + j-1] -= FLUX[i, j - 1] / 2;

                    //        //--- add flux from cell stage 
                    //        //Get the available head, assuming that all of it can flow onto the current cell
                    //        double head_diff = Math.Abs(H[i, j] - H[i, j - 1]);
                    //        //convert the head difference into flux
                    //        double head_flux = head_diff * _cellsize / _dt;
                    //        //apply half of it to the current cell
                    //        ////in_flux += head_flux / 2;

                    //        //subtract this from the higher node
                    //        //FLUX[i, j - 1] -= head_flux / 2;
                            

                    //    }
                            //in_flux += B[i, j - 1] / _cellsize;

                            //double slope = (-1) * (_elevation[i, j] - _elevation[i, j - 1]) / _cellsize * 100;
                            //double velocity = GetVelocity(slope, ny);
                            //B[i, j] += ((B[i, j - 1] + _elevation[i, j - 1]) - (current_flux + _elevation[i, j])) / _cellsize * velocity;
                        //}
                    //}
                }

               flows[i * cols + j] = FLUX[i,j] + in_flux;

                //Ignore Small InFluxes
                //if (current_flux + in_flux > 0.0001)
                    //B[i,j] is equal to the combination of fluxes into the cell
                    //flows[i * cols + j] = FLUX[i,j] + in_flux;
                //FLUX[i, j] = current_flux + in_flux;
                //else
                    //flows[i * cols + j] = FLUX[i, j];
                    //FLUX[i, j] = current_flux;

                //add flux from excess precip
                //B[i, j] += E[i, j] / _dt ;
            }
        }
        */
            #endregion
        }
        
    }
}
