## How it works

Bubble sort walks through the array repeatedly, comparing each pair of adjacent elements and swapping them if they are in the wrong order. After every full pass the largest unsorted element has "bubbled" to its final position at the right end.

1. Compare adjacent elements left to right.
2. Swap whenever the left element is greater than the right.
3. After each pass the rightmost unsorted element is in its final position.
4. Repeat until a full pass completes with no swaps (early exit).

## Why it matters

- **Simplest sorting algorithm** to understand and visualize.
- **Adaptive** — detects an already-sorted input in O(n) with the early-exit check.
- **Stable** — equal elements never swap, so their original relative order is preserved.
- Rarely used in practice due to O(n²) average performance, but ideal as a first algorithm to animate.

## Pseudocode

```
bubble_sort(a):
    for i from 0 to n-2:
        swapped = false
        for j from 0 to n-2-i:
            if a[j] > a[j+1]:
                swap(a[j], a[j+1])
                swapped = true
        if not swapped: break
```
