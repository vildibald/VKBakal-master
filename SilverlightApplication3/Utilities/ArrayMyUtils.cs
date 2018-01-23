using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace SilverlightApplication3
{
    public class ArrayMyUtils
    {
        public static int[] findValuesInArray<T>(T[] array, T[] values)
        {
            var results = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (values.Contains(array[i]))
                {
                    results.Add(i);
                }                  
            }
            return results.ToArray();
        }

        public static Boolean areValuesInArray<T>(T[] array, T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!array.Contains(values[i]))
                {
                    return false;
                }
            }
            return true;
        }


        internal static double[] GrowingArray(int length, double fromValue, double toValue)
        {
            var result = new double[length];
            var gap = (toValue - fromValue) / (double)(length-1);

            double offset = fromValue; 
            for (int i = 0; i < length; i++)
            {
                result[i] = offset;
                offset += gap;
            }
            return result;
        }

        public static Tuple<double[], int[]> FindNearestValuesAndIndexes(double[] numberArray, double[] valuesWeWantFind)
        {
            double[] results = new double[valuesWeWantFind.Length];
            int[] indexes = new int[valuesWeWantFind.Length];
            double diff = double.MaxValue;
            int j = 0;
            int i = 0;
            // int indexWhereKnotIsFound = 0;
            while (i < numberArray.Length)
            {
                if (j == valuesWeWantFind.Length)
                    break;

                double currDiff = Math.Abs(numberArray[i] - valuesWeWantFind[j]);
                if (currDiff < diff)
                {
                    diff = currDiff;
                    results[j] = numberArray[i];
                    indexes[j] = i;
                }
                else
                {
                    diff = double.MaxValue;
                    --i;
                    ++j;
                }

                ++i;
            }

            return new Tuple<double[], int[]>(results, indexes);
        }

        //public static int FindFirstIndexInSortedArrayWhichValueIsBiggerThanDesiredNumber(double[] numberList, double number)
        //{

        //    //for (int i = 0; i < numberArray.Length; i++)
        //    //{
        //    //    if (numberArray[i] > number) return i;
        //    //}
        //    //return -1;

        //    if (number > numberList.Last())
        //    {
        //        return -1;
        //    }
        //    else
        //    {
        //        int index;
        //        index = numberList.ToList().BinarySearch(number);
        //        if (index < 0)
        //        {
        //            return ~index;

        //        }
        //        else
        //        {
        //            return index;
        //        }
        //    }
        //}

        public static int FindFirstIndexInSortedListWhichValueIsBiggerThanDesiredNumber(List<double> numberList, double number)
        {

            //for (int i = 0; i < numberList.Count; i++)
            //{
            //    if (numberList[i] > number) return i;
            //}
            //return -1;
            if (number > numberList.Last())
            {
               return -1;
            }
            else
            {
                int index; 
                index = numberList.BinarySearch(number);
                if (index < 0)
                {
                    return ~index;
                    
                }
                else
                {
                    return index;
                }
            }

        }

        public static void InsertIntoSortedList(List<double> numberList, double number)
    {
        if (number > numberList.Last())
        {
            numberList.Add(number);
        }
        else
        {
            int index;
            index = numberList.BinarySearch(number);
            if (index < 0)
            {
                numberList.Insert(~index,number);

            }
           
        }
    }

        public static void ArrayFill<T>(T[] arrayToFill, T fillValue)
        {
            // if called with a single value, wrap the value in an array and call the main function

            if (arrayToFill.Length == 1)
            {
                arrayToFill[0] = fillValue;
                return;
            }
            ArrayFill<T>(arrayToFill, new T[] { fillValue });
        }

        private static void ArrayFill<T>(T[] arrayToFill, T[] fillValue)
        {
            if (fillValue.Length >= arrayToFill.Length)
            {
                throw new ArgumentException("fillValue array length must be smaller than length of arrayToFill");
            }

            // set the initial array value
            Array.Copy(fillValue, arrayToFill, fillValue.Length);

            int arrayToFillHalfLength = arrayToFill.Length / 2;

            for (int i = fillValue.Length; i < arrayToFill.Length; i *= 2)
            {
                int copyLength = i;
                if (i > arrayToFillHalfLength)
                {
                    copyLength = arrayToFill.Length - i;
                }

                Array.Copy(arrayToFill, 0, arrayToFill, i, copyLength);
            }
        }

        public static void ReplaceInList<T>(List<T> source, List<T> dest, int destFrom)
        {

            for (int i = 0; i < source.Count; i++)
            {
                if (destFrom + i < dest.Count)
                {
                    dest[destFrom + i] = source[i];
                }
                else
                {
                    dest.Add(source[i]);
                }
            }
        }

        public static T[][] CreateMatrix<T>(int rowsAndColums)
        {
            return CreateMatrix<T>(rowsAndColums, rowsAndColums);
        }

        public static T[][] CreateMatrix<T>(int rows, int colums)
        {
            var matrix = new T[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new T[colums];
            }
            return matrix;
        }

        internal static T[][] ArrayToMatrix<T>(T[] array)
        {
            T[][] X = new T[array.Length][];
            for (int i = 0; i < array.Length; i++)
            {
                X[i] = new T[1];
                X[i][0] = array[i];
            }
            return X;
        }

        internal static T[] OneColumnMatrixToArray<T>(T[][] matrix)
        {
            T[] array = new T[matrix.Length];
            if (matrix[0].Length != 1)
            {
                throw new NumberOfMatrixColumnIsNotOneException();
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                array[i] = matrix[i][0];
            }

            return array;
        }

        internal static List<T> OneColumnMatrixToList<T>(T[][] matrix)
        {
            var list = new List<T>(matrix.Length);
            if (matrix[0].Length != 1)
            {
                throw new NumberOfMatrixColumnIsNotOneException();
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                list.Add(matrix[i][0]);
            }

            return list;
        }

        internal static T[] GetNthValues<T>(T[] array, int n)
        {
            int resultsLength = array.Length / n;
            var results = new T[resultsLength];
            
            var jndex = 0;
            for (int i = 0; i < resultsLength; i++)
            {
                results[i] = array[jndex];
                jndex+=n;
            }

            return results;
        }
    }
}
