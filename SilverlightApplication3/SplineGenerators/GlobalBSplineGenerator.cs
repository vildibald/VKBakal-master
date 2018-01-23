using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightApplication3
{
    internal class GlobalBSplineGenerator : BSplineGenerator
    {

        internal Tuple<double[], double[]> GlobalControlPointsAndAuxiliaryKnots(FunctionDefinition functionDefinition)
        {
            return GlobalControlPointsAndAuxiliaryKnots(functionDefinition.Degree, functionDefinition.Knots, functionDefinition.YCoordinates,functionDefinition.LeftDerivation,functionDefinition.RightDerivation, false);
        }


        private Tuple<double[][], double[]> GlobalBSplineMatrix(int degree, double[] knots, bool areKnotsService)
        {

            double[] serviceKnots;
            int k;
            int numberOfRowsAndColumns;
            int numberOfRowsForLoop;
            if (areKnotsService)
            {
                serviceKnots = knots;
                k = knots.Length - 2 * degree - 1;
                numberOfRowsAndColumns = k + 3;
                numberOfRowsForLoop = k + 1;
            }
            else
            {
                serviceKnots = BSpline.ServiceKnots(knots, degree, true);
                k = knots.Length - 1;
                numberOfRowsAndColumns = k + 3;
                numberOfRowsForLoop = knots.Length;
            }




            bool isDegreeEven = degree % 2 == 0;
            var iterations = degree+2;
            double[][] matrix = ArrayMyUtils.CreateMatrix<double>(numberOfRowsAndColumns, numberOfRowsAndColumns);

            //matrix[0][0] = Bell(knots[0], degree, serviceKnots, !isDegreeEven, 1, 0, iterations);
            //matrix[0][1] = Bell(knots[0], degree, serviceKnots, !isDegreeEven, 2, 1, iterations);
            //matrix[0][2] = Bell(knots[0], degree, serviceKnots, !isDegreeEven, 3, 2, iterations);
            for (int i = 0; i < numberOfRowsForLoop; i++)
            {
                matrix[i][i] = Bell(serviceKnots[degree + i], degree, serviceKnots, !isDegreeEven,  1, i, degree + 2);
                matrix[i][i+1] = Bell(serviceKnots[degree + i], degree, serviceKnots, !isDegreeEven, 2, i+1, degree + 2);
                matrix[i][i+2] = Bell(serviceKnots[degree + i], degree, serviceKnots, !isDegreeEven, 3, i+2, degree + 2);
            }
            matrix[numberOfRowsAndColumns - 2][0] = BellDerivative(serviceKnots[degree], degree, serviceKnots, !isDegreeEven, 1, 0, iterations);
            matrix[numberOfRowsAndColumns - 2][1] = BellDerivative(serviceKnots[degree], degree, serviceKnots, !isDegreeEven, 2, 1, iterations);
            matrix[numberOfRowsAndColumns - 2][2] = BellDerivative(serviceKnots[degree], degree, serviceKnots, !isDegreeEven, 3, 2, iterations);

            matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 3] = BellDerivative(serviceKnots[degree + numberOfRowsForLoop - 1], degree, serviceKnots, !isDegreeEven, 1, numberOfRowsAndColumns - 3, iterations);
            matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 2] = BellDerivative(serviceKnots[degree + numberOfRowsForLoop - 1], degree, serviceKnots, !isDegreeEven, 2, numberOfRowsAndColumns - 2, iterations);
            matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 1] = BellDerivative(serviceKnots[degree + numberOfRowsForLoop - 1], degree, serviceKnots, !isDegreeEven, 3, numberOfRowsAndColumns - 1, iterations);

            //System.Diagnostics.Debug.WriteLine("  ...BEG.......   ");
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 2][0]);
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 2][1]);
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 2][2]);
            //System.Diagnostics.Debug.WriteLine("  ...MED.......   ");
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 3]);
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 2]);
            //System.Diagnostics.Debug.WriteLine(matrix[numberOfRowsAndColumns - 1][numberOfRowsAndColumns - 1]);
            //System.Diagnostics.Debug.WriteLine("  ...END.......   ");

                return new Tuple<double[][], double[]>(matrix, serviceKnots);
        }
        internal Tuple<double[], double[]> GlobalControlPointsAndAuxiliaryKnots(int degree, double[] knots, double[] knotsFunctionValues,double leftDerivation,double rightDerivation, bool areKnotsService)
        {
            var matrixAndAuxKnots = GlobalBSplineMatrix(degree, knots, areKnotsService);
            var invertedMatrix = MathOperations.MatrixInvert(matrixAndAuxKnots.Item1);
            var controlPoints = MathOperations.MultiplyMatrixAndVector(invertedMatrix, knotsFunctionValues,leftDerivation,rightDerivation);


            //var knotsFunctionValuesMatrix = ArrayMyUtils.ArrayToMatrix(knotsFunctionValues);
            //var controlPointsMatrix = AlgebraOperations.MultiplyMatrices(invertedMatrix,knotsFunctionValuesMatrix);
            //double[] controlPoints = null;
            //try
            //{
            //    controlPoints = ArrayMyUtils.OneColumnMatrixToArray(controlPointsMatrix);
            //}
            //catch (NumberOfMatrixColumnIsNotOneException)
            //{
            //    System.Diagnostics.Debug.WriteLine("Matrix cannot be converted to an array because number of columns is not one");
            //    return null;
            //}
            return new Tuple<double[], double[]>(controlPoints, matrixAndAuxKnots.Item2);

        }

        internal FunctionDefinition CalculateFunctionDefinition(int degree, double[] knots, Function function)
        {
            double[] Y = new double[knots.Length];
            //int[] knotsIndexes = null;
            double leftDer = 0;
            double rightDer = 0;

            if (function.Equals(Function.Runge))
            {
                for (int i = 0; i < knots.Length; i++)
                {

                    Y[i] = 1 / (1 + knots[i] * knots[i]);
                }
               leftDer = (-2 * knots[0]) / Math.Pow((1 + knots[0] * knots[0]), 2);
               rightDer = (-2 * knots[knots.Length - 1]) / Math.Pow((1 + knots[knots.Length - 1] * knots[knots.Length - 1]), 2);
            }
            else if (function.Equals(Function.Sinus))
            {
                for (int i = 0; i < knots.Length; i++)
                {
                    Y[i] = Math.Sin(knots[i]);

                }
               leftDer = Math.Cos(knots[0]);
               rightDer = Math.Cos(knots[knots.Length - 1]);

            }
            
            return new FunctionDefinition(degree, Y, knots,leftDer,rightDer, function);
            //return new FunctionDefinition(degree, controlPoints, knots, function);
        } 

    }
}
