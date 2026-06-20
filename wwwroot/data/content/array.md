## Overview

An **array** stores elements of the same type in a single, contiguous block of memory. Each element can be reached directly using its **index**, which is its position in the block (starting at 0).

Because the memory is contiguous, the address of any element can be computed instantly:

```
address(i) = base_address + i * element_size
```

This is why arrays give **O(1)** access — no traversal needed.

## How It Works

- The array reserves a block of memory big enough for all its elements.
- Reading or writing `arr[i]` jumps directly to that memory address.
- Inserting or deleting in the middle requires shifting every element after that position.
- Many languages use **dynamic arrays** (e.g. `List<T>` in C#, `ArrayList` in Java, Python `list`) that automatically resize by allocating a bigger block and copying elements when full.

## Common Operations

| Operation        | Time       | Why                                           |
|-------------------|-----------|-----------------------------------------------|
| Access by index   | O(1)      | Direct address calculation                     |
| Search (unsorted) | O(n)      | Must check elements one by one                 |
| Insert at end     | O(1)*     | Amortized — occasional resize costs O(n)       |
| Insert at middle  | O(n)      | Shift all following elements right             |
| Delete            | O(n)      | Shift all following elements left              |

## Use Cases

- Storing fixed-size collections (lookup tables, matrices, image pixels).
- As the underlying storage for other structures (stacks, queues, heaps, hash tables).
- Whenever fast random access by index matters more than fast insertion/deletion.

## Pseudocode — Insert at Index

```
function insertAt(array, index, value):
    shift all elements from index to end one position right
    array[index] = value
    size += 1
```

## Common Pitfalls

- Off-by-one errors when using 0-based indices.
- Forgetting that inserting/deleting in the middle of a large array is expensive — O(n).
- Confusing a fixed-size array with a dynamic array's amortized growth behavior.
