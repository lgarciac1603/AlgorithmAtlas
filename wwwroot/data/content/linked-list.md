## Overview

A **linked list** is a chain of **nodes**. Each node stores a value and a pointer (reference) to the next node. Unlike arrays, the nodes don't need to live in contiguous memory — they can be scattered anywhere, connected only by pointers.

The most common variants are:

- **Singly linked list** — each node points to the next only.
- **Doubly linked list** — each node points to both the next and previous node.
- **Circular linked list** — the last node points back to the first.

## How It Works

- The list keeps a reference to the **head** (first node), and optionally a **tail** (last node).
- To reach the *k*-th element, you must walk node by node from the head — there's no direct addressing.
- Inserting or removing a node only requires updating a few pointers, **not shifting elements** like an array.

## Common Operations

| Operation             | Time  | Why                                              |
|------------------------|------|---------------------------------------------------|
| Access by index         | O(n) | Must traverse from the head                        |
| Search                  | O(n) | Same as access — sequential traversal              |
| Insert at head          | O(1) | Just repoint the head                              |
| Insert at tail          | O(1)*| O(1) only if a tail pointer is maintained          |
| Delete (with reference) | O(1) | Repoint neighboring pointers                       |

## Use Cases

- Implementing stacks, queues, and adjacency lists for graphs.
- Scenarios with frequent insertions/deletions at the ends (e.g. LRU caches with a doubly linked list).
- Undo/redo history, music playlists ("next"/"previous").

## Pseudocode — Insert at Head

```
function insertAtHead(list, value):
    newNode = Node(value)
    newNode.next = list.head
    list.head = newNode
```

## Pseudocode — Traverse

```
function traverse(list):
    current = list.head
    while current != null:
        process(current.value)
        current = current.next
```

## Common Pitfalls

- Losing the reference to the rest of the list when re-pointing `next` (always save it first).
- Forgetting to update the `tail` pointer after inserting/removing the last node.
- Null-pointer errors when traversing an empty list.
