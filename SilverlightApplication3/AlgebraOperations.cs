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
using System.Collections.Generic;

namespace SilverlightApplication3
{
    internal class AlgebraOperations
    {
        internal static double[][] MatrixInvert(double[][] matrix)
        {
            int n = matrix.Length;
            //e will represent each column in the identity matrix
            double[] e;
            //x will hold the inverse matrix to be returned
            double[][] invertedMatrix = new double[n][];

            for (int i = 0; i < n; i++)
            {
                invertedMatrix[i] = new double[matrix[i].Length];
            }
            /*
            * solve will contain the vector solution for the LUP decomposition as we solve
            * for each vector of x.  We will combine the solutions into the double[][] array x.
            * */
            double[] solve;

            //Get the LU matrix and P matrix (as an array)
            Tuple<double[][], int[]> results = LUPDecomposition(matrix);

            double[][] LU = results.Item1;
            int[] P = results.Item2;

            /*
            * Solve AX = e for each column ei of the identity matrix using LUP decomposition
            * */
            for (int i = 0; i < n; i++)
            {
                e = new double[matrix[i].Length];
                e[i] = 1;
                solve = SolveLUP(LU, P, e);
                for (int j = 0; j < solve.Length; j++)
                {
                    invertedMatrix[j][i] = solve[j];
                }
            }
            return invertedMatrix;
        }

        /*
* Given L,U,P and b solve for x.
* Input the L and U matrices as a single matrix LU.
* Return the solution as a double[].
* LU will be a n+1xm+1 matrix where the first row and columns are zero.
* This is for ease of computation and consistency with Cormen et al.
* pseudocode.
* The pi array represents the permutation matrix.
* */
        internal static double[] SolveLUP(double[][] LU, int[] pi, double[] b)
        {
            int n = LU.Length - 1;
            double[] x = new double[n + 1];
            double[] y = new double[n + 1];
            double suml = 0;
            double sumu = 0;
            double lij = 0;

            /*
            * Solve for y using formward substitution
            * */
            for (int i = 0; i <= n; i++)
            {
                suml = 0;
                for (int j = 0; j <= i - 1; j++)
                {
                    /*
                    * Since we've taken L and U as a singular matrix as an input
                    * the value for L at index i and j will be 1 when i equals j, not LU[i][j], since
                    * the diagonal values are all 1 for L.
                    * */
                    if (i == j)
                    {
                        lij = 1;
                    }
                    else
                    {
                        lij = LU[i][j];
                    }
                    suml = suml + (lij * y[j]);
                }
                y[i] = b[pi[i]] - suml;
            }
            //Solve for x by using back substitution
            for (int i = n; i >= 0; i--)
            {
                sumu = 0;
                for (int j = i + 1; j <= n; j++)
                {
                    sumu = sumu + (LU[i][j] * x[j]);
                }
                x[i] = (y[i] - sumu) / LU[i][i];
            }
            return x;
        }

        /*
* Perform LUP decomposition on a matrix A.
* Return L and U as a single matrix(double[][]) and P as an array of ints.
* We implement the code to compute LU "in place" in the matrix A.
* In order to make some of the calculations more straight forward and to 
* match Cormen's et al. pseudocode the matrix A should have its first row and first columns
* to be all 0.
* */
        internal static Tuple<double[][], int[]> LUPDecomposition(double[][] matrix)
        {
            int n = matrix.Length - 1;
            /*
            * pi represents the permutation matrix.  We implement it as an array
            * whose value indicates which column the 1 would appear.  We use it to avoid 
            * dividing by zero or small numbers.
            * */
            int[] pi = new int[n + 1];
            double p = 0;
            int kp = 0;
            int pik = 0;
            int pikp = 0;
            double aki = 0;
            double akpi = 0;

            //Initialize the permutation matrix, will be the identity matrix
            for (int j = 0; j <= n; j++)
            {
                pi[j] = j;
            }

            for (int k = 0; k <= n; k++)
            {
                /*
                * In finding the permutation matrix degree that avoids dividing by zero
                * we take a slightly different approach.  For numerical stability
                * We find the element with the largest 
                * absolute value of those in the current first column (column k).  If all elements in
                * the current first column are zero then the matrix is singluar and throw an
                * error.
                * */
                p = 0;
                for (int i = k; i <= n; i++)
                {
                    if (Math.Abs(matrix[i][k]) > p)
                    {
                        p = Math.Abs(matrix[i][k]);
                        kp = i;
                    }
                }
                if (p == 0)
                {
                    throw new Exception("singular matrix");
                }
                /*
                * These lines update the pivot array (which represents the pivot matrix)
                * by exchanging pi[k] and pi[kp].
                * */
                pik = pi[k];
                pikp = pi[kp];
                pi[k] = pikp;
                pi[kp] = pik;

                /*
                * Exchange rows k and kpi as determined by the pivot
                * */
                for (int i = 0; i <= n; i++)
                {
                    aki = matrix[k][i];
                    akpi = matrix[kp][i];
                    matrix[k][i] = akpi;
                    matrix[kp][i] = aki;
                }

                /*
                    * Compute the Schur complement
                    * */
                for (int i = k + 1; i <= n; i++)
                {
                    matrix[i][k] = matrix[i][k] / matrix[k][k];
                    for (int j = k + 1; j <= n; j++)
                    {
                        matrix[i][j] = matrix[i][j] - (matrix[i][k] * matrix[k][j]);
                    }
                }
            }
            return Tuple.Create(matrix, pi);
        }

