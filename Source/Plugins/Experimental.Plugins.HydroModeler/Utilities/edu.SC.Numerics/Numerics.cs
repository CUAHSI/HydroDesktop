using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Numerics
{
    public class Numerics
    {

        public Dictionary<string,double> SecantMethod(double fx1,double x1, double fx2, double x2)
        {
            double x3 = x2 - (fx2 * (x1 - x2) / (fx1 - fx2));
            double e = Math.Abs(x2 - x3);

            Dictionary<string, double> results = new Dictionary<string, double>();
            results["x2"] = x3;
            results["x1"] = x2;
            results["error"] = e;

            return results;
           
        }

        public Dictionary<string, double> ModifiedSecantMethod(double fx, double x, double fxdel, double delta)
        {
            double x2 = x - ((delta * x * fx) / (fxdel - fx));
            double e = Math.Abs(x - x2);

            Dictionary<string, double> results = new Dictionary<string, double>();
            results["x"] = x2;
            results["error"] = e;
            results["delta"] = delta;

            return results;

        }

        #region not working
        //public NewtonRhapson(Parameter[] parameters, Func<double>[] functions, int numberOfDerivativePoints)
        //{

        //    Parameter[] _parameters = parameters;
        //    Func<double>[] _functions = functions;
        //    int numberOfFunctions = _functions.Length;
        //    int numberOfParameters = _parameters.Length;
        //    Derivatives _derivatives = new Derivatives(numberOfDerivativePoints);

        //    Matrix _jacobian = new Matrix(numberOfFunctions, numberOfParameters);
        //    Matrix _functionMatrix = new Matrix(numberOfFunctions, 1);
        //    Matrix _x0 = new Matrix(numberOfFunctions, 1);
        //}

        //public NewtonRaphson(Parameter[] parameters, Func<double>[] functions): this(parameters, functions, 3)
        //{
        //}

        //public void Iterate()
        //{
        //    int numberOfFunctions = _functions.Length;
        //    int numberOfParameters = _parameters.Length;
        //    for (int i = 0; i < numberOfFunctions; i++)
        //    {
        //        _functionMatrix[i, 0] = _functions[i]();
        //        _x0[i, 0] = _parameters[i];
        //    }

        //    for (int i = 0; i < numberOfFunctions; i++)
        //    {
        //        for (int j = 0; j < numberOfParameters; j++)
        //        {
        //            _jacobian[i, j] = _derivatives.ComputePartialDerivative(_functions[i], _parameters[j], 1, _functionMatrix[i, 0]);
        //        }
        //    }

        //    Matrix newXs = _x0 - _jacobian.SolveFor(_functionMatrix);

        //    for (int i = 0; i < numberOfFunctions; i++)
        //    {
        //        _parameters[i].Value = newXs[i, 0];
        //    }
        //}
#endregion
    }

    //***************************************************************************************
    //**** The classes below were adapted from Trent Guidry http://www.trentfguidry.net/ ****
    //***************************************************************************************
    public class NewtonRaphson
    {
        private Matrix _jacobian;
        private Matrix _functionMatrix;
        private Matrix _x0;
        private Derivatives _derivatives;
        private Parameter[] _parameters;
        private Func<double>[] _functions;

        public NewtonRaphson(Parameter[] parameters, Func<double>[] functions, int numberOfDerivativePoints)
        {
            _parameters = parameters;
            _functions = functions;
            int numberOfFunctions = _functions.Length;
            int numberOfParameters = _parameters.Length;

            Debug.Assert(numberOfParameters == numberOfFunctions);

            _derivatives = new Derivatives(numberOfDerivativePoints);

            _jacobian = new Matrix(numberOfFunctions, numberOfParameters);
            _functionMatrix = new Matrix(numberOfFunctions, 1);
            _x0 = new Matrix(numberOfFunctions, 1);
        }

        public NewtonRaphson(Parameter[] parameters, Func<double>[] functions)
            : this(parameters, functions, 3)
        {
        }

        public void Iterate()
        {
            int numberOfFunctions = _functions.Length;
            int numberOfParameters = _parameters.Length;
            for (int i = 0; i < numberOfFunctions; i++)
            {
                _functionMatrix[i, 0] = _functions[i]();
                _x0[i, 0] = _parameters[i];
            }

            for (int i = 0; i < numberOfFunctions; i++)
            {
                for (int j = 0; j < numberOfParameters; j++)
                {
                    _jacobian[i, j] = _derivatives.ComputePartialDerivative(_functions[i], _parameters[j], 1, _functionMatrix[i, 0]);
                }
            }

            Matrix newXs = _x0 - _jacobian.SolveFor(_functionMatrix);

            for (int i = 0; i < numberOfFunctions; i++)
            {
                _parameters[i].Value = newXs[i, 0];
            }
        }
        public Parameter[] parameter
        {
            get { return _parameters; }
        }
        public double[] GetResult()
        {

            double[] r = new double[_parameters.Length];
            for (int i = 0; i <= _parameters.Length - 1; i++ )
            {
                r[i] = _parameters[i].Value;
            }
            return r;

        }
    }

    public class Matrix
    {
        #region ctor
        public Matrix()
        {
            _values = new double[_rowCount, _columnCount];
        }

        public Matrix(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _values = new double[_rowCount, _columnCount];
        }
        #endregion

        #region Row Column values
        public double this[int row, int column]
        {
            get { return _values[row, column]; }
            set { _values[row, column] = value; }
        }
        #endregion

        #region F&P
        private double[,] _values;

        private int _rowCount = 3;
        public int RowCount
        {
            get { return _rowCount; }
        }

        private int _columnCount = 3;
        public int ColumnCount
        {
            get { return _columnCount; }
        }
        #endregion

        #region basic single matrix stuff
        public static Matrix Identity(int size)
        {
            Matrix resultMatrix = new Matrix(size, size);
            Parallel.For(0, size, (i) =>
            {
                for (int j = 0; j < size; j++)
                {
                    resultMatrix[i, j] = (i == j) ? 1.0 : 0.0;
                }
            }
            );
            return resultMatrix;
        }

        public Matrix Clone()
        {
            Matrix resultMatrix = new Matrix(_rowCount, _columnCount);
            Parallel.For(0, _rowCount, (i) =>
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[i, j] = this[i, j];
                }
            }
            );
            return resultMatrix;
        }

        public Matrix Transpose()
        {
            Matrix resultMatrix = new Matrix(_columnCount, _rowCount);
            Parallel.For(0, _rowCount, (i) =>
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    resultMatrix[j, i] = this[i, j];
                }
            }
            );
            return resultMatrix;
        }

        #endregion

        #region Binary Math
        public static Matrix Add(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);

            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] + rightMatrix[i, j];
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator +(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Add(leftMatrix, rightMatrix);
        }

        public static Matrix Subtract(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.ColumnCount);
            Debug.Assert(leftMatrix.RowCount == rightMatrix.RowCount);
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] - rightMatrix[i, j];
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator -(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Subtract(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
        {
            Debug.Assert(leftMatrix.ColumnCount == rightMatrix.RowCount);
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, resultMatrix.ColumnCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.RowCount; j++)
                {
                    double value = 0.0;
                    for (int k = 0; k < rightMatrix.RowCount; k++)
                    {
                        value += leftMatrix[j, k] * rightMatrix[k, i];
                    }
                    resultMatrix[j, i] = value;
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.Multiply(leftMatrix, rightMatrix);
        }

        public static Matrix Multiply(double left, Matrix rightMatrix)
        {
            Matrix resultMatrix = new Matrix(rightMatrix.RowCount, rightMatrix.ColumnCount);
            Parallel.For(0, resultMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < rightMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = left * rightMatrix[i, j];
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator *(double left, Matrix rightMatrix)
        {
            return Matrix.Multiply(left, rightMatrix);
        }

        public static Matrix Multiply(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] * right;
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator *(Matrix leftMatrix, double right)
        {
            return Matrix.Multiply(leftMatrix, right);
        }

        public static Matrix Divide(Matrix leftMatrix, double right)
        {
            Matrix resultMatrix = new Matrix(leftMatrix.RowCount, leftMatrix.ColumnCount);
            Parallel.For(0, leftMatrix.RowCount, (i) =>
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    resultMatrix[i, j] = leftMatrix[i, j] / right;
                }
            }
            );
            return resultMatrix;
        }

        public static Matrix operator /(Matrix leftMatrix, double right)
        {
            return Matrix.Divide(leftMatrix, right);
        }
        #endregion

        #region Assorted Casts
        public static Matrix FromArray(double[] left)
        {
            int length = left.Length;
            Matrix resultMatrix = new Matrix(length, 1);
            for (int i = 0; i < length; i++)
            {
                resultMatrix[i, 0] = left[i];
            }
            return resultMatrix;
        }

        public static implicit operator Matrix(double[] left)
        {
            return FromArray(left);
        }

        public static double[] ToArray(Matrix leftMatrix)
        {
            Debug.Assert((leftMatrix.ColumnCount == 1 && leftMatrix.RowCount >= 1) || (leftMatrix.RowCount == 1 && leftMatrix.ColumnCount >= 1));

            double[] result = null;
            if (leftMatrix.ColumnCount > 1)
            {
                int numElements = leftMatrix.ColumnCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[0, i];
                }
            }
            else
            {
                int numElements = leftMatrix.RowCount;
                result = new double[numElements];
                for (int i = 0; i < numElements; i++)
                {
                    result[i] = leftMatrix[i, 0];
                }
            }
            return result;
        }

        public static implicit operator double[](Matrix leftMatrix)
        {
            return ToArray(leftMatrix);
        }

        public static Matrix FromDoubleArray(double[,] left)
        {
            int length0 = left.GetLength(0);
            int length1 = left.GetLength(1);
            Matrix resultMatrix = new Matrix(length0, length1);
            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    resultMatrix[i, j] = left[i, j];
                }
            }
            return resultMatrix;
        }

        public static implicit operator Matrix(double[,] left)
        {
            return FromDoubleArray(left);
        }

        public static double[,] ToDoubleArray(Matrix leftMatrix)
        {
            double[,] result = new double[leftMatrix.RowCount, leftMatrix.ColumnCount];
            for (int i = 0; i < leftMatrix.RowCount; i++)
            {
                for (int j = 0; j < leftMatrix.ColumnCount; j++)
                {
                    result[i, j] = leftMatrix[i, j];
                }
            }
            return result;
        }

        public static implicit operator double[,](Matrix leftMatrix)
        {
            return ToDoubleArray(leftMatrix);
        }
        #endregion
        public Matrix SolveFor(Matrix rightMatrix)
        {
            Debug.Assert(rightMatrix.RowCount == _columnCount);
            Debug.Assert(_columnCount == _rowCount);

            Matrix resultMatrix = new Matrix(_columnCount, rightMatrix.ColumnCount);
            LUDecompositionResults resDecomp = LUDecompose();
            int[] nP = resDecomp.PivotArray;
            Matrix lMatrix = resDecomp.L;
            Matrix uMatrix = resDecomp.U;
            Parallel.For(0, rightMatrix.ColumnCount, k =>
            {
                //Solve for the corresponding d Matrix from Ld=Pb
                double sum = 0.0;
                Matrix dMatrix = new Matrix(_rowCount, 1);
                dMatrix[0, 0] = rightMatrix[nP[0], k] / lMatrix[0, 0];
                for (int i = 1; i < _rowCount; i++)
                {
                    sum = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        sum += lMatrix[i, j] * dMatrix[j, 0];
                    }
                    dMatrix[i, 0] = (rightMatrix[nP[i], k] - sum) / lMatrix[i, i];
                }
                //Solve for x using Ux = d
                resultMatrix[_rowCount - 1, k] = dMatrix[_rowCount - 1, 0];
                for (int i = _rowCount - 2; i >= 0; i--)
                {
                    sum = 0.0;
                    for (int j = i + 1; j < _rowCount; j++)
                    {
                        sum += uMatrix[i, j] * resultMatrix[j, k];
                    }
                    resultMatrix[i, k] = dMatrix[i, 0] - sum;
                }
            }
            );
            return resultMatrix;
        }

        private LUDecompositionResults LUDecompose()
        {
            Debug.Assert(_columnCount == _rowCount);
            // Using Crout Decomp with P
            //
            // Ax = b //By definition of problem variables.
            //
            // LU = PA //By definition of L, U, and P.
            //
            // LUx = Pb //By substition for PA.
            //
            // Ux = d //By definition of d
            //
            // Ld = Pb //By subsitition for d.
            //
            //For 4x4 with P = I
            // [l11 0 0 0 ] [1 u12 u13 u14] [a11 a12 a13 a14]
            // [l21 l22 0 0 ] [0 1 u23 u24] = [a21 a22 a23 a24]
            // [l31 l32 l33 0 ] [0 0 1 u34] [a31 a32 a33 a34]
            // [l41 l42 l43 l44] [0 0 0 1 ] [a41 a42 a43 a44]
            LUDecompositionResults result = new LUDecompositionResults();
            try
            {
                int[] pivotArray = new int[_rowCount]; //Pivot matrix.
                Matrix uMatrix = new Matrix(_rowCount, _columnCount);
                Matrix lMatrix = new Matrix(_rowCount, _columnCount);
                Matrix workingUMatrix = Clone();
                Matrix workingLMatrix = new Matrix(_rowCount, _columnCount);
                Parallel.For(0, _rowCount, i =>
                {
                    pivotArray[i] = i;
                }
                );
                //Iterate down the number of rows in the U matrix.
                for (int i = 0; i < _rowCount; i++)
                {
                    //Do pivots first.
                    //I want to make the matrix diagnolaly dominate.
                    //Initialize the variables used to determine the pivot row.
                    double maxRowRatio = double.NegativeInfinity;
                    int maxRow = -1;
                    int maxPosition = -1;
                    //Check all of the rows below and including the current row
                    //to determine which row should be pivoted to the working row position.
                    //The pivot row will be set to the row with the maximum ratio
                    //of the absolute value of the first column element divided by the
                    //sum of the absolute values of the elements in that row.
                    Parallel.For(i, _rowCount, j =>
                    {
                        //Store the sum of the absolute values of the row elements in
                        //dRowSum. Clear it out now because I am checking a new row.
                        double rowSum = 0.0;
                        //Go across the columns, add the absolute values of the elements in
                        //that column to dRowSum.
                        for (int k = i; k < _columnCount; k++)
                        {
                            rowSum += Math.Abs(workingUMatrix[pivotArray[j], k]);
                        }
                        //Check to see if the absolute value of the ratio of the lead
                        //element over the sum of the absolute values of the elements is larger
                        //that the ratio for preceding rows. If it is, then the current row
                        //becomes the new pivot candidate.
                        if (rowSum == 0.0)
                        {
                            throw new SingularMatrixException();
                        }
                        double dCurrentRatio = Math.Abs(workingUMatrix[pivotArray[j], i]) / rowSum;
                        lock (this)
                        {
                            if (dCurrentRatio > maxRowRatio)
                            {
                                maxRowRatio = Math.Abs(workingUMatrix[pivotArray[j], i] / rowSum);
                                maxRow = pivotArray[j];
                                maxPosition = j;
                            }
                        }
                    }
                    );

                    //If the pivot candidate isn't the current row, update the
                    //pivot array to swap the current row with the pivot row.
                    if (maxRow != pivotArray[i])
                    {
                        int hold = pivotArray[i];
                        pivotArray[i] = maxRow;
                        pivotArray[maxPosition] = hold;
                    }
                    //Store the value of the left most element in the working U
                    //matrix in dRowFirstElementValue.
                    double rowFirstElementValue = workingUMatrix[pivotArray[i], i];
                    //Update the columns of the working row. j is the column index.
                    Parallel.For(0, _columnCount, j =>
                    {
                        if (j < i)
                        {
                            //If j<1, then the U matrix element value is 0.
                            workingUMatrix[pivotArray[i], j] = 0.0;
                        }
                        else if (j == i)
                        {
                            //If i == j, the L matrix value is the value of the
                            //element in the working U matrix.
                            workingLMatrix[pivotArray[i], j] = rowFirstElementValue;
                            //The value of the U matrix for i == j is 1
                            workingUMatrix[pivotArray[i], j] = 1.0;
                        }
                        else // j>i
                        {
                            //Divide each element in the current row of the U matrix by the
                            //value of the first element in the row
                            workingUMatrix[pivotArray[i], j] /= rowFirstElementValue;
                            //The element value of the L matrix for j>i is 0
                            workingLMatrix[pivotArray[i], j] = 0.0;
                        }
                    }
                    );
                    //For the working U matrix, subtract the ratioed active row from the rows below it.
                    //Update the columns of the rows below the working row. k is the row index.
                    for (int k = i + 1; k < _rowCount; k++)
                    {
                        //Store the value of the first element in the working row
                        //of the U matrix.
                        rowFirstElementValue = workingUMatrix[pivotArray[k], i];
                        //Go accross the columns of row k.
                        Parallel.For(0, _columnCount, j =>
                        {
                            if (j < i)
                            {
                                //If j<1, then the U matrix element value is 0.
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else if (j == i)
                            {
                                //If i == j, the L matrix value is the value of the
                                //element in the working U matrix.
                                workingLMatrix[pivotArray[k], j] = rowFirstElementValue;
                                //The element value of the L matrix for j>i is 0
                                workingUMatrix[pivotArray[k], j] = 0.0;
                            }
                            else //j>i
                            {
                                workingUMatrix[pivotArray[k], j] = workingUMatrix[pivotArray[k], j] - rowFirstElementValue * workingUMatrix[pivotArray[i], j];
                            }
                        }
                        );
                    }
                }
                Parallel.For(0, _rowCount, i =>
                {
                    for (int j = 0; j < _rowCount; j++)
                    {
                        uMatrix[i, j] = workingUMatrix[pivotArray[i], j];
                        lMatrix[i, j] = workingLMatrix[pivotArray[i], j];
                    }
                }
                );
                result.U = uMatrix;
                result.L = lMatrix;
                result.PivotArray = pivotArray;
            }
            catch (AggregateException ex2)
            {
                if (ex2.InnerExceptions.Count > 0)
                {
                    throw ex2.InnerExceptions[0];
                }
                else
                {
                    throw ex2;
                }
            }
            catch (Exception ex3)
            {
                throw ex3;
            }
            return result;
        }

        public Matrix Invert()
        {
            Debug.Assert(_rowCount == _columnCount);
            Matrix resultMatrix = SolveFor(Identity(_rowCount));
            Matrix matIdent = this * resultMatrix;

            return SolveFor(Identity(_rowCount));
        }
    }

    public class LUDecompositionResults
    {
        private Matrix _lMatrix;
        private Matrix _uMatrix;
        private int[] _pivotArray;

        public LUDecompositionResults()
        {
        }

        public LUDecompositionResults(Matrix matL, Matrix matU, int[] nPivotArray)
        {
            _lMatrix = matL;
            _uMatrix = matU;
            _pivotArray = nPivotArray;
        }

        public Matrix L
        {
            get { return _lMatrix; }
            set { _lMatrix = value; }
        }

        public Matrix U
        {
            get { return _uMatrix; }
            set { _uMatrix = value; }
        }

        public int[] PivotArray
        {
            get { return _pivotArray; }
            set { _pivotArray = value; }
        }

    }

    public class SingularMatrixException : ArithmeticException
    {
        public SingularMatrixException()
            : base("Invalid operation on a singular matrix.")
        {
        }
    }

    public class Derivatives
    {
        // _coefficients is the array of differential coefficients matrices.
        // The index corresponds to the position from the left edge
        // of the points.
        // I.e _coefficients[0] is for a matrix with three points in it corresponds to
        // the left most point.
        // The coefficients of the derivatives go down by row.  I.e. the first row
        // is the functional value, the second row is for the first derivative of the functional
        // value, the third row is the second derivative of the functional value.
        // The columns correspond to the points themselves.
        private Matrix[] _coefficients;

        private Derivatives()
        {
        }

        public Derivatives(int numberOfPoints)
            : this()
        {
            SolveCoefs(numberOfPoints);
        }

        public void SolveCoefs(int numberOfPoints)
        {
            _coefficients = new Matrix[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                Matrix deltsMatrix = new Matrix(numberOfPoints, numberOfPoints);
                for (int j = 0; j < numberOfPoints; j++)
                {
                    double delt = (double)(j - i);
                    double HTerm = 1.0;
                    for (int k = 0; k < numberOfPoints; k++)
                    {
                        deltsMatrix[j, k] = HTerm / Factorial(k);
                        HTerm *= delt;
                    }
                }
                _coefficients[i] = deltsMatrix.Invert();
                double numPointsFactorial = Factorial(numberOfPoints);
                for (int j = 0; j < numberOfPoints; j++)
                {
                    for (int k = 0; k < numberOfPoints; k++)
                    {
                        _coefficients[i][j, k] = (Math.Round(_coefficients[i][j, k] * numPointsFactorial)) / numPointsFactorial;
                    }
                }
            }
        }

        private static double Factorial(int value)
        {
            double result = 1.0;
            for (int i = 1; i <= value; i++)
            {
                result *= (double)i;
            }
            return result;
        }

        /// <summary>
        /// Computes the derivative of a function.
        /// </summary>
        /// <param name="points">Equally spaced function value points</param>
        /// <param name="order">The order of the derivative to take</param>
        /// <param name="variablePosition">The position in the array of function values to take the derivative at.</param>
        /// <param name="step">The x axis step size.</param>
        /// <returns></returns>
        public double ComputeDerivative(double[] points, int order, int variablePosition, double step)
        {
            Debug.Assert(points.Length == _coefficients.Length);
            Debug.Assert(order < _coefficients.Length);
            double result = 0.0;
            for (int i = 0; i < _coefficients.Length; i++)
            {
                result += _coefficients[variablePosition][order, i] * points[i];
            }
            result /= Math.Pow(step, order);
            return result;
        }

        public double ComputePartialDerivative(Func<double> function, Parameter parameter, int order)
        {
            int numberOfPoints = _coefficients.Length;
            double result = 0.0;
            double originalValue = parameter;
            double[] points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                parameter.Value = originalValue + ((double)(i - centerPoint)) * derivativeStepSize;
                points[i] = function();
            }
            result = ComputeDerivative(points, order, centerPoint, derivativeStepSize);
            parameter.Value = originalValue;
            return result;
        }

        public double ComputePartialDerivative(Func<double> function, Parameter parameter, int order, double currentFunctionValue)
        {
            int numberOfPoints = _coefficients.Length;
            double result = 0.0;
            double originalValue = parameter;
            double[] points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (i != centerPoint)
                {
                    parameter.Value = originalValue + ((double)(i - centerPoint)) * derivativeStepSize;
                    points[i] = function();
                }
                else
                {
                    points[i] = currentFunctionValue;
                }
            }
            result = ComputeDerivative(points, order, centerPoint, derivativeStepSize);
            parameter.Value = originalValue;
            return result;
        }

        public double[] ComputePartialDerivatives(Func<double> function, Parameter parameter, int[] derivativeOrders)
        {
            int numberOfPoints = _coefficients.Length;
            double[] result = new double[derivativeOrders.Length];
            double originalValue = parameter;
            double[] points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                parameter.Value = originalValue + ((double)(i - centerPoint)) * derivativeStepSize;
                points[i] = function();
            }
            for (int i = 0; i < derivativeOrders.Length; i++)
            {
                result[i] = ComputeDerivative(points, derivativeOrders[i], centerPoint, derivativeStepSize);
            }
            parameter.Value = originalValue;
            return result;
        }

        public double[] ComputePartialDerivatives(Func<double> function, Parameter parameter, int[] derivativeOrders, double currentFunctionValue)
        {
            int numberOfPoints = _coefficients.Length;
            double[] result = new double[derivativeOrders.Length];
            double originalValue = parameter;
            double[] points = new double[numberOfPoints];
            double derivativeStepSize = parameter.DerivativeStepSize;
            int centerPoint = (numberOfPoints - 1) / 2;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (i != centerPoint)
                {
                    parameter.Value = originalValue + ((double)(i - centerPoint)) * derivativeStepSize;
                    points[i] = function();
                }
                else
                {
                    points[i] = currentFunctionValue;
                }
            }
            for (int i = 0; i < derivativeOrders.Length; i++)
            {
                result[i] = ComputeDerivative(points, derivativeOrders[i], centerPoint, derivativeStepSize);
            }
            parameter.Value = originalValue;
            return result;
        }

    }

    public class Parameter
    {
        private bool _isSolvedFor = true;
        private double _value;
        private double _derivativeStep = 1e-3;
        private DerivativeStepType _derivativeStepType = DerivativeStepType.Relative;

        public Parameter()
            : base()
        {
        }

        public Parameter(double value)
            : this()
        {
            _value = value;
        }

        public Parameter(double value, double derivativeStep)
            : this(value)
        {
            _derivativeStep = derivativeStep;
        }

        public Parameter(double value, double derivativeStep, DerivativeStepType stepSizeType)
            : this(value, derivativeStep)
        {
            _derivativeStepType = stepSizeType;
        }

        public Parameter(double value, double derivativeStep, DerivativeStepType stepSizeType, bool isSolvedFor)
            : this(value, derivativeStep, stepSizeType)
        {
            _isSolvedFor = isSolvedFor;
        }

        public Parameter(bool isSolvedFor)
            : this()
        {
            _isSolvedFor = isSolvedFor;
        }

        public Parameter(Parameter clone)
        {
            _isSolvedFor = clone.IsSolvedFor;
            _value = clone.Value;
            _derivativeStep = clone.DerivativeStep;
            _derivativeStepType = clone.DerivativeStepType;
        }

        public bool IsSolvedFor
        {
            get { return _isSolvedFor; }
            set { _isSolvedFor = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public double DerivativeStep
        {
            get { return _derivativeStep; }
            set { _derivativeStep = value; }
        }

        public double DerivativeStepSize
        {
            get
            {
                double derivativeStepSize;
                if (_derivativeStepType == DerivativeStepType.Absolute)
                {
                    derivativeStepSize = _derivativeStep;
                }
                else
                {
                    if (_value != 0.0)
                    {
                        derivativeStepSize = _derivativeStep * Math.Abs(_value);
                    }
                    else
                    {
                        derivativeStepSize = _derivativeStep;
                    }
                }
                return derivativeStepSize;
            }
        }

        public DerivativeStepType DerivativeStepType
        {
            get { return _derivativeStepType; }
            set { _derivativeStepType = value; }
        }

        public static implicit operator double(Parameter p)
        {
            return p.Value;
        }

        public override string ToString()
        {
            return "Parameter: Value:" + Value.ToString() + " IsSolvedFor:" + _isSolvedFor.ToString();
        }
    }

    public enum DerivativeStepType
    {
        Relative,
        Absolute
    }

    public class ParameterCollection : Collection<Parameter>
    {
        public ParameterCollection()
            : base()
        {
        }
    }


}
