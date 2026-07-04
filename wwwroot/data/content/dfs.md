## Overview

**Depth-First Search (DFS)** explores a graph by going *as deep as possible* along each branch before backtracking. From a node it dives into the first unvisited neighbor, then that node's first unvisited neighbor, and so on; when it hits a dead end it backtracks to the most recent node with an unexplored branch.

## How It Works

DFS is naturally **recursive** — the call stack *is* the frontier. It can equally be written iteratively with an explicit **stack**.

```
dfs(graph, u, goal, visited):
    visited.add(u)
    if u == goal: return true
    for v in graph.neighbors(u):
        if v not in visited:
            parent[v] = u
            if dfs(graph, v, goal, visited):
                return true
    return false
```

The first path DFS finds to the goal is *a* valid path, but **not necessarily the shortest** — DFS commits to a branch and follows it to the end before considering alternatives.

## DFS vs BFS

Both run in **O(V + E)** and share the same skeleton; the only difference is the frontier data structure:

| | Frontier | Order | Shortest path? |
|---|---|---|---|
| **BFS** | Queue (FIFO) | Level by level | Yes (unweighted) |
| **DFS** | Stack / recursion | Deep first | No |

## Complexity

| | Cost |
|---|---|
| Time | O(V + E) |
| Space | O(V) — visited set + recursion/stack depth |

On a deep graph the recursion depth can reach O(V); very deep graphs may overflow the call stack, so an explicit stack is safer at scale.

## Use Cases

- **Cycle detection** in directed and undirected graphs.
- **Topological sort** of a DAG (dependency ordering).
- **Connected components** / flood fill.
- Maze generation and backtracking puzzles (Sudoku, N-Queens).

## Common Pitfalls

- Missing visited check → infinite recursion on cycles.
- Expecting a shortest path — DFS doesn't give one.
- Stack overflow on very deep graphs when using recursion; switch to an explicit stack.
