## Overview

A **graph** is a set of **vertices** (nodes) connected by **edges**. Graphs model relationships and networks: social networks, road maps, web links, dependency graphs, and more.

Graphs can be:

- **Directed** (edges have a direction, A → B) or **undirected** (edges go both ways).
- **Weighted** (edges carry a cost/distance) or **unweighted**.
- **Cyclic** (contains cycles) or **acyclic** (e.g. a DAG — Directed Acyclic Graph).

## How It Works — Representations

| Representation     | Space     | Check edge (u, v) | Iterate neighbors of u |
|----------------------|-----------|---------------------|--------------------------|
| Adjacency matrix      | O(V²)     | O(1)                | O(V)                     |
| Adjacency list        | O(V + E)  | O(degree(u))        | O(degree(u))             |

- **Adjacency matrix**: a `V x V` table where `matrix[u][v] = 1` (or weight) if an edge exists. Simple, fast edge lookup, but wastes space on sparse graphs.
- **Adjacency list**: each vertex stores a list of its neighbors. Efficient for sparse graphs (most real-world graphs).

## Traversals

- **Breadth-First Search (BFS)** — explores level by level using a **queue**. Finds the shortest path in unweighted graphs.
- **Depth-First Search (DFS)** — explores as far as possible along each branch using a **stack** (or recursion). Useful for cycle detection, topological sorting, connected components.

Both run in **O(V + E)**.

## Common Operations

| Operation                  | Time        | Why                                  |
|-----------------------------|-------------|----------------------------------------|
| Add vertex                  | O(1)        | Append to vertex list                  |
| Add edge                    | O(1)        | Append to adjacency list                |
| BFS / DFS traversal          | O(V + E)    | Visit every vertex and edge once       |
| Check adjacency (matrix)     | O(1)        | Direct lookup                           |
| Check adjacency (list)       | O(degree)   | Scan neighbor list                     |

## Use Cases

- Shortest path (Dijkstra, Bellman-Ford, BFS for unweighted graphs).
- Network routing, social network analysis, recommendation systems.
- Dependency resolution (build systems, package managers) via topological sort.
- Maze solving, puzzle solving (flood fill, Lee algorithm).

## Pseudocode — DFS (Recursive)

```
function dfs(graph, node, visited):
    if node in visited:
        return
    visited.add(node)
    process(node)
    for neighbor in graph.neighbors(node):
        dfs(graph, neighbor, visited)
```

## Common Pitfalls

- Forgetting to mark nodes visited, causing infinite loops on cyclic graphs.
- Using an adjacency matrix for very large, sparse graphs — wastes memory.
- Mixing up directed vs. undirected edges when building the adjacency structure.
