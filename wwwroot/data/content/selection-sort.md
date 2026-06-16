## How it works

Selection sort splits the array into a sorted left region and an unsorted right region. On each pass it scans the entire unsorted region to find the minimum element, then moves it to the front with a single swap.

1. Find the minimum element in the unsorted portion.
2. Swap it with the first unsorted position.
3. Advance the sorted boundary by one.
4. Repeat until the array is fully sorted.

## Why it matters

- Makes exactly **n − 1 swaps** regardless of input — useful when writes are expensive (e.g. flash memory).
- **Not stable** — a long-range swap can change the relative order of equal elements.
- Simple to reason about; performs fewer writes than bubble sort but the same number of comparisons.

## Pseudocode

```
selection_sort(a):
    for i from 0 to n-2:
        min = i
        for j from i+1 to n-1:
            if a[j] < a[min]: min = j
        if min != i: swap(a[i], a[min])
```
