using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Linq;

/////////////////////////////////////////////////////////////////////
//This file contains definitions for various discretization schemes//
/////////////////////////////////////////////////////////////////////

namespace DiffusiveWave.Source
{
    public interface Discretization
    {
        void CreateStiffness(double[] stage, double[] excess, double[] h, double[] b, int option);
        double[,] Sox { get; set; }
        double[,] Soy { get; set; }
        double Nx { get; set; }
        double Ny { get; set; }
        double Datum { get; set; }
        double[] Head { get; set; }
        double[,] A { get; set; }
        double[] q { get; set; }
    }

    public class Euler:Discretization
    {
        double _cellsize;
        double _dt;
        double _elementCount;
        double datum;
        public double nx { private get; set; }          //To be replaced with _nx[,]
        public double ny { private get; set; }          //To be replaced with _ny[,]
        //public double[,] _nx { private get; set; }
        //public double[,] _ny { private get; set; }

        private double[] head;
        public double[,] _sox { private get; set; }
        public double[,] _soy { private get; set; }
        public double[,] _elevation;
        private double[,] A;
        private double[] q;
        private double[,] H;
        double[,] Discretization.Sox
        {
            get
            {
               return _sox;
            }
            set
            {
                _sox = value;
            }
        }
        double[,] Discretization.Soy
        {
            get
            {
                return _soy;
            }
            set
            {
                _soy = value;
            }
        }
        double Discretization.Nx
        {
            get
            {
                return nx;
            }
            set
            {
                nx = value;
            }
        }
        double Discretization.Ny
        {
            get
            {
                return ny;
            }
            set
            {
                ny = value;
            }
        }
        double Discretization.Datum
        {
            get
            {
                return datum;
            }
            set
            {
                datum = value;
            }
        }
        double[] Discretization.Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }      
        double[,] Discretization.A { get { return this.A; } set { this.A = value; } }
        double[] Discretization.q { get { return this.q; } set { this.q = value; } }
        public Euler(double cellsize, double dt, int elementCount, double[,] elevation)
        {
            this._cellsize = cellsize;
            this._dt = dt;
            this._elementCount = elementCount;
            this._elevation = elevation;
            this.A = new double[elementCount, elementCount];
            this.q = new double[elementCount];
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
            int rows = _sox.GetLength(0);
            int cols = _soy.GetLength(1);

            //define some local variables
            double Cx1, Cx2, Cy1, Cy2;
            double dhdx, dhdy, dh2dx2, dh2dy2;

            H = new double[rows, cols];


            //HACK:  stage wont always exist at first element of row!!!  Should instead pass this method an array, where the
            //       stage values have already been assigned to specific locations within the matrix.

            //BROKEN: Why does this only iterate if option = 0????
            //set head in first column equal to the known stage
            //if (option == 0)
            //{
                //set the current head at each node equal to the known stage plus the head calculated in the last time step + plus the known excess
                for (int i = 0; i <= rows - 1; i++)
                {
                    for (int j = 0; j <= cols - 1; j++)
                    {
                        //set the initial head equal to the elevation minus the datum
                        //H[i, j] = _elevation[i, j] - datum;
                        //add the head from the previous time step
                        H[i, j] = h[j + (i * cols)]; //head[j + (i * cols)];
                        //H[i, j] = stage[j + (i * cols)] + head[j + (i * cols)] + excess[j + (i * cols)];   //HACK: Make sure this is correct!!!!!

                        //set H equal to the head from the previous timestep
                        //H[i, j] = head[j + (i * cols)];
                    }
                }
            //}


            //create source array based on the fluxes between adjacent cells
            q = CreateSource(inflow, excess);


            //---                            ---//
            //--- Formulate Stiffness Matrix ---//
            //---                            ---//
            System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\Temp\\Original.csv",true);
            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {
                    //--- Calculate the non-linear terms ---//

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
                        dhdx = (H[i, j+1] - H[i, j - 1]) / (2*_cellsize);
                        dh2dx2 = (H[i, j+1] + H[i, j - 1] - 2*H[i, j]) / (Math.Pow(_cellsize, 2));
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
                        dhdy = (H[i+1, j] - H[i - 1, j]) / (2*_cellsize);
                        dh2dy2 = (H[i+1, j] + H[i - 1, j] - 2*H[i, j]) / (Math.Pow(_cellsize, 2));
                    }

                    //--- SET TERMS IN THE STIFFNESS MATRIX A[,]---//

                    double XSlope, YSlope;

                    //CHECK THIS!!!: Which values of XSlope and YSlope should I use? 
                    //Using Central differences...
                    if (j == cols - 1)
                    {
                        //force slope out of grid by letting _elevation[i,j+1] = _elevation[i,j]-2
                        double nextelev = +_elevation[i, j] - 2;
                        XSlope = (_elevation[i, j - 1] - nextelev) / (2 * _cellsize);
                        //XSlope = (_elevation[i, j - 1] - _elevation[i, j]) / (2 * _cellsize);
                    }
                    else if (j == 0)
                        XSlope = (_elevation[i, j] - _elevation[i, j + 1]) / (2 * _cellsize);
                    else
                        XSlope = (_elevation[i, j - 1] - _elevation[i, j + 1]) / (2 * _cellsize);

                    if (i == rows - 1)
                    {
                        //force slope out of grid by letting _elevation[i+1,j] = _elevation[i,j]-2
                        double nextelev = +_elevation[i, j] - 2;
                        YSlope = (_elevation[i - 1, j] - nextelev) / (2 * _cellsize);
                        //YSlope = (_elevation[i - 1, j] - _elevation[i, j]) / (2 * _cellsize);
                    }
                    else if (i == 0)
                        YSlope = (_elevation[i, j] - _elevation[i + 1, j]) / (2 * _cellsize);
                    else
                        YSlope = (_elevation[i - 1, j] - _elevation[i + 1, j]) / (2 * _cellsize);


                    //set (i,j) term
                    A[i * cols + j, i * cols + j] = 1;

                    //set (i, j+1) term
                    if (j < cols - 1)
                    {
                        //XSlope = (_elevation[i, j] - _elevation[i, j + 1]) / _cellsize;
                        //dhdx = (H[i, j + 1] - H[i, j - 1]) / (2 * _cellsize);
                        Cx2 = (5.0 / (3.0 * nx)) * Math.Sign(XSlope) * Math.Sqrt(Math.Abs(XSlope - dhdx)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j + 1;//get the next column
                        A[row, col] = _dt / (2 * _cellsize) * Cx2;
                    }
                    //set (i,j-1) term
                    if (j - 1 >= 0)
                    {
                        //XSlope = (_elevation[i, j] - _elevation[i, j - 1]) / _cellsize;
                        Cx2 = (5.0 / (3.0 * nx)) * Math.Sign(XSlope) * Math.Sqrt(Math.Abs(XSlope - dhdx)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j - 1;//get the previous column
                        A[row, col] = -1 * _dt / (2 * _cellsize) * Cx2;
                    }
                    //set (i+1,j) term
                    if (i + 1 < rows - 1)
                    {
                        //YSlope = (_elevation[i, j] - _elevation[i + 1, j]) / _cellsize;
                        Cy2 = (5.0 / (3.0 * ny)) * Math.Sign(YSlope) * Math.Sqrt(Math.Abs(YSlope - dhdy)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j + cols;//get the column associated with the next row (current col + number of cols in elementset)
                        A[row, col] = _dt / (2 * _cellsize) * Cy2;
                    }
                    //set (i-1,j) term
                    if (i - 1 >= 0)
                    {
                        //YSlope = (_elevation[i, j] - _elevation[i - 1, j]) / _cellsize;
                        Cy2 = (5.0 / (3.0 * ny)) * Math.Sign(YSlope) * Math.Sqrt(Math.Abs(YSlope - dhdy)) * Math.Pow(H[i, j], 2.0 / 3.0);
                        if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }
                        int row = i * cols + j;//get the current row
                        int col = i * cols + j - cols;//get the column associated with the previous row (current col - number of cols in elementset)
                        A[row, col] = -1 * _dt / (2 * _cellsize) * Cy2;
                    }
                    for (int C = 0; C <=  A.GetLength(1)  - 1; C++)
                        sw.Write(A[i * cols + j, C].ToString() + ",");
                    sw.Write("\n");

                    Cx1 = (1.0 / (2.0 * nx)) * Math.Sign(XSlope) * Math.Pow(Math.Abs(XSlope - dhdx), -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (1.0 / (2.0 * ny)) * Math.Sign(YSlope) * Math.Pow(Math.Abs(YSlope - dhdy), -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    
                    

                    //catch infinity and nan terms
                    if (double.IsInfinity(Cx1) || double.IsNaN(Cx1)) { Cx1 = 0.0; }
                    if (double.IsInfinity(Cy1) || double.IsNaN(Cy1)) { Cy1 = 0.0; }


                    //---SET TERMS IN THE SOURCE ARRAY ---//
                    //multiply the flux (populated in Create Source) by dt
                    q[i * cols + j] *= this._dt;
                    //add the previous head term
                    q[i * cols + j] += head[i * cols + j] * (1 - (this._dt) * Cx1 - (this._dt) * Cy1);
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
            //TODO:
            //********
            //Store data in jagged array's for better performance.
            //This will also allow me to reduce the in-memory size of the data, but will require a "smarter" computing algorithm.
            //********

            //HACK:
            //********
            //Should I subtract the flux from the outgoing cell???
            //********

            //HACK:
            //*********
            //I think that I should be checking that the head (i.e. water height + elevation) is < 0 rather than just the elevations
            //*********

            //get rows and cols
            int rows = this._elevation.GetLength(0);
            int cols = this._elevation.GetLength(1);


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
            var sorted = from x in Enumerable.Range(0, _elevation.GetLength(0))
                         from y in Enumerable.Range(0, _elevation.GetLength(1))
                         select new
                         {
                             X = x,
                             Y = y,
                             Value = _elevation[x, y]
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
                    if (Head[i, j] + _elevation[i, j] > Head[i + 1, j] + _elevation[i + 1, j])
                    {
                        FLUX[i + 1, j] += (FLUX[i, j] / 2)/_cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }

                //slope for i-1 cell
                if (i - 1 >= 0)
                {

                    if (Head[i, j] + _elevation[i, j] > Head[i - 1, j] + _elevation[i - 1, j])
                    {
                        FLUX[i - 1, j] += (FLUX[i, j] / 2)/_cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }
                
                //slope for j+1 cell
                if (j + 1 < cols)
                {

                    if (Head[i, j] + _elevation[i, j] > Head[i, j + 1] + _elevation[i, j + 1])
                    {
                        FLUX[i, j + 1] += (FLUX[i, j] / 2) / _cellsize;
                        FLUX[i, j] -= (FLUX[i, j] / 2) / _cellsize;
                    }
                }

                //slope for j-1 cell
                if (j - 1 >= 0)
                {
                    if (Head[i, j] + _elevation[i, j] > Head[i, j - 1] + _elevation[i, j - 1])
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


        /// <summary>
        /// Flow velocity calculated by the upland method
        /// </summary>
        /// <param name="slope">slope between desired cells, in percent</param>
        /// <param name="n">mannings roughness coefficient</param>
        /// <returns>velocity in m3/s</returns>
        public double GetVelocity(double slope, double n)
        {
            double velocity = 0;

            //n ranges are interpolated from pg35 of (Chow et al 1988)
            //slope ranges are from the Upland Velocity Method Graph (National Engineering Handbook, 1972)
            if (slope < 0.5)
            {
                #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.1;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 0.2;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 0.3;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 0.4;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 0.7;
                //Paved
                else if (n <= 0.03)
                    velocity = 1;
                #endregion
            }
            else if (slope >= 0.5 && slope < 1)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.2;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 0.4;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 0.6;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 0.75;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 1.3;
                //Paved
                else if (n <= 0.03)
                    velocity = 1.7;
                #endregion
            }
            else if (slope >= 1 && slope < 2)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.3;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 0.6;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 0.8;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 1;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 1.7;
                //Paved
                else if (n <= 0.03)
                    velocity = 2.5;
                #endregion
            }
            else if (slope >= 2 && slope < 3)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.4;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 0.8;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.1;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 1.5;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 2.4;
                //Paved
                else if (n <= 0.03)
                    velocity = 3.1;
                #endregion
            }
            else if (slope >= 3 && slope < 4)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = .45;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = .92;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.4;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 1.6;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 2.8;
                //Paved
                else if (n <= 0.03)
                    velocity = 3.7;
                #endregion
            }
            else if (slope >= 4 && slope < 5)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = .52;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.1;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.5;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 1.9;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 2.1;
                //Paved
                else if (n <= 0.03)
                    velocity = 4.2;
                #endregion
            }
            else if (slope >= 5 && slope < 6)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.6;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.3;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.7;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 2.1;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 3.5;
                //Paved
                else if (n <= 0.03)
                    velocity = 4.7;
                #endregion
            }
            else if (slope >= 6 && slope < 7)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.64;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.4;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.9;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 2.25;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 3.9;
                //Paved
                else if (n <= 0.03)
                    velocity = 5.1;
                #endregion
            }
            else if (slope >= 7 && slope < 8)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.7;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.4;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 1.9;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 2.5;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 4.25;
                //Paved
                else if (n <= 0.03)
                    velocity = 5.5;
                #endregion
            }
            else if (slope >= 8 && slope < 9)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.75;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.5;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 2.1;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 2.6;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 4.5;
                //Paved
                else if (n <= 0.03)
                    velocity = 6;
                #endregion
            }
            else if (slope >= 9 && slope < 10)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.8;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.55;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 2.2;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 2.8;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 4.75;
                //Paved
                else if (n <= 0.03)
                    velocity = 6.2;
                #endregion
            }
            else if (slope >= 10 && slope < 20)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 0.9;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 1.9;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 2.8;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 3.5;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 5.6;
                //Paved
                else if (n <= 0.03)
                    velocity = 7.8;
                #endregion
            }
            else if (slope >= 20 && slope < 30)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 1.3;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 2.5;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 3.5;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 4.5;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 7.6;
                //Paved
                else if (n <= 0.03)
                    velocity = 10;
                #endregion
            }
            else if (slope >= 30 && slope < 40)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 1.6;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 3;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 4.2;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 5.5;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 9;
                //Paved
                else if (n <= 0.03)
                    velocity = 15;
                #endregion
            }
            else if (slope >= 40 && slope < 50)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 1.7;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 3.4;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 4.6;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 6.2;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 11;
                //Paved
                else if (n <= 0.03)
                    velocity = 17;
                #endregion
            }
            else if (slope >= 50 && slope < 60)
            {
            #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 1.8;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 3.8;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 5.1;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 6.8;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 13.5;
                //Paved
                else if (n <= 0.03)
                    velocity = 20;
                #endregion
            }
            else if (slope >= 60 && slope < 70)
            {
                #region Calculate Velocity
                //Dense Trees
                if (n > .085)
                    velocity = 2;
                //Dense Brush
                if (n <= 0.085 && n > 0.06)
                    velocity = 4.1;
                //Light Brush and Weeds
                else if (n <= 0.06 && n > 0.045)
                    velocity = 5.6;
                //Field Crops
                else if (n <= 0.045 && n > 0.037)
                    velocity = 7.3;
                //Pasture
                else if (n <= 0.037 && n > 0.03)
                    velocity = 15;
                //Paved
                else if (n <= 0.03)
                    velocity = 24;
                #endregion
            }

            //convert velocity from ft/s into m/s
            velocity /= 3.28;

            return velocity;

        }
    }


    //public class ForwardDifferencing : Discretization
    //{
    //    double _cellsize;
    //    double _dt;
    //    double _elementCount;
    //    double datum;
    //    public double nx { private get; set; }          //To be replaced with _nx[,]
    //    public double ny { private get; set; }          //To be replaced with _ny[,]
    //    private double[] head;
    //    //public double[,] _nx { private get; set; }
    //    //public double[,] _ny { private get; set; }
    //    public double[,] _sox { private get; set; }
    //    public double[,] _soy { private get; set; }

    //    double[,] Discretization.Sox
    //    {
    //        get
    //        {
    //            return _sox;
    //        }
    //        set
    //        {
    //            _sox = value;
    //        }
    //    }
    //    double[,] Discretization.Soy
    //    {
    //        get
    //        {
    //            return _soy;
    //        }
    //        set
    //        {
    //            _soy = value;
    //        }
    //    }
    //    double Discretization.Nx
    //    {
    //        get
    //        {
    //            return nx;
    //        }
    //        set
    //        {
    //            nx = value;
    //        }
    //    }
    //    double Discretization.Ny
    //    {
    //        get
    //        {
    //            return ny;
    //        }
    //        set
    //        {
    //            ny = value;
    //        }
    //    }
    //    double Discretization.Datum
    //    {
    //        get
    //        {
    //            return datum;
    //        }
    //        set
    //        {
    //            datum = value;
    //        }
    //    }
    //    double[] Discretization.Head
    //    {
    //        get
    //        {
    //            return head;
    //        }
    //        set
    //        {
    //            head = value;
    //        }
    //    }
    //    public ForwardDifferencing(double cellsize, double dt, int elementCount)
    //    {
    //        this._cellsize = cellsize;
    //        this._dt = dt;
    //        this._elementCount = elementCount;
    //    }

    //    /// <summary>
    //    /// Builds Stiffness Matrix
    //    /// </summary>
    //    /// <param name="stage">River Stage</param>
    //    /// <param name="h">Excess Head</param>
    //    /// <param name="b">Flow onto FloodPlain as a result of River Stage</param>
    //    /// <param name="A">Stiffness Matrix</param>
    //    /// <param name="q">Source Term</param>
    //    public void CreateStiffness(double[] stage, double[] excess, double[] h, double[] b, out double[,] A, out double[] q, int option)
    //    {
    //        //havent implemtented option yet!!!
    //        //get the number of rows and cols from Sox (assuming that Sox and Soy are of equal size)
    //        int rows = _sox.GetLength(0);
    //        int cols = _soy.GetLength(1);

    //        //define output variables
    //        A = new double[h.Length, h.Length];
    //        q = b;

    //        //define some local variables
    //        double Cx1, Cx2, Cy1, Cy2;
    //        double dhdx, dhdy, dh2dx2, dh2dy2;

    //        double[,] H = new double[rows, cols];
    //        //int eRow = (Int32)Math.Sqrt(h.Length);
    //        //int N = (Int32)Math.Sqrt(h.Length);


    //        //Convert h into 2d matrix
    //        for (int i = 0; i <= rows - 1; i++)
    //        {
    //            for (int j = 0; j <= cols - 1; j++)
    //            {
    //                H[i, j] = h[j + (i * cols)];
    //            }
    //        }

    //        //HACK:  stage wont always exist at first element of row!!!  Should instead pass this method an array, where the
    //        //       stage values have already been assigned to specific locations within the matrix.

    //        //set head in first column equal to the known stage
    //        for (int i = 0; i <= rows - 1; i++)
    //        {
    //            if (stage[i] > H[i, 0])
    //                H[i, 0] = stage[i];
    //        }

    //        //---                            ---//
    //        //--- Formulate Stiffness Matrix ---//
    //        //---                            ---//

    //        for (int i = 0; i <= rows - 1; i++)
    //        {
    //            for (int j = 0; j <= cols - 1; j++)
    //            {
    //                //--- Calculate the non-linear terms ---//

    //                //if this is the second to last column
    //                if (j == cols - 2)
    //                {
    //                    //forward differencing
    //                    dhdx = (H[i, j] - H[i, j + 1]) / (_cellsize);
    //                    dh2dx2 = (H[i, j] - 2 * H[i, j + 1] + 0) / (Math.Pow(_cellsize, 2));
    //                }
    //                //if this is the last column
    //                else if (j == cols - 1)
    //                {
    //                    //allow water to flow off grid, assuming the adjacent head is always zero
    //                    dhdx = (H[i, j] - 0) / (_cellsize);
    //                    dh2dx2 = (H[i, j] - 2 * 0 + 0) / (Math.Pow(_cellsize, 2));
    //                }
    //                //if this is an interior column
    //                else
    //                {
    //                    //forward differencing
    //                    dhdx = (H[i, j] - H[i, j + 1]) / (_cellsize);
    //                    dh2dx2 = (H[i, j] - 2 * H[i, j + 1] + H[i, j + 2]) / (Math.Pow(_cellsize, 2));
    //                }

    //                //if this is the second to last row
    //                if (i == rows - 2)
    //                {
    //                    //forward differencing
    //                    dhdy = (H[i, j] - H[i + 1, j]) / (_cellsize);
    //                    dh2dy2 = (H[i, j] - 2 * H[i + 1, j] + 0) / (Math.Pow(_cellsize, 2));
    //                }

    //                //if this is the last row
    //                else if (i == rows - 1)
    //                {
    //                    //forward differencing
    //                    dhdy = (H[i, j] - 0) / (_cellsize);
    //                    dh2dy2 = (H[i, j] - 2 * 0 + 0) / (Math.Pow(_cellsize, 2));
    //                }
    //                //if this is an interior row
    //                else
    //                {
    //                    //forward differencing
    //                    dhdy = (H[i, j] - H[i + 1, j]) / (_cellsize);
    //                    dh2dy2 = (H[i, j] - 2 * H[i + 1, j] + H[i + 2, j]) / (Math.Pow(_cellsize, 2));
    //                }


    //                //--- Calculate Coefficients ---//

    //                //transform from i, j into ii,jj for _sox and _soy arrays
    //                //int ii = _sox.GetLength(0) - _sox.GetLength(0) % (i+1);
    //                //int jj = _sox.GetLength(1) - _sox.GetLength(1) % (j+1);

    //                double Xslope = Math.Abs(_sox[i, j]);
    //                double Yslope = Math.Abs(_soy[i, j]);


    //                Cx1 = (1.0 / (2.0 * nx)) * Math.Sign(_sox[i, j]) * Math.Pow(Math.Abs(Xslope - dhdx), -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
    //                Cy1 = (1.0 / (2.0 * ny)) * Math.Sign(_soy[i, j]) * Math.Pow(Math.Abs(Yslope - dhdy), -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
    //                Cx2 = (5.0 / (3.0 * nx)) * Math.Sign(_sox[i, j]) * Math.Sqrt(Math.Abs(Xslope - dhdx)) * Math.Pow(H[i, j], 2.0 / 3.0);
    //                Cy2 = (5.0 / (3.0 * ny)) * Math.Sign(_soy[i, j]) * Math.Sqrt(Math.Abs(Yslope - dhdy)) * Math.Pow(H[i, j], 2.0 / 3.0);

    //                //catch infinity and nan terms
    //                if (double.IsInfinity(Cx1) || double.IsNaN(Cx1)) { Cx1 = 0.0; }
    //                if (double.IsInfinity(Cy1) || double.IsNaN(Cy1)) { Cy1 = 0.0; }
    //                if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
    //                if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }

 
    //                //--- SET TERMS IN THE STIFFNESS MATRIX A[,]---//

    //                //set (i,j) term
    //                A[i * cols + j, i * cols + j] = 1 + (this._dt / _cellsize) * Cx2 + (this._dt / _cellsize) * Cy2;

    //                //set(i,j+1) term if its not the last column
    //                if (j < cols - 2)
    //                    A[i * cols + j, i * cols + j + 1] = -1 * (this._dt / _cellsize) * Cx2;

    //                //set(i+1,j) term if its not the last row
    //                if (i < rows - 2)
    //                    A[(i+1) * cols + j, i * cols + j] = -1 * (this._dt / _cellsize) * Cy2;

    //                //---SET TERMS IN THE SOURCE ARRAY ---//

    //                //set(i,j) term 
    //                q[i * cols + j] *= this._dt / 3600;     //multiply by -1 to put the flow in the correct direction
    //                q[i * cols + j] += (this._dt / 3600) * (1 - Cx1 - Cy1);


    //            }
    //        }

    //        //DEBUG:  this is to make sure the matrix is in the correct form
    //        for (int i = 0; i <= A.GetLength(0) - 1; i++)
    //        {
    //            for (int j = 0; j <= A.GetLength(1) - 1; j++)
    //            {
    //                Debug.Write(Math.Round(A[i, j],0).ToString() + "  ");
    //            }
    //            Debug.Write("\n");
    //        }

    //    }


    //}
}
