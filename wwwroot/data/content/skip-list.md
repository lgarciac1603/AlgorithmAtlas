## Overview

A **skip list** (Pugh, 1990) is a probabilistic data structure that layers multiple sorted linked lists on top of one another. The bottom layer (level 0) contains every element. Each higher layer is a randomly chosen *express lane* — a sparser copy that lets you leap over many nodes at once. The result is O(log n) expected time for search, insert, and delete, achieved without the rotation machinery of balanced trees.

## The Core Idea

Imagine a sorted linked list. Searching it is O(n) because you must walk every node. Now imagine a second, sparser list of every other node sitting on top. You can scan the fast lane first, then drop down and finish the search in the slower lane — halving the work. Add more such layers and the search cost halves again with each level, giving O(log n) total.

```
Level 2:  head ────────────────────── 42 ─── ∞
Level 1:  head ──────── 17 ───────── 42 ─── ∞
Level 0:  head ── 7 ── 17 ── 31 ── 42 ── 55 ─── ∞
```

## Search

Start at the top-left. At each level, advance right as long as the next value is less than the target. When you can no longer advance, drop one level and repeat. Reaching level 0 either lands on the target or confirms it is absent.

```
search(target):
    cur = head
    for level = maxLevel down to 0:
        while cur.next[level] ≠ null AND cur.next[level].value < target:
            cur = cur.next[level]      // move right
        if cur.next[level].value == target:
            return FOUND
    return NOT FOUND
```

## Insert

Run the search procedure but record the last node visited at each level (`update[L]`). After the search, flip a biased coin to decide how tall the new node should be — each coin flip adds another level with probability p (typically 0.5). Wire the new node in at every level it reaches.

```
insert(value):
    build update[] via search
    height = random_level()          // coin flips decide tower height
    new_node = Node(value, height)
    for L = 0 to height - 1:
        new_node.next[L] = update[L].next[L]
        update[L].next[L] = new_node
```

## Delete

Search for the node, building `update[]` along the way. If found, unlink the node at every level it appears in.

## Complexity

| Operation  | Average    | Worst case |
|------------|------------|------------|
| Search     | O(log n)   | O(n)       |
| Insert     | O(log n)   | O(n)       |
| Delete     | O(log n)   | O(n)       |
| Space      | O(n log n) | O(n log n) |

The worst case is extremely unlikely with standard coin-flip probability p = 0.5; it requires an adversarial sequence of random outcomes, not adversarial input.

## Why Probabilistic?

Skip lists trade a deterministic balance guarantee (like AVL or Red-Black trees) for **simplicity**. There are no rotation cases, no balance factors, and no colour rules. The randomised tower heights naturally distribute the structure. With p = 0.5 and max level ⌈log₂ n⌉, the expected number of nodes examined per operation is about 2 log₂ n.

## Trade-offs vs. Balanced BSTs

| | Skip List | AVL / Red-Black |
|---|---|---|
| Balance mechanism | Probabilistic coin flip | Deterministic rotations |
| Implementation complexity | Low | Medium–High |
| Worst-case guarantee | None (negligible probability) | O(log n) |
| Cache friendliness | Poor (pointer chasing) | Poor (same) |
| Concurrent access | Easier to lock-free | Harder |

## Real-world Use

- **Redis** uses a skip list for sorted sets (`ZADD`, `ZRANGE`, etc.).
- **LevelDB / RocksDB** use a skip list for their in-memory write buffer (MemTable).
- **Java** `ConcurrentSkipListMap` provides a thread-safe sorted map via lock-free skip list.