        /*
* Multiply two matrices together and return the resulting matrix
* */
        internal static double[][] MultiplyMatrices(double[][] matrix1, double[][] matrix2)
        {

            double[][] solutionMatrix = new double[matrix1.Length][];
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                solutionMatrix[i] = new double[matrix2[0].Length];
            }

            if (matrix1[0].Length != matrix2.Length)
            {
                throw new Exception("Incompatable Dimensions");
            }
            else
            {

                for (int i = 0; i <= matrix1.Length - 1; i++)
                {
                    for (int j = 0; j <= matrix2[0].Length - 1; j++)
                    {
                        solutionMatrix[i][j] = 0;
                        for (int k = 0; k <= matrix1[0].Length - 1; k++)
                        {
                            solutionMatrix[i][j] = solutionMatrix[i][j] + matrix1[i][k] * matrix2[k][j];
                        }
                    }
                }
            }

            return solutionMatrix;
        }

        /*
* This method takes a list of matrices, the optimal parenthesis table s, and matrix index to start at
* i, and the matrix index to stop at j and multiplies them in an optimal manner and returns the result as
* a matrix.
* */
        internal static double[][] MultiplyOptimalParens(List<double[][]> matrices, int[][] s, int i, int j)
        {
            double[][] matrix1;
            double[][] matrix2;
            if (i == j)
                return matrices[i - 1];
            else
            {
                /*
                * Each entry s[i][j] records a value of k such that an optimal parenthesizaiton
                * of AiAi+1...Aj splits the product between Ak and Ak+1.  Thus the final matrix multiptication
                * in computing A1..n optimally is A1...s[1][n]As[1][n]+1...n.  We determine the earlier matrix 
                * muliplications recursively.
                * */
                matrix1 = MultiplyOptimalParens(matrices, s, i, s[i][j]);
                matrix2 = MultiplyOptimalParens(matrices, s, s[i][j] + 1, j);
                //multiply two matrices together
                double[][] multiply = MultiplyMatrices(matrix1, matrix2);
                return multiply;
            }
        }

        /*
* Given the relevant dimensions of a set of matrices return two tables m and s.
* m[i][j] is the minimum scalar multiplications for multiplying matrix i through matrix j.
* s[i][j] is the index k that achieves the optimnal cost in computing m[i][j]
* */
        internal static Tuple<int[][], int[][]> MatrixChainOrder(int[] p)
        {
            //Initialize the m and s tables
            int n = p.Length - 1;
            int[][] m = new int[n + 1][];
            int[][] s = new int[n + 1][];
            for (int i = 0; i <= n; i++)
            {
                m[i] = new int[n + 1];
                s[i] = new int[n + 1];
            }

            //set all the values in the m table to 0
            for (int i = 1; i <= n; i++)
            {
                m[i][i] = 0;
            }
            /*
            * Use recurrence to determine minimum scalar multiplications.  The first time through
            * it determines the costs for chains of length 2, the second time throught length 3, and 
            * so on.
            * */
            for (int l = 2; l <= n; l++)
            {
                for (int i = 1; i <= n - l + 1; i++)
                {
                    int j = i + l - 1;
                    //initialize m[i][j] to the maximim value of an int as a default
                    m[i][j] = int.MaxValue;
                    for (int k = i; k <= j - 1; k++)
                    {
                        int q = m[i][k] + m[k + 1][j] + (p[i - 1] * p[k] * p[j]);
                        if (q < m[i][j])
                        {
                            m[i][j] = q;
                            s[i][j] = k;
                        }
                    }
                }
            }

            return Tuple.Create(m, s);
        }

        /*
* The main method call for multiplying a set of matrices A1,A2,...,An
* Pass in the set of matrices as a generic list.
* */
        internal static double[][] MultiplyListOfMatrices(List<double[][]> matrices)
        {
            //degree holds the relevant dimensions of the matrices
            int[] p = new int[matrices.Count + 1];

            //Get the relevant dimensions of the matrices
            for (int i = 0; i <= matrices.Count; i++)
            {
                if (i == 0)
                {
                    //Get the row and column length of the first matrix
                    p[0] = matrices[0].Length;
                    p[1] = matrices[0][0].Length;
                }
                else
                {
                    //Get the column length of all other matrices
                    p[i] = matrices[i - 1][0].Length;
                }
            }

            //Pass in the degree array and get back two matrices.
            Tuple<int[][], int[][]> results = MatrixChainOrder(p);
            /*
            * The secod matrix returned s[i][j] records a value of k such that an optimal
            * parenthesization of AiAi+1...Aj splits the product between Ak and 
            * Ak+1.
            * */
            int[][] s = results.Item2;
            //matrixmultiply contains the final result of all the matrices multiplied together
            double[][] matrixmultiply = MultiplyOptimalParens(matrices, s, 1, matrices.Count);
            return matrixmultiply;
        }

        internal static double[][] TransposeMatrix(double[][] matrix)
        {
            double[][] transposedMatrix = new double[matrix[0].Length][];

            for (int i = 0; i < matrix[0].Length; i++)
                transposedMatrix[i] = new double[matrix.Length];
            
                for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix[0].Length; j++)
                        transposedMatrix[j][i] = matrix[i][j];

            return transposedMatrix;
        }


       

    

    }
}
