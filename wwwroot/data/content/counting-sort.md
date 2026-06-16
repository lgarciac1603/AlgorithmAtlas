## How it works

Counting sort avoids comparisons entirely. It counts how many times each value appears, computes prefix sums to determine final positions, then places every element directly into its correct index.

1. Scan the array to find the maximum value *k*.
2. Allocate a count array of size *k + 1* and tally occurrences of each value.
3. Compute prefix sums: each bucket now stores the last output index for its value.
4. Walk the input in reverse and place each element at `count[value] - 1`, then decrement that counter (reverse traversal keeps the sort stable).
5. Copy the output array back to the original.

## Why it matters

- **O(n + k)** time — beats every comparison-based sort when *k = O(n)*.
- **Stable** — equal elements preserve their original relative order.
- **Not in-place** — requires O(n + k) auxiliary memory for the output and count arrays.
- Only applicable to non-negative integers with a known bounded range; the range *k* must be small relative to *n* for the linear bound to matter.

## Pseudocode

```
counting_sort(a):
    k = max(a)
    count[0..k] = 0

    for x in a:
        count[x]++

    for i from 1 to k:
        count[i] += count[i-1]

    for i from n-1 down to 0:
        output[count[a[i]] - 1] = a[i]
        count[a[i]]--

    copy output → a
```
