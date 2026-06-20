## How it works

Quick sort is a **divide-and-conquer** algorithm that sorts in place. It picks a **pivot**, partitions the array so smaller values go left and larger values go right, then recursively sorts each partition.

1. Choose a pivot (here, the last element of the range).
2. **Partition:** scan through the range, swapping elements so everything less than the pivot ends up before it.
3. Place the pivot at the boundary — it is now in its final position.
4. Recurse on the left and right partitions.

## Why it matters

- **O(n log n) on average** and very cache-friendly, usually the fastest general-purpose sort in practice.
- **In-place** — only O(log n) stack space for recursion.
- **Worst case O(n²)** on already-sorted input with a naive pivot; good pivot choices (random / median-of-three) avoid it.

## Pseudocode

```
quick_sort(a, lo, hi):
    if lo >= hi: return
    p = partition(a, lo, hi)
    quick_sort(a, lo, p - 1)
    quick_sort(a, p + 1, hi)

partition(a, lo, hi):
    pivot = a[hi]
    i = lo
    for j from lo to hi-1:
        if a[j] < pivot:
            swap(a[i], a[j]); i = i + 1
    swap(a[i], a[hi])
    return i
```
