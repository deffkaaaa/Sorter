using System;

public static class Quicksort
{
    public static int maxRecursionDepth = 0;
    internal static int[] Sort(int[] array, int low, int high, Action<int[]> drawArray, int recursionDepth)
    {

        if (low < high)
        {
            int pi = Partition(array, low, high, drawArray);
            maxRecursionDepth = Math.Max(maxRecursionDepth, recursionDepth + 1);
            Sort(array, low, pi - 1, drawArray, recursionDepth + 1);
            Sort(array, pi + 1, high, drawArray, recursionDepth + 1);
        }

        return array;
    }

    internal static int Partition(int[] array, int low, int high, Action<int[]> drawArray)
    {
        int pivot = array[high];
        int i = (low - 1);

        for (int j = low; j <= high - 1; j++)
        {
            if (array[j] < pivot)
            {
                i++;
                Swap(array, i, j);
                drawArray(array);
            }
        }

        Swap(array, i + 1, high);
        drawArray(array);
        return i + 1;
    }

    internal static void Swap(int[] array, int i, int j)
    {
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}
