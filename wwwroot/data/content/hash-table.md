## Overview

A **hash table** (or hash map) stores key-value pairs and uses a **hash function** to convert a key into an index in an underlying array (the **bucket array**). This makes lookup, insertion, and deletion average **O(1)**, regardless of how many items are stored.

## How It Works

1. A **hash function** takes a key and produces an integer (the hash code).
2. The hash code is reduced to a valid array index, usually with `index = hash(key) % capacity`.
3. The value is stored at `buckets[index]`.
4. To look up a key, the same hash function recomputes the index and checks that bucket.

### Collisions

Two different keys can hash to the same index — a **collision**. Common resolution strategies:

- **Chaining** — each bucket holds a linked list (or small array) of entries; collisions just append to the list.
- **Open addressing** — on collision, probe for the next free slot (linear probing, quadratic probing, double hashing).

### Resizing

When the table gets too full (load factor exceeds a threshold, e.g. 0.75), it's **resized**: a bigger bucket array is allocated and every entry is rehashed into it. This is the source of the occasional O(n) cost, amortized to O(1).

## Common Operations

| Operation          | Average | Worst Case | Why                                      |
|---------------------|---------|------------|--------------------------------------------|
| Insert              | O(1)    | O(n)       | Worst case: all keys collide in one bucket  |
| Lookup              | O(1)    | O(n)       | Same — degenerates to a list scan           |
| Delete              | O(1)    | O(n)       | Same                                          |

## Use Cases

- Implementing dictionaries/maps/sets in most languages (`Dictionary<K,V>`, Python `dict`, JS `Map`/`Object`).
- Caching (memoization) — mapping inputs already computed to their results.
- Counting frequencies (e.g. word counts, detecting duplicates).
- Database indexing.

## Pseudocode — Insert with Chaining

```
function insert(table, key, value):
    index = hash(key) % table.capacity
    bucket = table.buckets[index]
    for entry in bucket:
        if entry.key == key:
            entry.value = value
            return
    bucket.append((key, value))
```

## Common Pitfalls

- A poor hash function that clusters keys into few buckets, degrading performance toward O(n).
- Forgetting that **iteration order is not guaranteed** in most hash table implementations.
- Mutating a key's hash-relevant fields after it has been inserted (the entry becomes "lost").
