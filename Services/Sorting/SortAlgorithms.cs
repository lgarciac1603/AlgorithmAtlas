using AlgorithmAtlas.Models.Sorting;

namespace AlgorithmAtlas.Services.Sorting;

/// <summary>
/// Instrumented sorting algorithms. Each method clones its input and yields a stream
/// of <see cref="SortStep"/> deltas describing every comparison, swap and write it
/// performs. Replaying that stream on an identical copy of the input reproduces the
/// sort exactly, which lets the same trace drive both the animation and the metrics.
/// </summary>
public static class SortAlgorithms
{
    public static IEnumerable<SortStep> BubbleSort(int[] input)
    {
        var a = (int[])input.Clone();
        var n = a.Length;
        for (var i = 0; i < n - 1; i++)
        {
            var swapped = false;
            for (var j = 0; j < n - 1 - i; j++)
            {
                yield return SortStep.Compare(j, j + 1);
                if (a[j] > a[j + 1])
                {
                    (a[j], a[j + 1]) = (a[j + 1], a[j]);
                    yield return SortStep.Swap(j, j + 1);
                    swapped = true;
                }
            }
            yield return SortStep.MarkSorted(n - 1 - i);
            if (!swapped)
            {
                for (var k = 0; k < n - 1 - i; k++)
                {
                    yield return SortStep.MarkSorted(k);
                }
                yield break;
            }
        }
        if (n > 0)
        {
            yield return SortStep.MarkSorted(0);
        }
    }

    public static IEnumerable<SortStep> SelectionSort(int[] input)
    {
        var a = (int[])input.Clone();
        var n = a.Length;
        for (var i = 0; i < n - 1; i++)
        {
            var min = i;
            yield return SortStep.Pivot(i);
            for (var j = i + 1; j < n; j++)
            {
                yield return SortStep.Compare(min, j);
                if (a[j] < a[min])
                {
                    min = j;
                }
            }
            if (min != i)
            {
                (a[i], a[min]) = (a[min], a[i]);
                yield return SortStep.Swap(i, min);
            }
            yield return SortStep.MarkSorted(i);
        }
        if (n > 0)
        {
            yield return SortStep.MarkSorted(n - 1);
        }
    }

    public static IEnumerable<SortStep> InsertionSort(int[] input)
    {
        var a = (int[])input.Clone();
        var n = a.Length;
        if (n > 0)
        {
            yield return SortStep.MarkSorted(0);
        }
        for (var i = 1; i < n; i++)
        {
            yield return SortStep.Pivot(i);
            var key = a[i];
            var j = i - 1;
            while (j >= 0)
            {
                yield return SortStep.Compare(j, j + 1);
                if (a[j] <= key)
                {
                    break;
                }
                a[j + 1] = a[j];
                yield return SortStep.Overwrite(j + 1, a[j]);
                j--;
            }
            a[j + 1] = key;
            yield return SortStep.Overwrite(j + 1, key);
        }
    }

    public static IEnumerable<SortStep> MergeSort(int[] input)
    {
        var a = (int[])input.Clone();

        IEnumerable<SortStep> Sort(int lo, int hi)
        {
            if (lo >= hi)
            {
                yield break;
            }
            var mid = (lo + hi) / 2;
            foreach (var s in Sort(lo, mid)) yield return s;
            foreach (var s in Sort(mid + 1, hi)) yield return s;

            var left = a[lo..(mid + 1)];
            var right = a[(mid + 1)..(hi + 1)];
            int i = 0, j = 0, k = lo;
            while (i < left.Length && j < right.Length)
            {
                yield return SortStep.Compare(lo + i, mid + 1 + j);
                if (left[i] <= right[j])
                {
                    a[k] = left[i];
                    yield return SortStep.Overwrite(k, left[i]);
                    i++;
                }
                else
                {
                    a[k] = right[j];
                    yield return SortStep.Overwrite(k, right[j]);
                    j++;
                }
                k++;
            }
            while (i < left.Length)
            {
                a[k] = left[i];
                yield return SortStep.Overwrite(k, left[i]);
                i++; k++;
            }
            while (j < right.Length)
            {
                a[k] = right[j];
                yield return SortStep.Overwrite(k, right[j]);
                j++; k++;
            }
        }

        foreach (var s in Sort(0, a.Length - 1)) yield return s;
        for (var idx = 0; idx < a.Length; idx++)
        {
            yield return SortStep.MarkSorted(idx);
        }
    }

    public static IEnumerable<SortStep> QuickSort(int[] input)
    {
        var a = (int[])input.Clone();

        IEnumerable<SortStep> Sort(int lo, int hi)
        {
            if (lo > hi)
            {
                yield break;
            }
            if (lo == hi)
            {
                yield return SortStep.MarkSorted(lo);
                yield break;
            }

            // partition (Lomuto, pivot = last element)
            yield return SortStep.Pivot(hi);
            var pivot = a[hi];
            var p = lo;
            for (var j = lo; j < hi; j++)
            {
                yield return SortStep.Compare(j, hi);
                if (a[j] < pivot)
                {
                    if (p != j)
                    {
                        (a[p], a[j]) = (a[j], a[p]);
                        yield return SortStep.Swap(p, j);
                    }
                    p++;
                }
            }
            if (p != hi)
            {
                (a[p], a[hi]) = (a[hi], a[p]);
                yield return SortStep.Swap(p, hi);
            }
            yield return SortStep.MarkSorted(p);

            foreach (var s in Sort(lo, p - 1)) yield return s;
            foreach (var s in Sort(p + 1, hi)) yield return s;
        }

        foreach (var s in Sort(0, a.Length - 1)) yield return s;
    }

    public static IEnumerable<SortStep> HeapSort(int[] input)
    {
        var a = (int[])input.Clone();
        var n = a.Length;

        IEnumerable<SortStep> SiftDown(int root, int size)
        {
            while (true)
            {
                var child = 2 * root + 1;
                if (child >= size)
                {
                    yield break;
                }
                if (child + 1 < size)
                {
                    yield return SortStep.Compare(child, child + 1);
                    if (a[child + 1] > a[child])
                    {
                        child++;
                    }
                }
                yield return SortStep.Compare(root, child);
                if (a[root] >= a[child])
                {
                    yield break;
                }
                (a[root], a[child]) = (a[child], a[root]);
                yield return SortStep.Swap(root, child);
                root = child;
            }
        }

        for (var i = n / 2 - 1; i >= 0; i--)
        {
            foreach (var s in SiftDown(i, n)) yield return s;
        }
        for (var end = n - 1; end > 0; end--)
        {
            (a[0], a[end]) = (a[end], a[0]);
            yield return SortStep.Swap(0, end);
            yield return SortStep.MarkSorted(end);
            foreach (var s in SiftDown(0, end)) yield return s;
        }
        if (n > 0)
        {
            yield return SortStep.MarkSorted(0);
        }
    }
}
