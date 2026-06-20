## How it works

Merge sort is a **divide-and-conquer** algorithm. It splits the array into halves until each piece has one element, then merges the pieces back together in sorted order.

1. **Divide:** split the array into two halves.
2. **Conquer:** recursively sort each half.
3. **Combine:** merge the two sorted halves by repeatedly taking the smaller front element.

## Why it matters

- **Guaranteed O(n log n)** in the best, average, and worst case — no degenerate inputs.
- **Stable**: equal elements keep their original order.
- The trade-off is **O(n) auxiliary memory** for the merge buffer, which makes it heavier on resources than in-place sorts.

## Pseudocode

```
merge_sort(a):
    if length(a) <= 1: return a
    mid = length(a) / 2
    left  = merge_sort(a[0..mid])
    right = merge_sort(a[mid..])
    return merge(left, right)

merge(l, r):
    result = []
    while l and r not empty:
        if l[0] <= r[0]: result.append(l.pop_front())
        else:            result.append(r.pop_front())
    append remaining of l and r
    return result
```
