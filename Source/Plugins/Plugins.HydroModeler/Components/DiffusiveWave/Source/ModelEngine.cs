using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMW;
using System.Collections;
using Oatc.OpenMI.Sdk.Backbone;
using System.Diagnostics;




////////////////////////////////////////////////
//
//  Original Author:    Anthony Castronova, University of South Carolina, Columbia SC
//  Created On:         July 21st 2010
//  Version:            1.0.0
//
//  Component Name:     2D Diffusive Wave
//  Inputs:             Configuration.xml
//  Purpose:            Perform 2D surface routing using the diffusive wave approximation.
//  Methodology:        Implements the diffusive wave approximation for use in surface flow routing                        
//                      in flood inundation studies.
//
//  Additional Resources   
//  --------------------
//  Modification History:
//  10/05/10: Changed the discretization scheme to Forward Differencing.  This should by changed to Crank-Nicholson or Central Diff later.
//  10/07/10: Added simple ElementSet creating routing that uses ASCII raster input 
//
////////////////////////////////////////////////


namespace DiffusiveWave
{
    public class ModelEngine:SMW.Wrapper
    {
        #region Global Variables
        double[,] values;
        double[] h;
        double[] h1;
        //double[] q;

        public double dt;
        string _inStageElementSet, _inStageQuantity,
               _inExcessElementSet, _inExcessQuantity,
               _outExcessElementSet, _outExcessQuantity;
        private int elements;

        //HACK:
        public double cellsize = 10;
        public int rows = 10;
        public int cols = 10;
        public double nx = 0.001;
        public double ny = 0.001;
        public double sox = 0.1;
        public double soy = 0.1;
        public double hw = 0.1; //HACK: need to set this somewhere else [weir height]
        public double[,] _soy;
        public double[,] _sox;

        double Cx1, Cx2, Cx3, Cy1, Cy2, Cy3;
        double dhdx, dhdy, dh2dx2, dh2dy2;
        #endregion

        public ModelEngine()
        {
        }

        public override void Initialize(System.Collections.Hashtable properties)
        {
            string config = null;

            //read input arguments
            foreach (DictionaryEntry arg in properties)
            {
                switch (arg.Key.ToString())
                {
                    case "ConfigFile":
                        config = arg.Value.ToString();
                        break;

                }
            }

            //setup model properties
            this.SetValuesTableFields();
            this.SetVariablesFromConfigFile(config);

            //get model information
            InputExchangeItem input = this.GetInputExchangeItem(0);
            _inExcessElementSet = input.ElementSet.ID;
            _inExcessQuantity = input.Quantity.ID;

            input = this.GetInputExchangeItem(1);
            _inStageElementSet = input.ElementSet.ID;
            _inStageQuantity = input.Quantity.ID;

            dt = this.GetTimeStep()/3600;

            OutputExchangeItem output = this.GetOutputExchangeItem(0);
            _outExcessElementSet = output.ElementSet.ID;
            _outExcessQuantity = output.Quantity.ID;



            //DEBUG: manually define element set
            CreateElementSet(rows, cols);

            //determine the number of elements
            elements = rows * cols;

            //create array to hold head
            h = new double[elements];
            
            //TODO: Define these from inputs
            //nx = new double[elements];
            //ny = new double[elements];
            //sox = new double[elements]; 
            //soy = new double[elements]; 

        }

        public override bool PerformTimeStep()
        {
            //get inputs
            ScalarSet inStage = (ScalarSet)this.GetValues(_inStageQuantity, _inStageElementSet);
            double[] stage = inStage.data;
            ScalarSet inExcess = (ScalarSet)this.GetValues(_inExcessQuantity,_inExcessElementSet);
            double[] excess = inExcess.data;
            
            
            //transform stage into flow
            double[] flow = Stage2Flow(stage);

            //assign excess to h
            if (excess.Length != h.Length)
                excess = new double[h.Length];
            for (int i = 0; i <= h.Length - 1; i++)
                h[i] += excess[i];

            //transform excess into flux
            //flow = Excess2Flow(flow, excess);

            //set cell flows based on stage
            //double[] b = Stage2CellFlow(flow);
            double[] b = flow;

            //--- Populate Stiffness Matrix ---
            double[] q;
            double[,] A;
            CreateStiffness(stage, h, h, b,out A, out q);

            //--- Perform SOR to get first approx of H ---
            h1 = SuccessiveOverRelaxation(A, q);

            //--- Re-Populate Stiffness Matrix ---
            //b = q;
            //double[] s = new double[stage.Length];
            CreateStiffness(stage, h1,h, b, out A, out q);

            double[] H = SuccessiveOverRelaxation(A, q);

            //--- Re-Populate Stiffness Matrix ---
            //b = q;
            ////double[] s = new double[stage.Length];
            //CreateStiffness(stage, h1, b, out A, out q);

            ////A = CreateStiffness(h, q);

            ////--- Perform SOR to get second approx of H ---
            //double[] H = SuccessiveOverRelaxation(A, q);

            //save these head values for the next time step
            h = H;

            this.SetValues(_outExcessQuantity, _outExcessElementSet, new ScalarSet(H));


            return true;
        }



        private void CreateStiffness(double[] stage, double[] h, double[] h_old, double[] b, out double[,] A, out double[] q)
        {
            //int e = (Int32)Math.Pow(h.Length, 2);
            A = new double[h.Length, h.Length];
            q = b;

            //double[,] A = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            double[,] H = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            double[,] H_old = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            int eRow = (Int32)Math.Sqrt(h.Length);
            int N = (Int32)Math.Sqrt(h.Length);


            #region convert h into 2d matrix
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length) - 1; i++)
            {
                for (int j = 0; j <= (Int32)Math.Sqrt(h.Length) - 1; j++)
                {
                    H[i, j] = h[j + (i * (Int32)Math.Sqrt(h.Length))];
                }
            }
            #endregion

