## Overview

**A\*** ("A-star", Hart, Nilsson & Raphael, 1968) is an **informed** shortest-path search. It behaves like Dijkstra but adds a **heuristic** that estimates how far each node still is from the goal, steering exploration toward the target. With a good heuristic A* expands dramatically fewer nodes than Dijkstra while still returning an **optimal** path.

## The f = g + h Idea

A* orders its priority queue by

```
f(n) = g(n) + h(n)
```

- **g(n)** — the *known* cost from the start to `n` (exactly what Dijkstra tracks).
- **h(n)** — a *heuristic* estimate of the remaining cost from `n` to the goal.

Dijkstra is simply A* with `h(n) = 0`: no guidance, so it expands outward in all directions. A useful `h` biases the search along the straight line toward the goal.

## How It Works

```
a_star(graph, start, goal):
    g[start] = 0
    pq = {(h(start), start)}
    while pq not empty:
        u = pq.extract_min()          // lowest f = g + h
        if u == goal: return path
        for (v, w) in graph.neighbors(u):
            if g[u] + w < g[v]:        // relax
                g[v] = g[u] + w
                parent[v] = u
                pq.push((g[v] + h(v), v))
```

## Admissible & Consistent Heuristics

- **Admissible**: `h(n)` never *overestimates* the true remaining cost. This is what guarantees A* returns an optimal path. Straight-line (Euclidean) distance is admissible for maps because no real route is shorter than a straight line.
- **Consistent** (monotone): `h(u) ≤ w(u, v) + h(v)` for every edge. Consistency implies admissibility and lets A* settle each node once, like Dijkstra.

The closer `h` gets to the true cost (while staying admissible), the fewer nodes A* explores. If `h` equals the true remaining cost, A* walks straight to the goal.

## Complexity

Worst case is the same as Dijkstra — **O((V + E) log V)** time, **O(V)** space — but with an informative heuristic the *practical* number of expanded nodes is far smaller. A poor or zero heuristic degrades A* back into Dijkstra.

## Use Cases

- Pathfinding in games and robotics (grid and navmesh navigation).
- GPS routing with geographic straight-line heuristics.
- Puzzle solving (15-puzzle, Rubik's cube) with pattern-database heuristics.

## Common Pitfalls

- An **inadmissible** heuristic (overestimating) can yield a non-optimal path.
- Using a heuristic in the wrong units/scale than the edge costs — it must be comparable to `g`.
- Forgetting A* reduces to Dijkstra when `h = 0`; if you have no heuristic, use Dijkstra directly.
