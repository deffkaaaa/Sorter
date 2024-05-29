using System;

public static class Introsort
{
    public static int maxRecursionDepth = 0;

    internal static int[] IntroSort(int[] array, int low, int high, int depthLimit, Action<int[]> drawArray, int recursionDepth)
    {
        maxRecursionDepth = Math.Max(maxRecursionDepth, recursionDepth + 1);

        if (high - low < 16)
        {
            InsertionSort(array, low, high, drawArray);
        }
        else if (depthLimit == 0)
        {
            HeapSort(array, low, high, drawArray);
        }
        else
        {
            int pivot = Partition(array, low, high, drawArray);
            IntroSort(array, low, pivot, depthLimit - 1, drawArray, recursionDepth + 1);
            IntroSort(array, pivot + 1, high, depthLimit - 1, drawArray, recursionDepth + 1);
        }

        return array;
    }

    internal static int Partition(int[] array, int low, int high, Action<int[]> drawArray)
    {
        int pivot = array[low];
        int left = low - 1;
        int right = high + 1;

        while (left < right)
        {
            do
            {
                left++;
            } while (array[left] < pivot);

            do
            {
                right--;
            } while (array[right] > pivot);

            if (left >= right) return right;

            Swap(array, left, right);
            drawArray(array);
        }
        return right;
    }

    internal static void InsertionSort(int[] array, int low, int high, Action<int[]> drawArray)
    {
        for (int i = low + 1; i <= high; i++)
        {
            int key = array[i];
            int j = i - 1;

            while (j >= low && array[j] > key)
            {
                array[j + 1] = array[j];
                j--;
                drawArray(array);
            }

            array[j + 1] = key;
            drawArray(array);
        }
    }

    internal static void HeapSort(int[] array, int low, int high, Action<int[]> drawArray)
    {
        int n = high - low + 1;

        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(array, n, i, low, drawArray);

        for (int i = n - 1; i > 0; i--)
        {
            Swap(array, low, low + i);
            drawArray(array);

            Heapify(array, i, 0, low, drawArray);
        }
    }

    internal static void Heapify(int[] array, int n, int i, int low, Action<int[]> drawArray)
    {
        int largest = i;
        int l = 2 * i + 1;
        int r = 2 * i + 2;

        if (l < n && array[low + l] > array[low + largest])
            largest = l;

        if (r < n && array[low + r] > array[low + largest])
            largest = r;

        if (largest != i)
        {
            Swap(array, low + i, low + largest);
            drawArray(array);

            Heapify(array, n, largest, low, drawArray);
        }
    }

    internal static void Swap(int[] array, int i, int j)
    {
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}
