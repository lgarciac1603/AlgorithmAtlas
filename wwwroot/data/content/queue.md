## Overview

A **queue** is a collection that follows the **FIFO** principle — *First In, First Out*. Elements are added at the **rear** and removed from the **front**, just like people waiting in line.

## How It Works

A queue supports:

- **Enqueue** — add an element to the rear.
- **Dequeue** — remove and return the element at the front.
- **Peek / Front** — look at the front element without removing it.

A naive array-based queue would need O(n) shifting on dequeue. Efficient implementations use a **circular buffer** or a **linked list with head and tail pointers** to get O(1) for both ends.

## Common Operations

| Operation | Time | Why                                       |
|-----------|------|---------------------------------------------|
| Enqueue   | O(1) | Add at the rear (tail pointer / circular buffer) |
| Dequeue   | O(1) | Remove at the front (head pointer)          |
| Peek      | O(1) | Direct access to the front element          |
| Search    | O(n) | Must traverse to find an item               |

## Variants

- **Circular queue** — reuses freed space at the front of an array buffer.
- **Deque (double-ended queue)** — allows insertion/removal at both ends.
- **Priority queue** — elements are dequeued by priority, not insertion order (typically backed by a heap).

## Use Cases

- **Breadth-First Search (BFS)** on graphs and trees.
- Task scheduling (CPU scheduling, print queues, message queues).
- Buffering data streams (I/O buffers, producer-consumer pipelines).

## Pseudocode — BFS Using a Queue

```
function bfs(graph, start):
    queue = [start]
    visited = { start }
    while queue is not empty:
        node = queue.dequeue()
        process(node)
        for neighbor in graph.neighbors(node):
            if neighbor not in visited:
                visited.add(neighbor)
                queue.enqueue(neighbor)
```

## Common Pitfalls

- Implementing a queue with a plain array and shifting elements on dequeue — degrades to O(n).
- Forgetting to mark nodes as visited in BFS, causing infinite loops on cyclic graphs.
- Confusing queue (FIFO) with stack (LIFO) order.
