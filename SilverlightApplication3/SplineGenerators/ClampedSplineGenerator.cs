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
    internal class ClampedSplineGenerator : HermiteSplineGenerator
    {
        private const double FEL = 0.5;
        private const double HET = 7.0;
        
        //internal double DualHermiteBasis(double x, double x0, double y0, double x1, double y1, double x2, double y2, double leftDerivation, double d2)
        //{

        //}

        // matica pre nezjodnocavny cspline
        //protected double[][] ClampedSplineMatrixForFiveControlPoints()
        //{
        //    var numberOfRowsAndColums = 3;
        //    // var derivations = new double[controlPoints.Length];

        //    double[][] matrix = ArrayMyUtils.CreateMatrix<double>(numberOfRowsAndColums, numberOfRowsAndColums);
        //    matrix[0][0] = 1;
        //    matrix[0][1] = 0.25;
        //    matrix[numberOfRowsAndColums - 1][numberOfRowsAndColums - 1] = 1;
        //    matrix[numberOfRowsAndColums - 1][numberOfRowsAndColums - 2] = 0.25;
        //    for (int i = 1; i < numberOfRowsAndColums - 1; i++)
        //    {
        //        matrix[i][i - 1] = 0.25;
        //        matrix[i][i] = 1;
        //        matrix[i][i + 1] = 0.25;
        //        //  leftSide[i] = 
        //    }
        //    return matrix;
        //}


        //protected Tuple<double[][], double[]> ClampedSplineMatrixAndRightSide(double[] controlPoints, double leftDerivation, double rightDerivation)
        //{
           
        //    int numberOfEquations;
        //    double lastValueOnLeftSide;
        //    double lastValueOnRightSide;
        //    int numberOfControlPoints = controlPoints.Length;
        //    //int k0;
        //    if (numberOfControlPoints % 2 == 0)
        //    {
        //        numberOfEquations = (int)(0.5 + ((numberOfControlPoints - 5.0) / 2.0));
        //        lastValueOnLeftSide = 7.5;
        //        lastValueOnRightSide = (-2) * rightDerivation + 1.5 * controlPoints[numberOfControlPoints - 4] 
        //            - 6 * controlPoints[numberOfControlPoints - 3] - 1.5 * controlPoints[numberOfControlPoints - 2] 
        //            + 6 * controlPoints[numberOfControlPoints - 1];
        //        //k0 = (2 + 2*numberOfEquations);
        //    }
        //    else
        //    {
        //        numberOfEquations = (int)(1.0 + ((numberOfControlPoints - 5.0) / 2.0));
        //        lastValueOnLeftSide = 7;
        //        lastValueOnRightSide = 0.5 * rightDerivation + 1.5 * controlPoints[numberOfControlPoints - 4]
        //            - 6 * controlPoints[numberOfControlPoints - 3] + 6 * controlPoints[numberOfControlPoints - 2]
        //            - 1.5 * controlPoints[numberOfControlPoints - 1];
        //        //k0 = (3 + 2 * numberOfEquations);
        //    }

        //    var leftSide = ArrayMyUtils.CreateMatrix<double>(numberOfEquations);
        //    var rightSide = new double[numberOfEquations];

        //    leftSide[0][0] = 7;
        //    leftSide[0][1] = -0.5;
        //    leftSide[numberOfEquations - 1][numberOfEquations - 1] = lastValueOnLeftSide;
        //    leftSide[numberOfEquations - 1][numberOfEquations - 2] = -0.5;
        //    rightSide[0] = 1.5*controlPoints[0]-1.5*controlPoints[4]-6*controlPoints[1]+6*controlPoints[3]+0.5*leftDerivation;
        //    rightSide[numberOfEquations-1] = lastValueOnRightSide;

        //    for (int i = 1; i < numberOfEquations - 1; i++)
        //    {
        //        var k = 2*i;
        //        leftSide[i][i - 1] = -0.5;
        //        leftSide[i][i] = -7; 
        //        leftSide[i][i + 1] = -0.5;
        //        rightSide[i] = 1.5 * controlPoints[k] - 1.5 * controlPoints[k + 4] - 6 * controlPoints[k + 1] + 6 * controlPoints[k + 3];
        //    }                   
        //    return Tuple.Create(leftSide, rightSide);
       
        //}

        protected double[] EvenDerivations(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            int numberOfEquations; 
            double lastValueOnLeftSide;
            double lastValueOnRightSide;
            int numberOfControlPoints = controlPoints.Length;
            var doubleDiff = 2* difference;
            if (numberOfControlPoints % 2 == 0)
            {
                
                numberOfEquations = (int)(1.5 + ((numberOfControlPoints - 5.0) / 2.0));
                var idx = (numberOfEquations -1) * 2;
                lastValueOnLeftSide = HET+FEL;
               // lastValueOnRightSide = (-2) * rightDerivation + 1.5 * controlPoints[numberOfControlPoints - 4]
               //     - 6 * controlPoints[numberOfControlPoints - 3] - 1.5 * controlPoints[numberOfControlPoints - 2]
               //     + 6 * controlPoints[numberOfControlPoints - 1];
               // lastValueOnRightSide = (3 * (controlPoints[numberOfControlPoints - 4] - controlPoints[numberOfControlPoints - 2]) - 12 * (controlPoints[numberOfControlPoints - 3] - controlPoints[numberOfControlPoints - 1]) - 4* rightDerivation * difference) / (doubleDiff);
                lastValueOnRightSide = (3 * (controlPoints[idx] - controlPoints[idx+2]) - 12 * (controlPoints[idx+1] - controlPoints[idx+3]) - 4 * rightDerivation * difference) / (doubleDiff);
            }
            else
            {
                numberOfEquations = (int)(1.0 + ((numberOfControlPoints - 5.0) / 2.0));
                var idx = (numberOfEquations - 1) * 2;
                lastValueOnLeftSide = HET;
                //lastValueOnRightSide = FEL * rightDerivation + 1.5 * controlPoints[numberOfControlPoints - 4]
                //    - 6 * controlPoints[numberOfControlPoints - 3] + 6 * controlPoints[numberOfControlPoints - 2]
                //    - 1.5 * controlPoints[numberOfControlPoints - 1];
                //lastValueOnRightSide = (3* (controlPoints[numberOfControlPoints-5] - controlPoints[numberOfControlPoints-1]) - 12 * (controlPoints[numberOfControlPoints-4] - controlPoints[numberOfControlPoints-2]) + rightDerivation * difference)/(doubleDiff);
                lastValueOnRightSide = (3 * (controlPoints[idx] - controlPoints[idx+4]) - 12 * (controlPoints[idx+1] - controlPoints[idx+3]) + rightDerivation * difference) / (doubleDiff);
            }

            var lowerDiagonal = new double[numberOfEquations - 1];
            var mainDiagonal = new double[numberOfEquations];
            var upperDiagonal = new double[numberOfEquations - 1];
            var rightSide = new double[numberOfEquations];

            ArrayMyUtils.ArrayFill(lowerDiagonal, -FEL);
            Array.Copy(lowerDiagonal, upperDiagonal, upperDiagonal.Length-1);
            ArrayMyUtils.ArrayFill(mainDiagonal, 7);
            mainDiagonal[mainDiagonal.Length - 1] = lastValueOnLeftSide;

            //rightSide[0] = 1.5*controlPoints[0]-1.5*controlPoints[4]-6*controlPoints[1]+6*controlPoints[3]+0.5*leftDerivation;
            rightSide[0] = (3 * (controlPoints[0] - controlPoints[4]) - 12 * (controlPoints[1] - controlPoints[3]) + leftDerivation * difference) / (doubleDiff);
            rightSide[numberOfEquations-1] = lastValueOnRightSide;
            for (int i = 1; i < numberOfEquations - 1; i++)
            {
                var k = 2*i;
                //leftSide[i][i - 1] = -0.5;
                //leftSide[i][i] = -7; 
                //leftSide[i][i + 1] = -0.5;
                //rightSide[i] = 1.5 * controlPoints[k] - 1.5 * controlPoints[k + 4] - 6 * controlPoints[k + 1] + 6 * controlPoints[k + 3];
                rightSide[i] = (3*(controlPoints[k]-controlPoints[k+4]) -12*(controlPoints[k+1] - controlPoints[k+3]))/(doubleDiff);
            }

            return MathOperations.SolveTridiagonalLinearEquationSystem(lowerDiagonal, mainDiagonal, upperDiagonal, rightSide);
        }

        protected double[] ClampedDerivationsForThreeKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            return ClampedDerivationsForThreeKnots(controlPoints, leftDerivation, rightDerivation,difference, 3);
        }

        private double ClampedDerivation(double cp0, double cp1, double cp2, double leftDerivation, double rightDerivation, double difference)
        {
          //  return 0.75 * (cp2 - cp0) - 0.25 * (rightDerivation - leftDerivation);
            return -(3 * (cp0 - cp2) / difference + rightDerivation + leftDerivation)/4;
        }


        protected double[] ClampedDerivationsForThreeKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference, int sizeOfReturnedArray)
        {
            var derivations = new double[sizeOfReturnedArray];
            derivations[0] = leftDerivation;
            derivations[2] = rightDerivation;

            //derivations[1] = 0.75 * (controlPoints[2] - controlPoints[0]) - 0.25 * (rightDerivation - leftDerivation);
            derivations[1] = -(3 * (controlPoints[0] - controlPoints[2]) / difference + rightDerivation + leftDerivation) / 4;
            return derivations;
        }

        protected double[] ClampedDerivationsForFourKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            return ClampedDerivationsForFourKnots(controlPoints, leftDerivation, rightDerivation,difference, 4);
        }

        protected double[] ClampedDerivationsForFourKnots(double[] controlPoints, double leftDerivation, double rightDerivation,double difference, int sizeOfReturnedArray)
        {
           // var d2 = 0.2 * ((1 / 3) * leftDerivation + controlPoints[0] - controlPoints[2] - 4 * controlPoints[1] + 4 * controlPoints[3] - (4 / 3) * rightDerivation);
            var d2 =  (leftDerivation + 3 * controlPoints[0] - 3 * controlPoints[2] - 12 * controlPoints[1] + 12 * controlPoints[3] - 4 * rightDerivation)/15;
            var d1 = ClampedDerivation(controlPoints[0], controlPoints[1], controlPoints[2], leftDerivation, d2, difference);
            var derivations = new double[sizeOfReturnedArray];
            derivations[0] = leftDerivation;
            derivations[1] = d1;
            derivations[2] = d2;
            derivations[3] = rightDerivation;
            return derivations;
        }

        protected double[] ClampedDerivationsForFiveKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            return ClampedDerivationsForFiveKnots(controlPoints, leftDerivation, rightDerivation,difference, 5);
        }

        protected double[] ClampedDerivationsForFiveKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference, int sizeOfReturnedArray)
        {
            //trocha vadny pristup
           // var d2 = (leftDerivation + 12 * controlPoints[3] + 3 * controlPoints[0] - 3 * controlPoints[4] - 12 * controlPoints[1] + rightDerivation) / 14;
            
            //var d1 = ClampedDerivation(controlPoints[0], controlPoints[1], controlPoints[2], leftDerivation, d2);
            //var d3 = ClampedDerivation(controlPoints[2], controlPoints[3], controlPoints[4], d2, rightDerivation);

            //podla clanku C. Toroka
            var d2 = (difference * (leftDerivation + rightDerivation) + 3 * (controlPoints[0] - 4 * controlPoints[1] + 4 * controlPoints[3] - controlPoints[4]))
                / (14 * difference);
            var d1 = -(difference * (15 * leftDerivation + rightDerivation) + 3 * (15 * controlPoints[0] - 4 * controlPoints[1] - 14 * controlPoints[2] + 4 * controlPoints[3] - controlPoints[4]))
                /(56*difference);
            var d3 = -(difference * (leftDerivation + 15 * rightDerivation) + 3 * (controlPoints[0] - 4 * controlPoints[1] + 14 * controlPoints[2] + 4 * controlPoints[3] - 15 * controlPoints[4]))
                / (56 * difference);
           
            /////////////////////////


            var derivations = new double[sizeOfReturnedArray];
            derivations[0] = leftDerivation;
            derivations[1] = d1;
            derivations[2] = d2;
            derivations[3] = d3;
            derivations[4] = rightDerivation;
            return derivations;
        }
        //protected double[] ClampedDerivationsForSixKnots(double cp0, double cp1, double cp2, double cp3, double cp4, double cp5, double leftDerivation, double rightDerivation)
        //{
        //    return ClampedDerivationsForSixKnots(cp0, cp1, cp2, cp3, cp4, cp5, leftDerivation, rightDerivation);
        //}
        //protected double[] ClampedDerivationsForSixKnots(double cp0, double cp1, double cp2, double cp3, double cp4,double cp5, double leftDerivation, double rightDerivation, int sizeOfReturnedArray)
        //{
        //    throw new NotImplementedException();
        //}

        protected double[] ClampedDerivationsForNKnots(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            //var equationSystem = EvenDerivations(controlPoints, leftDerivation, rightDerivation);
            //var inverseLeftSide = AlgebraOperations.MatrixInvert(equationSystem.Item1);

            var evenDerivations = EvenDerivations(controlPoints, leftDerivation, rightDerivation, difference);

            var controlPointsLength = controlPoints.Length;
            var derivations = new double[controlPointsLength];

            derivations[0] = leftDerivation;
            derivations[controlPointsLength - 1] = rightDerivation;
            int j=2;
            for (int i = 0; i < evenDerivations.Length; i++)
            {
                derivations[j] = evenDerivations[i];
                derivations[j - 1] = ClampedDerivation(controlPoints[j - 2], controlPoints[j - 1], controlPoints[j], derivations[j - 2], derivations[j], difference);
                j+=2;
            }
            derivations[controlPointsLength - 2] = ClampedDerivation(controlPoints[controlPointsLength - 3], 
                controlPoints[controlPointsLength - 2], controlPoints[controlPointsLength - 1], 
                derivations[controlPointsLength - 3], derivations[controlPointsLength - 1], difference);

            return derivations;

        }

        internal double[] ClampedDerivations(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            var controlPointsLength = controlPoints.Length;
            switch (controlPointsLength)
            {
                case 3:
                    return ClampedDerivationsForThreeKnots(controlPoints, leftDerivation, rightDerivation, difference);
                case 4:
                    return ClampedDerivationsForFourKnots(controlPoints, leftDerivation, rightDerivation, difference);
                case 5:
                    return ClampedDerivationsForFiveKnots(controlPoints, leftDerivation, rightDerivation, difference);
                //case 6:
                  //  return ClampedDerivationsForSixKnots(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], controlPoints[4], controlPoints[5], leftDerivation, rightDerivation);
                default:
                    return ClampedDerivationsForNKnots(controlPoints, leftDerivation, rightDerivation, difference);
            }
        }

        internal double[] ClampedDerivations2(double[] controlPoints, double leftDerivation, double rightDerivation, double difference)
        {
            var r = controlPoints.Length;
            int K;
            int k0;
      
            if (r % 2 == 0)
            {
                K = 3 / 2 + (r - 5) / 2;
                k0 = 2 + 2 * K;
            }
            else
            {
                K = 1 + (r - 5) / 2;
                k0 = 3 + 2 * K;
            }

            var eqLS = ArrayMyUtils.CreateMatrix<double>(K);
            var eqPS = new double[K];

            if (r >= 5)
            {
                //var k = 0;
                //var i = k * 2;
                eqLS[0][0] = HET;
             //   eqPS[0] = (3*(controlPoints[0]-controlPoints[4]) -12*(controlPoints[1] - controlPoints[3]) + )
            }
            throw new NotImplementedException();
        }

    }
}
