## How it works

Insertion sort builds the final sorted array one element at a time. It walks left to right; for each new element it slides it backwards over the already-sorted portion until it sits in the right place — much like sorting a hand of playing cards.

1. Start with the second element (the first is trivially "sorted").
2. Compare it with the elements to its left, shifting larger ones one slot to the right.
3. Drop the element into the gap once a smaller (or equal) value is found.
4. Repeat for every remaining element.

## Why it matters

- **Adaptive:** nearly-sorted data runs in close to O(n) — it is one of the fastest algorithms on small or almost-ordered inputs.
- **Stable** and **in-place** (O(1) extra memory).
- Often used as the base case inside faster divide-and-conquer sorts.

## Pseudocode

```
for i from 1 to n-1:
    key = a[i]
    j = i - 1
    while j >= 0 and a[j] > key:
        a[j+1] = a[j]
        j = j - 1
    a[j+1] = key
```
