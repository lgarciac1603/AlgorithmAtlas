using AlgorithmAtlas.Models.Graphs;

namespace AlgorithmAtlas.Services.Graphs;

/// <summary>
/// Instrumented graph searches. Each method walks a <see cref="GraphModel"/> from a
/// start vertex toward a goal and yields a stream of <see cref="GraphStep"/> deltas
/// describing every discovery, visit and relaxation, then the reconstructed path.
/// The same trace drives both the animation and the tests.
/// </summary>
public static class GraphAlgorithms
{
    /// <summary>Breadth-first search — explores level by level with a queue. Finds the fewest-hop path.</summary>
    public static IEnumerable<GraphStep> Bfs(GraphModel g, int start, int goal)
    {
        var visited = new HashSet<int> { start };
        var parent = new Dictionary<int, int>();
        var queue = new Queue<int>();
        queue.Enqueue(start);
        yield return GraphStep.Discover(start);

        var found = false;
        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            yield return GraphStep.Visit(u);
            if (u == goal)
            {
                found = true;
                break;
            }
            foreach (var (v, _) in g.Neighbors(u))
            {
                if (visited.Add(v))
                {
                    parent[v] = u;
                    queue.Enqueue(v);
                    yield return GraphStep.Discover(v, u);
                }
            }
        }

        if (found)
        {
            foreach (var s in Reconstruct(parent, start, goal)) yield return s;
        }
        yield return GraphStep.Done();
    }

    /// <summary>Depth-first search — explores as far as possible along each branch (recursively).</summary>
    public static IEnumerable<GraphStep> Dfs(GraphModel g, int start, int goal)
    {
        var visited = new HashSet<int>();
        var parent = new Dictionary<int, int>();
        var found = false;

        IEnumerable<GraphStep> Explore(int u)
        {
            visited.Add(u);
            yield return GraphStep.Visit(u);
            if (u == goal)
            {
                found = true;
                yield break;
            }
            foreach (var (v, _) in g.Neighbors(u))
            {
                if (found) yield break;
                if (visited.Contains(v)) continue;
                parent[v] = u;
                yield return GraphStep.Discover(v, u);
                foreach (var s in Explore(v)) yield return s;
            }
        }

        yield return GraphStep.Discover(start);
        foreach (var s in Explore(start)) yield return s;

        if (found)
        {
            foreach (var s in Reconstruct(parent, start, goal)) yield return s;
        }
        yield return GraphStep.Done();
    }

    /// <summary>Dijkstra's algorithm — least-cost path on a weighted graph, greedily settling the nearest node.</summary>
    public static IEnumerable<GraphStep> Dijkstra(GraphModel g, int start, int goal)
    {
        var dist = g.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var parent = new Dictionary<int, int>();
        var settled = new HashSet<int>();
        var pq = new PriorityQueue<int, double>();

        dist[start] = 0;
        pq.Enqueue(start, 0);
        yield return GraphStep.Discover(start, -1, 0);

        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            if (!settled.Add(u)) continue; // skip stale duplicate entries
            yield return GraphStep.Visit(u);
            if (u == goal) break;

            foreach (var (v, w) in g.Neighbors(u))
            {
                if (settled.Contains(v)) continue;
                var nd = dist[u] + w;
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    parent[v] = u;
                    pq.Enqueue(v, nd);
                    yield return GraphStep.Relax(v, u, nd);
                }
            }
        }

        foreach (var s in Reconstruct(parent, start, goal)) yield return s;
        yield return GraphStep.Done();
    }

    /// <summary>A* search — Dijkstra guided toward the goal by a straight-line heuristic, so it explores far less.</summary>
    public static IEnumerable<GraphStep> AStar(GraphModel g, int start, int goal)
    {
        var dist = g.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var parent = new Dictionary<int, int>();
        var settled = new HashSet<int>();
        var pq = new PriorityQueue<int, double>();

        dist[start] = 0;
        pq.Enqueue(start, g.StraightLine(start, goal));
        yield return GraphStep.Discover(start, -1, 0);

        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            if (!settled.Add(u)) continue;
            yield return GraphStep.Visit(u);
            if (u == goal) break;

            foreach (var (v, w) in g.Neighbors(u))
            {
                if (settled.Contains(v)) continue;
                var nd = dist[u] + w;
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    parent[v] = u;
                    pq.Enqueue(v, nd + g.StraightLine(v, goal)); // f = g + h
                    yield return GraphStep.Relax(v, u, nd);
                }
            }
        }

        foreach (var s in Reconstruct(parent, start, goal)) yield return s;
        yield return GraphStep.Done();
    }

    /// <summary>Walk parent pointers back from the goal and emit the path start → goal in order.</summary>
    private static IEnumerable<GraphStep> Reconstruct(IReadOnlyDictionary<int, int> parent, int start, int goal)
    {
        var path = new List<int> { goal };
        var cur = goal;
        while (cur != start)
        {
            if (!parent.TryGetValue(cur, out var p))
            {
                yield break; // goal unreachable — no path to draw
            }
            path.Add(p);
            cur = p;
        }
        path.Reverse();

        for (var i = 0; i < path.Count; i++)
        {
            var from = i == 0 ? -1 : path[i - 1];
            yield return GraphStep.Path(path[i], from);
        }
    }
}
