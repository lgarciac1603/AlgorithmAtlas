## Overview

A **binary heap** is a complete binary tree stored efficiently as a plain array. The **heap property** determines the variant:

- **Min-heap** — every parent ≤ its children; the root is always the minimum.
- **Max-heap** — every parent ≥ its children; the root is always the maximum.

Because the tree is always complete, the array indices encode the structure directly — no pointers needed:

```
Parent of index i  →  (i − 1) / 2
Left child of i    →  2i + 1
Right child of i   →  2i + 2
```

## Operations

### Insert — O(log n)
1. Append the new value at the end of the array.
2. **Bubble up**: compare with parent; swap if out of order; repeat until the heap property is restored.

### Extract-Min — O(log n)
1. Save `arr[0]` (the minimum).
2. Move the last element to index 0.
3. Remove the last slot.
4. **Bubble down**: compare with the smaller child; swap if out of order; repeat.

### Peek — O(1)
Read `arr[0]` — always the minimum in a min-heap.

### Build Heap — O(n)
Start at the last internal node and sift down each node toward the root. This is faster than inserting one by one despite looking like O(n log n).

## Why it matters

- The most efficient data structure for a **priority queue**.
- Powers **Heap Sort** (Phase 1) and **Dijkstra's shortest path** (Phase 4).
- O(1) peek with O(log n) insert/remove — a sweet spot unavailable in sorted arrays or linked lists.

## Pseudocode

```
insert(heap, value):
    heap.append(value)
    bubble_up(heap, len(heap) - 1)

extract_min(heap):
    min = heap[0]
    heap[0] = heap.pop()
    bubble_down(heap, 0)
    return min
```
