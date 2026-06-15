## How it works

Heap sort turns the array into a **binary max-heap** (every parent ≥ its children), then repeatedly removes the largest element and rebuilds the heap.

1. **Build a max-heap** from the unordered array.
2. Swap the root (the maximum) with the last element of the heap.
3. Shrink the heap by one and **sift down** the new root to restore the heap property.
4. Repeat until the heap is empty — the array is now sorted.

## Why it matters

- **Guaranteed O(n log n)** in all cases.
- **In-place** — O(1) auxiliary memory, unlike merge sort.
- **Not stable**, and its scattered memory access makes it usually slower in practice than quick sort despite the same complexity class.

## Pseudocode

```
heap_sort(a):
    build_max_heap(a)
    for end from n-1 down to 1:
        swap(a[0], a[end])
        sift_down(a, 0, end)

sift_down(a, root, size):
    while child = 2*root + 1 < size:
        if child+1 < size and a[child+1] > a[child]: child = child + 1
        if a[root] >= a[child]: break
        swap(a[root], a[child]); root = child
```
