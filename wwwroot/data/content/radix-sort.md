## How it works

Radix sort processes integers digit by digit, from least significant to most significant. At each digit position it applies a stable counting sort over the 10 possible digits (0–9). Because counting sort is stable, the relative order established by earlier passes is preserved by later ones.

1. Find the maximum value to determine how many digit positions to process.
2. For each digit position (ones, tens, hundreds, …):
   - Count occurrences of each digit (0–9) at that position.
   - Compute prefix sums to derive target output indices.
   - Place every element in the output array (reverse traversal for stability) and copy back.

## Why it matters

- **O(d · (n + k))** time where *d* is the number of digits and *k = 10* — effectively O(n) for fixed-width integers.
- **Stable** — preserves relative order of equal keys at each pass.
- **Not in-place** — needs O(n + k) auxiliary memory per pass.
- Faster than comparison-based sorts on large datasets of bounded integers, but memory access patterns are less cache-friendly than quick sort in practice.

## Pseudocode

```
radix_sort(a):
    max = max(a)
    exp = 1
    while max / exp > 0:
        counting_sort_by_digit(a, exp)
        exp *= 10

counting_sort_by_digit(a, exp):
    count[0..9] = 0
    for x in a:
        count[(x / exp) % 10]++
    for i from 1 to 9:
        count[i] += count[i-1]
    for i from n-1 down to 0:
        output[count[(a[i] / exp) % 10] - 1] = a[i]
        count[(a[i] / exp) % 10]--
    copy output → a
```
