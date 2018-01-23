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
    internal class BSplineGenerator
    {
        //internal double[] ServiceKnots(double[] knots, int degree, bool generateBothSides)
        //{
        //    int length = knots.Length;

        //    double max = knots.Max();
        //    double min = knots.Min();
        //    double h = (max - min) / (length - 1);
        //    double[] knotsWithGeneratedServiceKnots;
        //    if (generateBothSides)
        //    {
        //        knotsWithGeneratedServiceKnots = new double[2 * degree + length];
        //        Array.Copy(knots, 0, knotsWithGeneratedServiceKnots, degree, length);
        //        for (int i = 0; i < degree; i++)
        //        {
        //            knotsWithGeneratedServiceKnots[i] = min - (degree + 1 - (i + 1)) * h;
        //            knotsWithGeneratedServiceKnots[degree + length + i] = max + (i + 1) * h;
        //        }
        //    }
        //    else
        //    {
        //        knotsWithGeneratedServiceKnots = new double[degree + length];
        //        Array.Copy(knots, 0, knotsWithGeneratedServiceKnots, degree, length);
        //        for (int i = 0; i < degree; i++)
        //        {
        //            knotsWithGeneratedServiceKnots[i] = min - (degree + 1 - (i + 1)) * h;
        //        }
        //    }

        //    return knotsWithGeneratedServiceKnots;
        //}


        internal double Bell(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations)
        {
            // double[] knots=new double[input_knots.Length];
            //Array.Copy(input_knots, knots, knots.Length);
            Array.Reverse(knots);
            double y = +Math.Pow((x - knots[0]), degree);
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

        internal double BellDerivative(double x, int degree, double[] knots, bool isDegreeEven, int numberOfIterations)
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

        private Tuple<double[][], double[]> FunctionApproximationMatrix(FunctionDefinition functionDefinition)
        {
            return FunctionApproximationMatrix(functionDefinition.XCoordinates, functionDefinition.Knots, functionDefinition.KnotsIndexes, functionDefinition.Degree);
        }


        private Tuple<double[][], double[]> FunctionApproximationMatrix(double[] X, double[] XKnots, int[] XKnotsIndexes, int degree)
        {
            int numberOfRows = X.Length;
            int numberOfColumns = XKnots.Length - 1 + degree;
            double[] auxXKnots;
            bool isDegreeEven = degree % 2 == 0;

            double[][] matrix = ArrayMyUtils.CreateMatrix<double>(numberOfRows, numberOfColumns);
            auxXKnots = BSpline.ServiceKnots(XKnots, degree, true);

            int k = 0;
            //for (int i = 0; i < numberOfRows; i++)
            //{
            //    int kk = Math.Max(0, numberOfEquations-1);

            //    for (int j = 0; j < numberOfColumns; j++)
            //    {

            //        if(j>=kk & j<=kk+degree)
            //        {
            //            //if (j - kk + 1==2)
            //            //{
            //            //    Console.WriteLine("zacina debug");
            //            //}
            //            leftSide[i][j]=Bell(knots, knots[i], degree,!isDegreeEven, j-kk+1, j, degree+2);
            //        }else{
            //            leftSide[i][j]=0;
            //        }
            //    }
            //    if(i==XKnotsIndexes[numberOfEquations]){
            //        numberOfEquations++;
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
            double[][] transposeMatrix = MathOperations.TransposeMatrix(matrix);
            double[][] transposeAndMatrixMultiplicationInversion = MathOperations.MatrixInvert(MathOperations.MultiplyMatrices(transposeMatrix, matrix));
            double[][] YMatrix = ArrayMyUtils.ArrayToMatrix(pointsY);
            double[][] finalMatrix;
            double[] finalMatrixAsArray;
            List<double[][]> matrixList = new List<double[][]>();
            matrixList.Add(transposeAndMatrixMultiplicationInversion);
            matrixList.Add(transposeMatrix);
            matrixList.Add(YMatrix);
            finalMatrix = MathOperations.MultiplyListOfMatrices(matrixList);
            try
            {
                finalMatrixAsArray = ArrayMyUtils.OneColumnMatrixToArray(finalMatrix);

            }
            catch (NumberOfMatrixColumnIsNotOneException)
            {
                System.Diagnostics.Debug.WriteLine("Matrix cannot be converted to an array because number of columns is not one");
                return null;
            }

            return new Tuple<double[], double[]>(finalMatrixAsArray, matrixAndAuxKnots.Item2);
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
