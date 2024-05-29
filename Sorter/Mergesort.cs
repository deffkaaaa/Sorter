using System;

public class Mergesort
{
    public static int maxRecursionDepth = 0;

    internal static int[] Sort(int[] array, int left, int right, Action<int[]> drawArray, int recursionDepth)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            maxRecursionDepth = Math.Max(maxRecursionDepth, recursionDepth + 1);
            Sort(array, left, mid, drawArray, recursionDepth + 1);
            Sort(array, mid + 1, right, drawArray, recursionDepth + 1);
            Merge(array, left, mid, right, drawArray);
        }

        return array;
    }

    internal static void Merge(int[] array, int left, int mid, int right, Action<int[]> drawArray)
    {
        int leftSize = mid - left + 1;
        int rightSize = right - mid;

        int[] leftArray = new int[leftSize];
        int[] rightArray = new int[rightSize];

        Array.Copy(array, left, leftArray, 0, leftSize);
        Array.Copy(array, mid + 1, rightArray, 0, rightSize);

        int i = 0, j = 0, k = left;

        while (i < leftSize && j < rightSize)
        {
            if (leftArray[i] <= rightArray[j])
            {
                array[k] = leftArray[i];
                i++;
            }
            else
            {
                array[k] = rightArray[j];
                j++;
            }
            k++;
            drawArray(array);
        }

        while (i < leftSize)
        {
            array[k] = leftArray[i];
            i++;
            k++;
            drawArray(array);
        }

        while (j < rightSize)
        {
            array[k] = rightArray[j];
            j++;
            k++;
            drawArray(array);
        }
    }
}
