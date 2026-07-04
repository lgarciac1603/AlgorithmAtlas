## Overview

**Breadth-First Search (BFS)** explores a graph *level by level*: it visits the start node, then all of its neighbors, then all of *their* unvisited neighbors, and so on. Because it always expands the closest frontier first, BFS discovers nodes in non-decreasing order of hop-distance — which makes it the natural way to find the **shortest path in an unweighted graph**.

## How It Works

BFS maintains a **queue** (FIFO) of discovered-but-not-yet-processed nodes and a **visited** set so each node is enqueued at most once.

1. Mark the start visited and enqueue it.
2. Dequeue a node, process it.
3. For each unvisited neighbor: mark visited, record its parent, enqueue it.
4. Repeat until the queue is empty (or the goal is dequeued).

Recording each node's **parent** lets you rebuild the path by walking parents back from the goal.

```
bfs(graph, start, goal):
    visited = {start}
    queue = [start]
    while queue not empty:
        u = queue.dequeue()
        if u == goal: break
        for v in graph.neighbors(u):
            if v not in visited:
                visited.add(v)
                parent[v] = u
                queue.enqueue(v)
```

## Why the Queue Matters

The FIFO order is what guarantees level-by-level exploration. Swap the queue for a stack and you get **DFS** instead — same skeleton, completely different traversal order. Marking a node visited *when you enqueue it* (not when you dequeue it) prevents the same node being queued twice.

## Complexity

| | Cost |
|---|---|
| Time | O(V + E) — every vertex and edge examined once |
| Space | O(V) — visited set + queue |

## Use Cases

- Shortest path / fewest hops in **unweighted** graphs and grids.
- Web crawlers, social-network "degrees of separation".
- Flood fill, maze solving, connected components.
- Level-order traversal of trees.

## Common Pitfalls

- Forgetting the visited set → nodes revisited, or infinite loops on cycles.
- Marking visited on dequeue instead of enqueue → the same node enqueued many times.
- Using BFS for **weighted** shortest paths — it minimizes hop count, not cost. Use **Dijkstra** when edges have weights.