            //HACK:
            //set head in first column equal to the known stage
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length) - 1; i++)
            {
                if(stage[i] > H[i, 0])
                    H[i, 0] = stage[i];
            }

            #region Formulate Stiffness Matrix

            for (int i = 0; i <= H.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= H.GetLength(1) - 1; j++)
                {
                    //---                                ---//
                    //--- Calculate the non-linear terms ---//
                    //---                                ---//

                    ////if this is the first column
                    //if (j == 0)
                    //{
                    //    //h(i-1) == h(i)
                    //    //dhdx = (H[i, j + 1] - H[i, j]) / (2 * cellsize);
                    //    //dh2dx2 = (H[i, j + 1] - H[i, j]) / (Math.Pow(cellsize, 2));

                    //    //forward differencing
                    //    dhdx = (H[i, j] - H[i, j+1]) / (2 * cellsize);
                    //    dh2dx2 = (H[i, j] - 2 * H[i, j + 1] + H[i, j + 2]) / (Math.Pow(cellsize, 2));
                    //}
                    //else
                    
                    //if this is the second to last column
                    if (j == H.GetLength(1) - 2)
                    {
                        //forward differencing
                        dhdx = (H[i, j] - H[i, j + 1]) / (cellsize);
                        dh2dx2 = (H[i, j] - 2 * H[i, j + 1] + 0) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        //h(i+1) = h(i)
                        //dhdx = (-1 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        //dh2dx2 = (-1 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));

                        //allow water to flow off grid, assuming the adjacent head is always zero
                        dhdx = (H[i, j] - 0) / (cellsize);
                        dh2dx2 = (H[i, j] - 2 * 0 + 0) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior column
                    else
                    {
                        //dhdx = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        //dh2dx2 = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));

                        //forward differencing
                        dhdx = (H[i, j] - H[i, j + 1]) / (cellsize);
                        dh2dx2 = (H[i, j] - 2 * H[i, j + 1] + H[i, j + 2]) / (Math.Pow(cellsize, 2));
                    }

                    ////if this the first row
                    //if (i == 0)
                    //{
                    //    //h(j-1) = h(j)
                    //    dhdy = (H[i + 1, j] - H[i, j]) / (2 * cellsize);
                    //    dh2dy2 = (H[i + 1, j] - H[i, j]) / (Math.Pow(cellsize, 2));
                    //}

                    //if this is the second to last row
                    if (i == H.GetLength(0) - 2)
                    {
                        ////h(j+1) = h(j)
                        //dhdy = (-1 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        //dh2dy2 = (-1 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));

                        //forward differencing
                        dhdy = (H[i, j] - H[i+1, j]) / (cellsize);
                        dh2dy2 = (H[i, j] - 2 * H[i+1, j] + 0) / (Math.Pow(cellsize, 2));
                    }

                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        ////h(j+1) = h(j)
                        //dhdy = (-1 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        //dh2dy2 = (-1 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));

                        //forward differencing
                        dhdy = (H[i, j] - 0) / (cellsize);
                        dh2dy2 = (H[i, j] - 2 * 0 + 0) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior row
                    else
                    {
                        //dhdy = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        //dh2dy2 = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));

                        //forward differencing
                        dhdy = (H[i, j] - H[i + 1, j]) / (cellsize);
                        dh2dy2 = (H[i, j] - 2 * H[i + 1, j] + H[i + 2, j]) / (Math.Pow(cellsize, 2));
                    }


                    //---
                    //--- Calculate Coefficients ---
                    //---
                    
                    Cx1 = (1.0 / (2.0*nx)) * Math.Pow(sox - dhdx, -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (1.0 / (2.0*ny)) * Math.Pow(soy - dhdy, -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cx2 = (5.0 / (3.0*nx)) * Math.Sqrt(sox - dhdx) * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy2 = (5.0 / (3.0*ny)) * Math.Sqrt(soy - dhdy) * Math.Pow(H[i, j], 2.0 / 3.0);

                    //catch infinity and nan terms
                    if (double.IsInfinity(Cx1) || double.IsNaN(Cx1)) { Cx1 = 0.0; }
                    if (double.IsInfinity(Cy1) || double.IsNaN(Cy1)) { Cy1 = 0.0; }
                    if (double.IsInfinity(Cx2) || double.IsNaN(Cx2)) { Cx2 = 0.0; }
                    if (double.IsInfinity(Cy2) || double.IsNaN(Cy2)) { Cy2 = 0.0; }

                    //---
                    //--- Add coefficients to stiffness matrix
                    //---

                    #region old
                    ////last column and last row
                    //if (i == H.GetLength(0) - 1 && j == H.GetLength(1) - 1)
                    //{
                    //    //(i,j)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (2 * Cx1 / (Math.Pow(cellsize, 2))) - (Cx2 / cellsize) + (2 * Cy1 / (Math.Pow(cellsize, 2))) - (Cy2 / cellsize);
                    //    //(i,j-1)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j - 1)] = (Cx2 / (2 * cellsize)) - (Cx1 / Math.Pow(cellsize, 2));


                        
                    //}
                    ////last column
                    //else if (j == H.GetLength(1) - 1)
                    //{
                    //    //(i,j)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (2 * Cx1 / (Math.Pow(cellsize, 2))) - (Cx2 / cellsize) + (2 * Cy1 / (Math.Pow(cellsize, 2))) - (Cy2 / cellsize);
                    //    //(i,j-1)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j - 1)] = (Cx2 / (2 * cellsize)) - (Cx1 / Math.Pow(cellsize, 2));

                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1;
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j - 1)] = 1;
                    //}
                    ////first column
                    //else if (i * H.GetLength(0) + j == 0)
                    //{
                    //    //(i,j)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (2 * Cx1 / (Math.Pow(cellsize, 2))) - (Cx2 / cellsize) + (2 * Cy1 / (Math.Pow(cellsize, 2))) - (Cy2 / cellsize);
                    //    //(i,j-1)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = (Cx2 / (2 * cellsize)) - (Cx1 / Math.Pow(cellsize, 2));

                    //    ////h(x)
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1;
                    //    ////h(x+1)
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = 1;
                    //}
                    ////interior element
                    //else
                    //{
                    //    //(i,j)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (2*Cx1 / (Math.Pow(cellsize,2))) - (Cx2 / cellsize) + (2*Cy1 / (Math.Pow(cellsize,2))) - (Cy2/cellsize);
                    //    //(i,j+1)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = (Cx2 / (2*cellsize)) - (Cx1 / Math.Pow(cellsize,2));
                    //    //(i,j-1)
                    //    A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j - 1)] = (Cx2 / (2 * cellsize)) - (Cx1 / Math.Pow(cellsize, 2));

                    //    ////h(x)
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1;
                    //    ////h(x+1)
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = 1;
                    //    ////h(x-1)
                    //    //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j - 1)] = 1;
                    //}



                    ////add j+1 and j-1 terms
                    //if ((i * H.GetLength(0) + j) - N >= 0)
                    //{
                    //    //(i-1,j)
                    //    A[i * H.GetLength(0) + j, (i * H.GetLength(0) + j) - N] = (Cy2 / (2*cellsize)) - (Cy1/(Math.Pow(cellsize,2)));
                    //    //A[i * H.GetLength(0) + j, (i * H.GetLength(0) + j) - N] = 1;
                    //}
                    //if ((i * H.GetLength(0) + j) + N <= h.Length - 1)
                    //{
                    //    //(i+1,j)
                    //    A[i * H.GetLength(0) + j, (i * H.GetLength(0) + j) + N] = (Cy2 / (2 * cellsize)) - (Cy1 / (Math.Pow(cellsize, 2)));
                    //    //A[i * H.GetLength(0) + j, (i * H.GetLength(0) + j) + N] = 1;
                    //}

                    #endregion


                    //--------------------- SET TERMS IN THE STIFFNESS MATRIX A[,]--------------------------
                    //---
                    //--- set (i,j) term
                    //---
                    A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (this.dt / cellsize) * Cx2 + (this.dt / cellsize) * Cy2;

                    //---
                    //--- set(i,j+1) term if its not the last column
                    //---
                    if (j < H.GetLength(1) - 1)
                    {
                        //set (i,j+1)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + j + 1] =  -1 * (this.dt / cellsize) * Cx2;
                    }

                    //---
                    //--- set(i+1,j) term if its not the last row
                    //---
                    if ((i * H.GetLength(0) + j) + N < h.Length - 1)
                    {
                        //(i+1,j)
                        A[i * H.GetLength(0) + j, (i * H.GetLength(0) + j) + N] = -1 * (this.dt / cellsize)* Cy2;
                    }

                    //------------------ SET TERMS IN THE SOURCE ARRAY-----------------
                    //set(i,j) term 
                    q[i * H.GetLength(0) + j] *= this.dt;     //multiply by -1 to put the flow in the correct direction
                    q[i * H.GetLength(0) + j] += this.dt*(1 - Cx1 - Cy1);
                    //q[i * H.GetLength(0) + j] += H_old[i, j];// H[i, j];


                }
            }
            #endregion


            //DEBUG:  this is to make sure the matrix is in the corrent form
            for (int i = 0; i <= A.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= A.GetLength(1) - 1; j++)
                {
                    Debug.Write(A[i, j].ToString() + " ");
                }
                Debug.Write("\n");
            }

            //return A, q;
        }

        private double[,] CreateStiffness(double[] h, double[] q)
        {
            //int e = (Int32)Math.Pow(h.Length, 2);
            double[,] A = new double[h.Length, h.Length];
            //double[,] A = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            double Dt = this.dt / 3600;
            double[,] H = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            int eRow = (Int32)Math.Sqrt(h.Length);

            #region convert h into 2d matrix
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length) - 1; i++)
            {
                for (int j = 0; j <= (Int32)Math.Sqrt(h.Length) - 1; j++)
                {
                    H[i, j] = h[j + (i * (Int32)Math.Sqrt(h.Length))];
                }
            }
            #endregion

            #region Formulate Stiffness Matrix

            for (int i = 0; i <= H.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= H.GetLength(1) - 1; j++)
                {
                    //---                                ---//
                    //--- Calculate the non-linear terms ---//
                    //---                                ---//

                    //if this is the first column
                    if (j == 0)
                    {
                        //h(i-1) == h(i)
                        dhdx = (H[i, j + 1] - H[i, j]) / (2 * cellsize);
                        dh2dx2 = (H[i, j + 1] - H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        //h(i+1) = h(i)
                        dhdx = (-1 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        dh2dx2 = (-1 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior column
                    else
                    {
                        dhdx = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        dh2dx2 = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this the first row
                    if (i == 0)
                    {
                        //h(j-1) = h(j)
                        dhdy = (H[i + 1, j] - H[i, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] - H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //h(j+1) = h(j)
                        dhdy = (-1 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (-1 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior row
                    else
                    {
                        dhdy = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }


                    //---
                    //--- Calculate Coefficients ---
                    //---

                    //if this is the first column
                    if (j == 0)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j + 1], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this is an interior column
                    else
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j + 1], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this the first row
                    if (i == 0)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    //if this is an interior row
                    else
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    Cx1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(sox - dhdx, -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(soy - dhdy, -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cx3 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy3 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * Math.Pow(H[i, j], 2.0 / 3.0);

                    //---
                    //--- Add coefficients to stiffness matrix
                    //---

                    //last column and last row
                    if (i == H.GetLength(0) - 1 && j == H.GetLength(1) - 1)
                    {
                        //h(x)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (Cx3 / cellsize) + (Cy3 / cellsize);

                    }
                    //last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        //h(x)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = 0;
                        //h(y+1)
                        A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;
                    }
                    //last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //h(x)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;
                    }
                    //interior element
                    else
                    {
                        //h(x)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;
                        //h(y+1)
                        A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;
                    }


                    //HACK: SHOULD THIS ADD OR REPLACE (THE SECOND TIME AROUND)
                    q[i * H.GetLength(0) + j] += (-1) * (Cx1 + Cx2 + Cy1 + Cy2);
                }
            }
            #endregion
            return A;
        }







        /// <summary>
        /// Converts excess into flux 
        /// </summary>
        /// <param name="flow">flux array</param>
        /// <param name="excess">excess rainfall</param>
        /// <returns></returns>
        private double[] Excess2Flow(double[] flow, double[] excess)
        {
            //assume broad crested weir
            double[] Q = new double[elements];

            for (int i = 0; i <= Q.Length - 1; i++)
            {
                //convert excess into volume per timestep
                flow[i] += excess[i] * Math.Pow(cellsize, 2) / dt;
   

            }
            return flow;
        }

        /// <summary>
        /// This method calculates the flow rates between cells.
        /// </summary>
        /// <param name="Q">flow rate at cells adjacent to river</param>
        /// <returns></returns>
        private double[] Stage2CellFlow(double[] Q)
        {


            int n = (Int32)Math.Sqrt(Q.Length);



            //--- Assuming only lateral flow (x dir only) ---
            int col = 0;
            for (int i = 1; i < Q.Length - 1; i++)
            {
                Q[i] += Q[i-1];
                //get the head based on the inflow of water from the adjacent river
                //double h1 = h[i] + (Q[i] / Math.Pow(cellsize, 2)) * (dt);

                col++;
                if (col == Math.Sqrt(Q.Length) - 1)
                {
                    i++;
                    col = 0;
                }
            }

            //for (int i = 0; i <= n - 1; i++)
            //{
            //    for (int j = 0; j <= n - 2; j++)
            //        Q[n * j + n + i] += Q[0];
            //}
            return Q;
            
        }

        public override void Finish()
        {
        }
        private void CreateElementSet(int rows, int cols)
        {

            //Get the input and output element sets from the SMW
            ElementSet out_elem = (ElementSet)this.Outputs[0].ElementSet;
            ElementSet in_elem = (ElementSet)this.Inputs[0].ElementSet;

            //Set some ElementSet properties
            out_elem.ElementType = OpenMI.Standard.ElementType.XYPoint;
            in_elem.ElementType = OpenMI.Standard.ElementType.XYPoint;


            //Create elements
            for (int i = 0; i <= rows-1; i++)
            {
                //add these elements to the output element set
                Element e = new Element();
                Vertex v = new Vertex(0, i, 0);
                e.AddVertex(v);
                in_elem.AddElement(e);

                for (int j = 0; j <= cols - 1; j++)
                {
                    //add these elements to the output element set
                    e = new Element();
                    v = new Vertex(i, j, 0);
                    e.AddVertex(v);
                    out_elem.AddElement(e);

                }
            }

            //initialize values matrix
            values = new double[rows,cols];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="option">1 for mixed implicit and explicit rep. 2 for completely implicity rep.</param>
        /// <returns></returns>
        private double[,] PopulateDiffusiveMatrix(int elements, int option)
        {
            double[,] A = new double[elements,elements];
            double Dt = this.dt / 3600;
            #region Implicit with Non-Linear Explicit Terms
            if (option == 1)
            {
                for (int i = 0; i <= elements - 1; i++)
                {
                    A[i, i] += (1.0 - h[i]) / Dt;

                    //-- Is this this last column? --
                    if ((i+1) % Math.Sqrt(elements) == 0)
                    {
                        //use backward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                * Math.Sqrt((1.0 / nx) * (sox - ((h[i] - h[i - 1]) / cellsize))) 
                                * Math.Pow((h[i] - h[i-1]), (2 / 3));
                        
                        //-- Is this the very last element? --
                        if (i == elements - 1)
                        {
                            //use backward differencing in Y
                            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                        * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
                                        * Math.Pow((h[rows] - h[i - rows]), (2 / 3));
                        }
                        else
                        {
                            //use forward differencing in Y
                            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                        * Math.Sqrt((1.0 / ny) * (soy - ((h[i+rows] - h[i]) / cellsize)))
                                        * Math.Pow((h[2 * rows] - h[rows]), (2 / 3));
                        }
                    }
                    //-- Is this the last row? --
                    else if ((i+1) >= (elements - (elements/rows)))
                    {
                        //use forward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i + 1] - h[i]) / cellsize))) 
                                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));

                        //use backward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
                                    * Math.Pow((h[rows] - h[i-rows]), (2 / 3));
                    }
                    else
                    {
                        //use forward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i + 1] - h[i]) / cellsize))) 
                                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));
                        //use forward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i + rows] - h[i]) / cellsize)))
                                    * Math.Pow((h[i + rows] - h[rows]), (2 / 3));
                    }
                }


            }
            #endregion

            #region Implict with Updated Head
            else if (option == 2)
            {
                for (int i = 0; i <= elements - 1; i++)
                {
                    A[i, i] += (1.0 - h[i]) / Dt;

                    //-- Is this this last column? --
                    if ((i + 1) % Math.Sqrt(elements) == 0)
                    {
                        //use backward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                * Math.Sqrt((1.0 / nx) * (sox - ((h1[i] - h1[i - 1]) / cellsize))) 
                                * Math.Pow((h1[i] - h1[i - 1]), (2 / 3));

                        //-- Is this the very last element? --
                        if (i == elements - 1)
                        {
                            //use backward differencing in Y
                            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                        * Math.Sqrt((1.0 / ny) * (soy - ((h1[i] - h1[i - rows]) / cellsize)))
                                        * Math.Pow((h1[rows] - h1[i - rows]), (2 / 3));
                        }
                        else
                        {
                            //use forward differencing in Y
                            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                        * Math.Sqrt((1.0 / ny) * (soy - ((h1[i + rows] - h1[i]) / cellsize)))
                                        * Math.Pow((h1[2 * rows] - h1[rows]), (2 / 3));
                        }
                    }
                    //-- Is this the last row? --
                    else if ((i + 1) >= (elements - (elements / rows)))
                    {
                        //use forward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h1[i + 1] - h1[i]) / cellsize))) 
                                    * Math.Pow((h1[i + 1] - h1[i]), (2 / 3));

                        //use backward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h1[i] - h1[i - rows]) / cellsize)))
                                    * Math.Pow((h1[rows] - h1[i - rows]), (2 / 3));
                    }
                    else
                    {
                        //use forward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h1[i + 1] - h1[i]) / cellsize))) 
                                    * Math.Pow((h1[i + 1] - h1[i]), (2 / 3));
                        //use backward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h1[i + rows] - h1[i]) / cellsize)))
                                    * Math.Pow((h1[i + rows] - h1[i]), (2 / 3));
                    }
                }
            }
            #endregion

            #region Experimental
            //experimental 
            else if (option == 3)
            {
                for (int i = 0; i <= elements - 1; i++)
                {
                    A[i, i] += (1.0 - h[i]) / Dt;

                    //if its the first element (i.e. next to the river) [ First Column ]
                    if (i % (elements / rows) == 0)
                    {
                        //use forward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i + 1] - h[i]) / cellsize))) 
                                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));
                        //use forward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i + rows] - h[rows]) / cellsize)))
                                    * Math.Pow((h[i + rows] - h[rows]), (2 / 3));
                    }
                    //[First Row, Not first Column]
                    else if (i <= (elements / rows))
                    {
                        //use backward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i] - h[i - 1]) / cellsize)))
                                    * Math.Pow((h[i] - h[i - 1]), (2 / 3));

                        //use forward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i + rows] - h[i]) / cellsize)))
                                    * Math.Pow((h[i + rows] - h[i]), (2 / 3));
                    }
                    //-- Is this this last column? --
                    else if ((i + 1) % Math.Sqrt(elements) == 0)
                    {
                        //use backward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                * Math.Sqrt((1.0 / nx) * (sox - ((h[i] - h[i - 1]) / cellsize))) 
                                * Math.Pow((h[i] - h[i - 1]), (2 / 3));

                        //use backward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
                                    * Math.Pow((h[i] - h[i - rows]), (2 / 3));

                    }
                    //if its an interior element
                    else
                    {
                        //use backward differencing in X
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i] - h[i - 1]) / cellsize))) 
                                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));
                        //use backward differencing in Y
                        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
                                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
                                    * Math.Pow((h[i] - h[i - rows]), (2 / 3));
                    }
                }
            }
            #endregion

            return A;
        }

        private double[,] PopulateA(double[] stage, double[] h)
        {
            //int e = (Int32)Math.Pow(h.Length, 2);
            //double[,] A = new double[h.Length, h.Length];
            double[,] A = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            double Dt = this.dt / 3600;
            double[,] H = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            int eRow = (Int32)Math.Sqrt(h.Length);
            
            #region convert h into 2d matrix
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length)-1; i++)
            {
                for (int j = 0; j <= (Int32)Math.Sqrt(h.Length) - 1; j++)
                {
                    H[i, j] = h[j + (i * (Int32)Math.Sqrt(h.Length))];
                }
            }
            #endregion

            //HACK:
            //set head in first column equal to the known stage
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length) - 1; i++)
            {
                H[i, 0] = stage[i];
            }

            #region Formulate Stiffness Matrix
            double Cx1, Cx2, Cx3, Cy1, Cy2, Cy3;
            double dhdx, dhdy, dh2dx2, dh2dy2;
            for (int i = 0; i <= H.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= H.GetLength(1) - 1; j++)
                {
                    //---                                ---//
                    //--- Calculate the non-linear terms ---//
                    //---                                ---//

                    //if this is the first column
                    if (j == 0)
                    {
                        //h(i-1) == h(i)
                        dhdx = (H[i, j + 1] - H[i, j] ) / (2 * cellsize);
                        dh2dx2 = (H[i, j + 1] - H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        //h(i+1) = h(i)
                        dhdx = (-1 * H[i, j] + H[i , j - 1]) / (2 * cellsize);
                        dh2dx2 = (-1 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior column
                    else
                    {
                        dhdx = (H[i, j + 1] - 2*H[i, j] + H[i,j - 1]) / (2*cellsize);
                        dh2dx2 = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this the first row
                    if (i == 0)
                    {
                        //h(j-1) = h(j)
                        dhdy = (H[i + 1, j] - H[i, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] -H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //h(j+1) = h(j)
                        dhdy = (-1 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (-1 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior row
                    else
                    {
                        dhdy = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }


                    //---
                    //--- Calculate Coefficients ---
                    //---

                    //if this is the first column
                    if (j == 0)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j+1], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdy;
                        
                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this is an interior column
                    else
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j+1], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;
                    
                    //if this the first row
                    if (i == 0)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdx;
                    
                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;
                    
                    //if this is an interior row
                    else
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;
                    
                    Cx1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(sox - dhdx, -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(soy - dhdy, -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cx3 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy3 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * Math.Pow(H[i, j], 2.0 / 3.0);

                    //---
                    //--- Add coefficients to stiffness matrix
                    //---

                    //last column and last row
                    if (i == H.GetLength(0) - 1 && j == H.GetLength(1) - 1)
                    {
                        //h(x)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);\
                        A[i, j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                    }
                    //last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        ////h(x)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        ////h(x+1)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = 0;
                        ////h(y+1)
                        //A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;

                        //h(x)
                        A[i,j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        //A[i,(j + 1)] = 0;
                        //h(y+1)
                        A[i+1,j] += -Cy3 / cellsize;
                    }
                    //last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //////h(x)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //////h(x+1)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;

                        //h(x)
                        A[i,j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i,(j + 1)] += -Cx3 / cellsize;
                    } 
                    //interior element
                    else
                    {
                        //////h(x)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //////h(x+1)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;
                        //////h(y+1)
                        ////A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;

                        //h(x)
                        A[i,j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i,(j + 1)] += -Cx3 / cellsize;
                        //h(y+1)
                        A[i+ 1,j] += -Cy3 / cellsize;
                    }
                }
            }
            #endregion

            #region set boundary elements

            //for (int i = 0; i <= H.GetLength(0) - 3; i++)
            //{
            //    A[i, H.GetLength(0) - 2] = A[i, H.GetLength(0) - 3];
            //    A[i, H.GetLength(0) - 1] = A[i, H.GetLength(0) - 2];
            //}
            //for (int j = 0; j <= H.GetLength(0) - 3; j++)
            //{
            //    A[H.GetLength(1) - 2,j] = A[H.GetLength(1) - 3,j];
            //    A[H.GetLength(1) - 1, j] = A[H.GetLength(1) - 2, j];
            //}

            #endregion


            #region HACK: Old Stuff
            //////get h(i), nx(i), ny(i), Sox(i), Soy(i)
                    ////double hi = h[j + i * eRow];
                    ////double sy = soy;
                    ////double sx = sox;
                    ////double nX = nx;
                    ////double nY = ny;

                    //////first row
                    ////if (i == 0)
                    ////{
                    ////    A[i,j] += 
                    ////}
                    //////last row
                    ////else if (i == eRow-1)
                    ////{
                    ////}
                    //////interior rows
                    ////else
                    ////{
                    ////}

                    //////first col
                    ////if (j == 0)
                    ////{
                    ////}
                    //////last col
                    ////else if (j == eRow-1)
                    ////{
                    ////}
                    //////interior row
                    ////else
                    ////{
                    ////}

#endregion 



            #region Implict Scheme with Explicit Non-Linear Terms

            ////for (int i = 0; i <= e - 1; i++)
            ////{


            ////    A[i, i] += (1.0 - h[i]) / Dt;
            ////    //-- Is this this last column? --
            ////    if ((i + 1) % Math.Sqrt(elements) == 0)
            ////    {
            ////        //use backward differencing in X
            ////        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                * Math.Sqrt((1.0 / nx) * (sox - ((h[i] - h[i - 1]) / cellsize)))
            ////                * Math.Pow((h[i] - h[i - 1]), (2 / 3));

            ////        //-- Is this the very last element? --
            ////        if (i == elements - 1)
            ////        {
            ////            //use backward differencing in Y
            ////            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                        * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
            ////                        * Math.Pow((h[rows] - h[i - rows]), (2 / 3));
            ////        }
            ////        else
            ////        {
            ////            //use forward differencing in Y
            ////            A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                        * Math.Sqrt((1.0 / ny) * (soy - ((h[i + rows] - h[i]) / cellsize)))
            ////                        * Math.Pow((h[2 * rows] - h[rows]), (2 / 3));
            ////        }
            ////    }
            ////    //-- Is this the last row? --
            ////    else if ((i + 1) >= (elements - (elements / rows)))
            ////    {
            ////        //use forward differencing in X
            ////        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i + 1] - h[i]) / cellsize)))
            ////                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));

            ////        //use backward differencing in Y
            ////        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i] - h[i - rows]) / cellsize)))
            ////                    * Math.Pow((h[rows] - h[i - rows]), (2 / 3));
            ////    }
            ////    else
            ////    {
            ////        //use forward differencing in X
            ////        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                    * Math.Sqrt((1.0 / nx) * (sox - ((h[i + 1] - h[i]) / cellsize)))
            ////                    * Math.Pow((h[i + 1] - h[i]), (2 / 3));
            ////        //use forward differencing in Y
            ////        A[i, i] += (1.0 - h[i]) * (1.0 / cellsize)
            ////                    * Math.Sqrt((1.0 / ny) * (soy - ((h[i + rows] - h[i]) / cellsize)))
            ////                    * Math.Pow((h[i + rows] - h[rows]), (2 / 3));
            ////    }
            ////}

            #endregion

            return A;
        }
        private double[,] PopulateA(double[] h)
        {
            //int e = (Int32)Math.Pow(h.Length, 2);
            double[,] A = new double[h.Length, h.Length];
            double Dt = this.dt / 3600;
            double[,] H = new double[(Int32)Math.Sqrt(h.Length), (Int32)Math.Sqrt(h.Length)];
            int eRow = (Int32)Math.Sqrt(h.Length);

            #region convert h into 2d matrix
            for (int i = 0; i <= (Int32)Math.Sqrt(h.Length) - 1; i++)
            {
                for (int j = 0; j <= (Int32)Math.Sqrt(h.Length) - 1; j++)
                {
                    H[i, j] = h[j + (i * (Int32)Math.Sqrt(h.Length))];
                }
            }
            #endregion

            #region Formulate Stiffness Matrix
            double Cx1, Cx2, Cx3, Cy1, Cy2, Cy3;
            double dhdx, dhdy, dh2dx2, dh2dy2;
            for (int i = 0; i <= H.GetLength(0) - 1; i++)
            {
                for (int j = 0; j <= H.GetLength(1) - 1; j++)
                {
                    //---                                ---//
                    //--- Calculate the non-linear terms ---//
                    //---                                ---//

                    //if this is the first column
                    if (j == 0)
                    {
                        //h(i-1) == h(i)
                        dhdx = (H[i, j + 1] - H[i, j]) / (2 * cellsize);
                        dh2dx2 = (H[i, j + 1] - H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        //h(i+1) = h(i)
                        dhdx = (-1 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        dh2dx2 = (-1 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior column
                    else
                    {
                        dhdx = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (2 * cellsize);
                        dh2dx2 = (H[i, j + 1] - 2 * H[i, j] + H[i, j - 1]) / (Math.Pow(cellsize, 2));
                    }
                    //if this the first row
                    if (i == 0)
                    {
                        //h(j-1) = h(j)
                        dhdy = (H[i + 1, j] - H[i, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] - H[i, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //h(j+1) = h(j)
                        dhdy = (-1 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (-1 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }
                    //if this is an interior row
                    else
                    {
                        dhdy = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (2 * cellsize);
                        dh2dy2 = (H[i + 1, j] - 2 * H[i, j] + H[i - 1, j]) / (Math.Pow(cellsize, 2));
                    }


                    //---
                    //--- Calculate Coefficients ---
                    //---

                    //if this is the first column
                    if (j == 0)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j + 1], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this is the last column
                    else if (j == H.GetLength(1) - 1)
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this is an interior column
                    else
                        Cy2 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * (Math.Pow(H[i, j + 1], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i, j - 1], 2.0 / 3.0)) * (0.5) * dhdy;

                    //if this the first row
                    if (i == 0)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - Math.Pow(H[i, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    //if this is the last row
                    else if (i == H.GetLength(0) - 1)
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (-1 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    //if this is an interior row
                    else
                        Cx2 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * (Math.Pow(H[i + 1, j], 2.0 / 3.0) - 2 * Math.Pow(H[i, j], 2.0 / 3.0) + Math.Pow(H[i - 1, j], 2.0 / 3.0)) * (0.5) * dhdx;

                    Cx1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(sox - dhdx, -1.0 / 2.0) * dh2dx2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy1 = (Dt / cellsize) * (1.0 / 2.0) * Math.Pow(soy - dhdy, -1.0 / 2.0) * dh2dy2 * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cx3 = (Dt / cellsize) * Math.Sqrt(sox - dhdx) * Math.Pow(H[i, j], 2.0 / 3.0);
                    Cy3 = (Dt / cellsize) * Math.Sqrt(soy - dhdy) * Math.Pow(H[i, j], 2.0 / 3.0);

                    //---
                    //--- Add coefficients to stiffness matrix
                    //---

                    //last column and last row
                    if (i == H.GetLength(0) - 1 && j == H.GetLength(1) - 1)
                    {
                        //h(x)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);\
                        A[i, j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                    }
                    //last column
                    else if (j == H.GetLength(1) - 1)
                    {
                        ////h(x)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        ////h(x+1)
                        //A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = 0;
                        ////h(y+1)
                        //A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;

                        //h(x)
                        A[i, j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        //A[i,(j + 1)] = 0;
                        //h(y+1)
                        A[i + 1, j] += -Cy3 / cellsize;
                    }
                    //last row
                    else if (i == H.GetLength(0) - 1)
                    {
                        //////h(x)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //////h(x+1)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;

                        //h(x)
                        A[i, j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i, (j + 1)] += -Cx3 / cellsize;
                    }
                    //interior element
                    else
                    {
                        //////h(x)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + j] = 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //////h(x+1)
                        ////A[i * H.GetLength(0) + j, i * H.GetLength(0) + (j + 1)] = -Cx3 / cellsize;
                        //////h(y+1)
                        ////A[i * H.GetLength(0) + (j + 1), i * H.GetLength(0) + j] = -Cy3 / cellsize;

                        //h(x)
                        A[i, j] += 1 - Cx1 - Cx2 - Cy1 - Cy2 + (Cx3 / cellsize) + (Cy3 / cellsize);
                        //h(x+1)
                        A[i, (j + 1)] += -Cx3 / cellsize;
                        //h(y+1)
                        A[i + 1, j] += -Cy3 / cellsize;
                    }
                }
            }
            #endregion

            return A;
        }
        public double[,] buildExcessMatrix(int rows, int cols)
        {
            return new double[rows,cols];
        }
        public double[] Stage2Flow(double[] stage)
        {
            //assume broad crested weir
            double[] Q = new double[elements];

            //get number of elements per row
            int rowCount = elements / stage.Length;


            //HACK: should probably be 9.81
            //double g = 32.2;

            int rowID = 0;
            double Stage = stage[0];

            for (int i = 0; i <= elements - 1; i++)
            {
                if (i % stage.Length == 0)
                {

                    //set crest elevation of embankment
                    
                    double hr = stage[rowID];
                    double hfp = h[i];

                    double Sb = 0;
                    double Hr = 0;

                    //calculate Hr
                    if (hr > hfp && hr > hw)
                        Hr = (hr - hw) / (hfp - hw);
                    else if (hfp > hw && hfp > hr)
                        Hr = (hfp - hw) / (hr - hw);

                    //calculate submergence factor
                    if (Hr > 0.67)
                        Sb = 1.0 - 27.8 * Math.Pow(stage[rowID] - 0.67, 3);
                    else
                        Sb = 1.0;

                    double Cf = 0.62;  //weir discharge coefficient (broad crested)(SI)


                    //for Inflow from river to flood plain


                    if (hr > hw && hr > hfp)
                        Q[i] = Cf * Sb * Math.Pow(hr - hw, 3.0 / 2.0) * cellsize;
                    else if (hfp > hw && hfp > hr)
                        Q[i] = Cf * Sb * Math.Pow(hfp - hw, 3.0 / 2.0) * cellsize;

                    rowID++;
                }

            }
            return Q;
        }

        public double[] SuccessiveOverRelaxation(double[,] A, double[] b)
        {

            double[] x = new double[A.GetLength(0)];
            double[] x1 = new double[A.GetLength(0)];
            double Maxdiff = 0;

            //calculate optimal relaxation factor
            double C = Math.Cos(Math.PI / A.GetLength(0)) + Math.Cos(Math.PI / A.GetLength(1));
            double w = 4 / (2 + Math.Sqrt(4 + Math.Pow(C, 2)));

            do
            {
                for (int i = 0; i <= A.GetLength(0)-1; i++)
                {
                    double R  = 0;

                    //calculate residual R
                    for (int j = 0; j <=i-1 ; j++)
                        R += A[i,j]*x1[j];
                    for(int j =i+1;j<=A.GetLength(0)-1;j++)
                        R += A[i,j]*x[j];

                    //calculate X at time k+1
                    x1[i] = (1 - w) * x[i] + (w / A[i, i]) * (b[i] - R);
                }

                //check for convergence
                Maxdiff = 0;
                for (int i = 0; i <= A.GetLength(0) - 1; i++)
                {
                    double diff = Math.Abs(x1[i] - x[i]);

                    if (diff > Maxdiff)
                        Maxdiff = diff;
                }

                //set x(k) = x(k+1)
                x1.CopyTo(x, 0);


            } while (Maxdiff > 0.001);

            //return result
            return x1;

            #region create diagonal matrix


            //int rows = A.GetLength(0);
            //int cols = A.GetLength(1);
            //double[,] U = new double[rows, cols];
            //double[,] L = new double[rows, cols];
            //double[,] D = new double[rows, cols];

            ////create upper, lower, and diagonal matrices
            //int diagonal = 0;
            //for (int i = 0; i <= rows - 1; i++)
            //{
            //    for (int j = 0; j <= cols - 1; j++)
            //    {
            //        if (i == j)
            //        {
            //            D[i, j] = A[i, j];
            //            diagonal = j;
            //        }
            //        else if (j > diagonal)
            //        {
            //            U[i, j] = A[i, j];
            //        }
            //        else
            //        {
            //            L[i, j] = A[i, j];
            //        }

            //    }
            //}

            //for (int array = 0; array <= 2; array++)
            //{
            //    for (int i = 0; i <= rows - 1; i++)
            //    {
            //        for (int j = 0; j <= cols - 1; j++)
            //        {

            //            if (array == 0)
            //            {
            //                Debug.Write(U[i, j].ToString() + "\t");
            //            }
            //            if (array == 1)
            //            {
            //                Debug.Write(L[i, j].ToString() + "\t");
            //            }
            //            if(array == 2)
            //            {
            //                Debug.Write(D[i, j].ToString() + "\t");
            //            }
            //        }
            //        Debug.Write("\n");
            //    }
            //    Debug.Write("\n");
            //}
            #endregion
        }

        public void TransformInputsToElementSet(double[] inputs, double[] transform)
        {
        }

        public ElementSet BuildElementSet(string elevation, string id, string desc)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(elevation);            

            //read element set attributes
            string[] line   = sr.ReadLine().Split(' ');
            int cols        = Convert.ToInt32(line[line.Length - 1]);   line = sr.ReadLine().Split(' ');
            int rows        = Convert.ToInt32(line[line.Length - 1]);   line = sr.ReadLine().Split(' ');
            double xlower   = Convert.ToDouble(line[line.Length - 1]);  line = sr.ReadLine().Split(' ');
            double ylower   = Convert.ToDouble(line[line.Length - 1]);  line = sr.ReadLine().Split(' ');
            double cellsize = Convert.ToDouble(line[line.Length - 1]);  line = sr.ReadLine().Split(' ');
            double NODATA   = Convert.ToDouble(line[line.Length - 1]);  
            
            //create array to hold elevations
            double[,] elevations     = new double[rows, cols];
            _soy    = new double[rows, cols];
            _sox    = new double[rows, cols];

            //get x upper and yupper coordinates
            double x = xlower;
            double y = ylower + cellsize * rows;

            //define element set
            ElementSet elementset = new ElementSet();
            elementset.Description = desc;
            elementset.ID = id;
            elementset.ElementType = OpenMI.Standard.ElementType.XYPoint;


            //read elevations
            for (int i = 0; i <= rows - 1; i++)
            {
                line = sr.ReadLine().Split(' ');

                for (int j = 0; j <= cols - 1; j++)
                {
                    //get the elevation
                    double z = Convert.ToDouble(line[j]);

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
                }

                //get the new y coordinate, and reset the x coordinate
                y -= cellsize;
                x = xlower;
            }

            //calculate slopes in the x and y directions
            for (int i = 0; i <= rows - 1; i++)
            {
                for (int j = 0; j <= cols - 1; j++)
                {
                    
                    double SOY = 0;
                    double SOX = 0;

                    //BROKEN:  Apply the same error checking to SOY that you did to SOX

                    //if its the first row
                    if (i == 0)
                    {
                        //take the difference between the (i)th and (i+1)th
                        SOY = elevations[i, j] - elevations[i + 1, j];

                       //check to see if something went wrong!
                        if (Math.Abs(SOY) > elevations[i, j] || Math.Abs(SOY) > elevations[i + 1, j])
                            SOY = 0.0; 
                    }
                    //if its the last row
                    else if (i == rows - 1)
                    {
                        //take the difference between the (i)th and (i-1)th
                        SOY = elevations[i, j] - elevations[i - 1, j];

                        //check to see if something went wrong!
                        if (Math.Abs(SOY) > elevations[i, j] || Math.Abs(SOY) > elevations[i - 1, j])
                            SOY = 0.0; 
                    }
                    //if its an interior row
                    else
                    { 
                        //take the maximum of the two
                        double soy1 = elevations[i, j] - elevations[i + 1, j];
                        double soy2 = elevations[i, j] - elevations[i - 1, j];

                        if (Math.Abs(soy1) > Math.Abs(elevations[i, j]))
                        {
                            if (Math.Abs(soy2) > Math.Abs(elevations[i, j]))
                            {
                                SOY = 0.0;
                            }
                            else
                            {
                                SOY = soy2;
                            }
                        }
                        else
                        {
                            if (Math.Abs(soy2) > Math.Abs(elevations[i, j]))
                            {
                                SOY = soy1;
                            }
                            else
                            {
                                if (Math.Abs(soy1) > Math.Abs(soy2))
                                    SOY = soy1;
                                else
                                    SOY = soy2;
                            }
                        }

                    }

                    //if its the first column
                    if (j == 0)
                    {
                        //take the difference between the (j)th and (j+1)th
                        SOX = elevations[i, j] - elevations[i, j + 1];

                        //check to see if something went wrong!
                        if (Math.Abs(SOX) > elevations[i, j] || Math.Abs(SOX) > elevations[i, j + 1])
                            SOX = 0.0; 
                    }
                    //if its the last column
                    else if (j == cols - 1)
                    {
                        //take the difference between the (j)th and (j-1)th
                        SOX = elevations[i, j] - elevations[i, j - 1];

                        //check to see if something went wrong!
                        if (Math.Abs(SOX) > elevations[i, j] || Math.Abs(SOX) > elevations[i, j - 1])
                            SOX = 0.0; 
                    }
                    //if its an interior column
                    else
                    {

                        //take the maximum of the two
                        double sox1 = elevations[i, j] - elevations[i, j + 1];
                        double sox2 = elevations[i, j] - elevations[i, j - 1];

                        if (Math.Abs(sox1) > Math.Abs(elevations[i, j]))
                        {
                            if (Math.Abs(sox2) > Math.Abs(elevations[i, j]))
                            {
                                SOX = 0.0;
                            }
                            else
                            {
                                SOX = sox2;
                            }
                        }
                        else
                        {
                            if (Math.Abs(sox2) > Math.Abs(elevations[i, j]))
                            {
                                SOX = sox1;
                            }
                            else
                            {
                                if (Math.Abs(sox1) > Math.Abs(sox2))
                                    SOX = sox1;
                                else
                                    SOX = sox2;
                            }
                        }
                    }


                    //set slope values
                    _soy[i, j] = SOY/cellsize;
                    _sox[i, j] = SOX/cellsize;
                }
            }
            return elementset;

        }

    }
    public class LinkableComponent : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            _engineApiAccess = new ModelEngine();
        }
        public LinkableComponent()
        {
            _engineApiAccess = new ModelEngine();
        }
    }

}
