## Overview

**Dijkstra's algorithm** (Edsger Dijkstra, 1959) finds the **least-cost path** from a source vertex to every other vertex in a graph with **non-negative** edge weights. Where BFS minimizes the *number of hops*, Dijkstra minimizes the *total weight* — the right tool when edges carry a distance, time, or cost.

## The Core Idea

Keep a tentative distance to every node (0 for the source, ∞ for the rest) and repeatedly **settle the closest unsettled node**. Because all weights are non-negative, once a node is settled its distance is final — no later path can improve it. That greedy guarantee is the heart of the algorithm.

## How It Works

A **priority queue** keyed by tentative distance gives the next-closest node in O(log V).

```
dijkstra(graph, source):
    dist[source] = 0, all others = ∞
    pq = {(0, source)}
    while pq not empty:
        u = pq.extract_min()          // closest unsettled node
        if u already settled: continue
        settle(u)
        for (v, w) in graph.neighbors(u):
            if dist[u] + w < dist[v]:  // relax the edge
                dist[v] = dist[u] + w
                parent[v] = u
                pq.push((dist[v], v))
```

**Relaxation** is the key operation: if going through `u` gives `v` a cheaper distance, update it. Recording `parent[v]` lets you reconstruct the path.

## Why Non-Negative Weights?

The "settled = final" invariant relies on the fact that extending a path can only *add* cost. A negative edge could make an already-settled node reachable more cheaply later, breaking the guarantee. For graphs with negative edges use **Bellman-Ford**.

## Complexity

| Implementation | Time |
|---|---|
| Binary heap (typical) | O((V + E) log V) |
| Array / linear scan | O(V²) |
| Fibonacci heap | O(E + V log V) |

Space is **O(V)** for the distance, parent and priority-queue structures.

## Use Cases

- Road navigation and GPS routing (shortest travel time/distance).
- Network routing protocols (e.g. OSPF link-state routing).
- Any least-cost path over non-negative weights.

## Common Pitfalls

- Running it on graphs with **negative edges** — use Bellman-Ford instead.
- Re-processing stale queue entries; guard with a "settled" check on extract.
- Confusing it with BFS: BFS is the special case where every edge weight is 1.
- Needing paths from *one* source to *one* target and wanting to prune the search early → **A\*** with a heuristic is usually faster.
