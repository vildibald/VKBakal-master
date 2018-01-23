using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SilverlightApplication3
{
    internal class EngineBSplineUtilities
    {
        internal double[] ServiceKnots(double[] knots, int degree, bool generateBothSides)
        {
            int pocet = knots.Length;

            double max = knots.Max();
            double min = knots.Min();
            double h = (max - min) / (pocet - 1);
            double[] serviceKnots;
            if (generateBothSides)
            {
                serviceKnots = new double[2 * degree + pocet];
                Array.Copy(knots, 0, serviceKnots, degree, pocet);
                for (int i = 0; i < degree; i++)
                {
                    serviceKnots[i] = min - (degree + 1 - (i + 1)) * h;
                    serviceKnots[degree + pocet + i] = max + (i + 1) * h;
                }
            }
            else
            {
                serviceKnots = new double[degree + pocet];
                Array.Copy(knots, 0, serviceKnots, degree, pocet);
                for (int i = 0; i < degree; i++)
                {
                    serviceKnots[i] = min - (degree + 1 - (i + 1)) * h;
                }
            }

            return serviceKnots;
        }


        internal double Bell(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations)
        {
            // double[] knots=new double[input_knots.Length];
            //Array.Copy(input_knots, knots, knots.Length);
            Array.Reverse(knots);
            double y = +Math.Pow((x - knots[0]), degree); ;
            //  bool isDegreeEven = degree % 2 == 0;
            if (numberOfIterations > degree + 1) throw new IndexOutOfRangeException("GenerateBell: numberOfIterations is larger than degree+1");

            for (int k = 1; k < numberOfIterations; k++)
            {

                double sigma = Math.Pow((x - knots[k]), degree);
                for (int m = 1; m <= degree + 1; m++)
                {
                    if (m != k)
                    {

                        sigma = sigma * (knots[0] - knots[m]) / (knots[k] - knots[m]);

                    }

                }

                y -= sigma;
            }
            if (isDegreeEven) return -y; else return y;

        }

        internal double Bell(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations, int knotStartIndex, int numberOfKnots)
        {
            double[] usedKnots;
            usedKnots = new double[numberOfKnots];
            Array.Copy(knots, knotStartIndex, usedKnots, 0, numberOfKnots);
            return Bell(x, degree, usedKnots, isDegreeEven, numberOfIterations);

        }

        public double BellDerivative(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations)
        {
            // double[] knots=new double[input_knots.Length];
            //Array.Copy(input_knots, knots, knots.Length);
            Array.Reverse(knots);
            double y = +Math.Pow((x - knots[0]), degree - 1) * degree;
            //  bool isDegreeEven = degree % 2 == 0;
            if (numberOfIterations > degree + 1) throw new IndexOutOfRangeException("GenerateBellDerivative: i is larger than degree+1");

            for (int k = 1; k < numberOfIterations; k++)
            {

                double sigma = degree * Math.Pow((x - knots[k]), degree - 1);
                for (int m = 1; m <= degree + 1; m++)
                {
                    if (m != k)
                    {

                        sigma = sigma * (knots[0] - knots[m]) / (knots[k] - knots[m]);

                    }

                }

                y -= sigma;
            }
            System.Diagnostics.Debug.WriteLine(y);
            if (isDegreeEven) return -y; else return y;

        }

        internal double BellDerivative(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations, int knotStartIndex, int numberOfKnots)
        {
            double[] usedKnots;
            usedKnots = new double[numberOfKnots];
            Array.Copy(knots, knotStartIndex, usedKnots, 0, numberOfKnots);
            return BellDerivative(x, degree, usedKnots, isDegreeEven, numberOfIterations);

        }

        internal Tuple<double[][], double[]> FunctionApproximationMatrix(FunctionDefinition functionDefinition)
        {
            return FunctionApproximationMatrix(functionDefinition.XCoordinates, functionDefinition.Knots, functionDefinition.KnotsIndexes, functionDefinition.Degree);
        }


        internal Tuple<double[][], double[]> FunctionApproximationMatrix(double[] X, double[] XKnots, int[] XKnotsIndexes, int degree)
        {
            int numberOfRows = X.Length;
            int numberOfColumns = XKnots.Length - 1 + degree;
            double[] auxXKnots;
            bool isDegreeEven = degree % 2 == 0;

            double[][] matrix = new double[numberOfRows][];


            for (int i = 0; i < numberOfRows; i++)
            {
                matrix[i] = new double[numberOfColumns];
            }
            auxXKnots = ServiceKnots(XKnots, degree, true);

            int k = 0;
            //for (int i = 0; i < numberOfRows; i++)
            //{
            //    int kk = Math.Max(0, k-1);

            //    for (int j = 0; j < numberOfColumns; j++)
            //    {

            //        if(j>=kk & j<=kk+degree)
            //        {
            //            //if (j - kk + 1==2)
            //            //{
            //            //    Console.WriteLine("zacina debug");
            //            //}
            //            matrix[i][j]=Bell(serviceKnots, X[i], degree,!isDegreeEven, j-kk+1, j, degree+2);
            //        }else{
            //            matrix[i][j]=0;
            //        }
            //    }
            //    if(i==XKnotsIndexes[k]){
            //        k++;
            //    }
            //}

            for (int i = 0; i < numberOfRows; i++)
            {
                int kk = Math.Max(0, k - 1);

                for (int j = 0; j < numberOfColumns; j++)
                {

                    if (j >= kk & j <= kk + degree)
                    {
                        //if (j - kk + 1==2)
                        //{
                        //    Console.WriteLine("zacina debug");
                        //}
                        matrix[numberOfRows - i - 1][numberOfColumns - j - 1] = Bell(X[i], degree, auxXKnots, !isDegreeEven, j - kk + 1, j, degree + 2);
                    }
                    else
                    {
                        matrix[numberOfRows - i - 1][numberOfColumns - j - 1] = 0;
                    }
                }
                if (k<XKnotsIndexes.Length&& i == XKnotsIndexes[k])
                {
                    k++;
                }
            }

            return new Tuple<double[][], double[]>(matrix, auxXKnots);
        }

        internal Tuple<double[], double[]> FunctionApproximationControlPointsAndAuxiliaryKnots(FunctionDefinition function)
        {
            if (function.Knots == null || function.KnotsIndexes == null)
            {
                System.Diagnostics.Debug.WriteLine("FunctionDefinition is incomplete!");
                return null;
            }
            return FunctionApproximationControlPointsAndAuxiliaryKnots(function.Degree, function.XCoordinates, function.YCoordinates, function.Knots, function.KnotsIndexes);
        }

        internal Tuple<double[], double[]> FunctionApproximationControlPointsAndAuxiliaryKnots(int degree, double[] pointsX, double[] pointsY, double[] knots, int[] knotsIndexes)
        {

            Tuple<double[][], double[]> matrixAndAuxKnots = FunctionApproximationMatrix(pointsX, knots, knotsIndexes, degree);
            double[][] matrix = matrixAndAuxKnots.Item1;
            double[][] transposeMatrix = MatrixOperations.TransposeMatrix(matrix);
            double[][] transposeAndMatrixMultiplicationInversion = MatrixOperations.MatrixInvert(MatrixOperations.MultiplyMatrices(transposeMatrix, matrix));
            double[][] YMatrix = MatrixOperations.ArrayToMatrix(pointsY);
            double[][] finalMatrix;
            double[] finalMatrixAsArray;
            List<double[][]> matrixList = new List<double[][]>();
            matrixList.Add(transposeAndMatrixMultiplicationInversion);
            matrixList.Add(transposeMatrix);
            matrixList.Add(YMatrix);
            finalMatrix = MatrixOperations.MultiplyListOfMatrices(matrixList);
            try
            {
                finalMatrixAsArray = MatrixOperations.OneColumnMatrixToArray(finalMatrix);

            }
            catch (NumberOfMatrixColumnIsNotOneException)
            {
                System.Diagnostics.Debug.WriteLine("Matrix cannot be converted to an array because number of columns is not one");
                return null;
            }

            return new Tuple<double[], double[]>(finalMatrixAsArray, matrixAndAuxKnots.Item2);
        }

        internal Tuple<double[], double[]> FunctionControlPointsAndAuxiliaryKnots(FunctionDefinition functionDefinition)
        {
            return FunctionControlPointsAndAuxiliaryKnots(functionDefinition.Degree, functionDefinition.Knots, functionDefinition.YCoordinates, false);
        }


        internal Tuple<double[][], double[]> FunctionBSplineMatrix(int degree, double[] knots, bool areKnotsService)
        {

            double[] serviceKnots;
            int k;
            int numberOfRowsAndColumns;
            int numberOfRowsForLoop;
            if (areKnotsService)
            {
                serviceKnots = knots;
                k = knots.Length -2*degree - 1;
                numberOfRowsAndColumns = k + 3;
                numberOfRowsForLoop = k+1;
            }
            else
            {
                serviceKnots = ServiceKnots(knots, degree, true);
                k = knots.Length - 1;
                numberOfRowsAndColumns = k + 3;
                numberOfRowsForLoop = knots.Length;
            }

           
            
            
            bool isDegreeEven = degree % 2 == 0;

            double[][] matrix = new double[numberOfRowsAndColumns][];


            for (int i = 0; i < numberOfRowsAndColumns; i++)
            {
                matrix[i] = new double[numberOfRowsAndColumns];
            }


            for (int i = 0; i < numberOfRowsForLoop; i++)
            {
                for (int j = 0; j < numberOfRowsAndColumns; j++)
                {
                    //matrix[i][j] = EngineUtils.Bell(serviceKnots[degree + i], degree, serviceKnots, isDegreeEven, j - i + 1, j, degree + 2);
                    if (j >= Math.Max(0, i - 1) && j <= i - 1 + degree)
                    //if (j >= i && j <= i - 1 + degree)
                    //if (i <= j & i >= j + degree)                  
                    {
                        matrix[i][j] = Bell(serviceKnots[degree + i], degree, serviceKnots, !isDegreeEven, j - i + 1, j, degree + 2);
                        //matrix[i][j] = EngineUtils.Bell(serviceKnots[degree + i], degree, serviceKnots, !isDegreeEven, j - i + 1);
                    }
                    else
                    {
                        matrix[i][j] = 0;
                    }
                }
            }
            for (int j = 0; j < numberOfRowsAndColumns; j++)
            {
                if (j >= 0 && j <= degree)
                //if (i <= j & i >= j + degree)                  
                {
                    //matrix[numberOfRowsForLoop - i - 1][numberOfRowsAndColumns - j - 1] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);
                    //matrix[i][j] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);
                    // matrix[i][j] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);                       
                    matrix[numberOfRowsForLoop][j] = BellDerivative(serviceKnots[degree], degree, serviceKnots, !isDegreeEven, j+1, j, degree + 2);
                }
                else
                {
                    matrix[numberOfRowsForLoop][j] = 0;
                }
            }

            for (int j = 0; j < numberOfRowsAndColumns; j++)
            {
                if (j >= numberOfRowsAndColumns - 1 -degree && j <= numberOfRowsAndColumns - 1)
                //if (i <= j & i >= j + degree)                  
                {
                    //matrix[numberOfRowsForLoop - i - 1][numberOfRowsAndColumns - j - 1] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);
                    //matrix[i][j] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);
                    // matrix[i][j] = EngineUtils.Bell(serviceKnots, knots[i], degree, !isDegreeEven, j - i + 1, j, degree + 2);                       
                    matrix[numberOfRowsForLoop + 1][j] = BellDerivative(serviceKnots[k], degree, serviceKnots, !isDegreeEven, j - (numberOfRowsForLoop-1) + 1, j, degree + 2);
                }
                else
                {
                    matrix[numberOfRowsForLoop+1][j] = 0;
                }
            }
            //matrix[numberOfRowsForLoop] = matrix[0];
            //matrix[numberOfRowsAndColumns-1] = matrix[numberOfRowsForLoop-1];
            //matrix[]

                return new Tuple<double[][], double[]>(matrix, serviceKnots);
        }



        internal Tuple<double[], double[]> FunctionControlPointsAndAuxiliaryKnots(int degree, double[] knots, double[] knotsFunctionValues, bool areKnotsService)
        {
            var matrixAndAuxKnots = FunctionBSplineMatrix(degree, knots, areKnotsService);
            var invertedMatrix = MatrixOperations.MatrixInvert(matrixAndAuxKnots.Item1);
            var knotsFunctionValuesMatrix = MatrixOperations.ArrayToMatrix(knotsFunctionValues);
            var controlPointsMatrix = MatrixOperations.MultiplyMatrices(invertedMatrix,knotsFunctionValuesMatrix);
            double[] controlPoints = null;
            try
            {
                controlPoints = MatrixOperations.OneColumnMatrixToArray(controlPointsMatrix);
            }
            catch (NumberOfMatrixColumnIsNotOneException)
            {
                System.Diagnostics.Debug.WriteLine("Matrix cannot be converted to an array because number of columns is not one");
                return null;
            }
            return new Tuple<double[], double[]>(controlPoints,matrixAndAuxKnots.Item2);
            
        }

        internal FunctionDefinition CalculateFunctionDefinition(int degree, double[] knots, Function function)
        {
            double[] Y = new double[knots.Length + 2];
            int[] knotsIndexes = null;


            if (function.Equals(Function.Runge))
            {
                for (int i = 0; i < knots.Length; i++)
                {

                    Y[i] = 1 / (1 + knots[i] * knots[i]);
                }
                Y[Y.Length - 2] = (-2 * knots[0]) / Math.Pow((1 + knots[0] * knots[0]), 2);
                Y[Y.Length - 1] = (-2 * knots[knots.Length - 1]) / Math.Pow((1 + knots[knots.Length - 1] * knots[knots.Length - 1]), 2);
            }
            else if (function.Equals(Function.Sinus))
            {
                for (int i = 0; i < knots.Length; i++)
                {
                    Y[i] = Math.Sin(knots[i]);

                }
                Y[Y.Length - 2] = Math.Cos(knots[0]);
                Y[Y.Length - 1] = Math.Cos(knots[knots.Length - 1]);

            }
            return new FunctionDefinition(degree, Y, knots, function);
            //return new FunctionDefinition(degree, Y, knots, function);
        }

        internal FunctionDefinition FunctionApproximationDefinition(int degree, double left, double right, int numberOfPoints, int numberOfIntervals, Function function)
        {
            double[] X = new double[numberOfPoints];
            double[] Y = new double[numberOfPoints];

            double[] knots = new double[numberOfIntervals + 1];
            double h = (right - left) / (numberOfPoints - 1);

            double x = left;

            int[] knotsIndexes = new int[numberOfIntervals + 1];
            for (int i = 0; i < knotsIndexes.Length; i++)
            {
                double a = i * ((double)(numberOfPoints - 1) / numberOfIntervals);
                int knotIndex = (int)Math.Round(a);
                knotsIndexes[i] = knotIndex;
                System.Diagnostics.Debug.WriteLine(string.Join(", ", knotsIndexes[i]));
            }

            int j = 0;

            if (function.Equals(Function.Runge))
            {
                for (int i = 0; i < numberOfPoints; i++)
                {
                    X[i] = x;
                    Y[i] = 1 / (1 + x * x);
                    if (/*j <= numberOfIntervals &&*/ i == knotsIndexes[j])
                    {
                        knots[j] = X[i];
                        j++;
                    }
                    x += h;
                }
            }
            else if (function.Equals(Function.Sinus))
            {
                for (int i = 0; i < numberOfPoints; i++)
                {
                    X[i] = x;
                    Y[i] = Math.Sin(x);
                    if (i == knotsIndexes[j])
                    {
                        knots[j] = X[i];
                        j++;
                    }
                    x += h;
                }
            }

            return new FunctionDefinition(degree, X, Y, knots, knotsIndexes);
        }

        private int[] knotIndexes(int numberOfPoints,int numberOfKnots)
        {
            int[] knotsIndexes = new int[numberOfKnots];
            for (int i = 0; i < knotsIndexes.Length; i++)
            {
                double a = i * ((double)(numberOfPoints - 1) / numberOfKnots+1);
                int knotIndex = (int)Math.Round(a);
                knotsIndexes[i] = knotIndex;
                System.Diagnostics.Debug.WriteLine(string.Join(", ", knotsIndexes[i]));
            }
            return knotsIndexes;
        }
    }
}
