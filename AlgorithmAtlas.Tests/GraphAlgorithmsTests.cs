using AlgorithmAtlas.Models.Graphs;
using AlgorithmAtlas.Services.Graphs;

namespace AlgorithmAtlas.Tests;

public class GraphAlgorithmsTests
{
    private static readonly GraphScenario Scenario = GraphSamples.Pathfinding();
    private static GraphModel Graph => Scenario.Graph;
    private static int Start => Scenario.Start;
    private static int Goal => Scenario.Goal;

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Extract the reconstructed path (in order) from a step stream.</summary>
    private static List<int> PathOf(IEnumerable<GraphStep> steps) =>
        steps.Where(s => s.Kind == GraphStepKind.Path).Select(s => s.Node).ToList();

    private static int EdgeWeight(int a, int b) =>
        Graph.Neighbors(a).First(n => n.To == b).Weight;

    private static bool AreAdjacent(int a, int b) =>
        Graph.Neighbors(a).Any(n => n.To == b);

    private static int PathCost(IReadOnlyList<int> path)
    {
        var cost = 0;
        for (var i = 1; i < path.Count; i++) cost += EdgeWeight(path[i - 1], path[i]);
        return cost;
    }

    /// <summary>Brute-force shortest weighted path cost over all simple paths (small graph).</summary>
    private static int BruteForceMinCost(int start, int goal)
    {
        var best = int.MaxValue;
        var visited = new HashSet<int>();

        void Dfs(int u, int cost)
        {
            if (cost >= best) return;
            if (u == goal) { best = cost; return; }
            visited.Add(u);
            foreach (var (v, w) in Graph.Neighbors(u))
                if (visited.Add(v))
                {
                    Dfs(v, cost + w);
                    visited.Remove(v);
                }
            visited.Remove(u);
        }

        Dfs(start, 0);
        return best;
    }

    /// <summary>Independent BFS to compute the minimum hop count between two nodes.</summary>
    private static int MinHops(int start, int goal)
    {
        var dist = new Dictionary<int, int> { [start] = 0 };
        var queue = new Queue<int>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            if (u == goal) return dist[u];
            foreach (var (v, _) in Graph.Neighbors(u))
                if (!dist.ContainsKey(v))
                {
                    dist[v] = dist[u] + 1;
                    queue.Enqueue(v);
                }
        }
        return -1;
    }

    private static void AssertValidPath(IReadOnlyList<int> path)
    {
        Assert.NotEmpty(path);
        Assert.Equal(Start, path[0]);
        Assert.Equal(Goal, path[^1]);
        for (var i = 1; i < path.Count; i++)
            Assert.True(AreAdjacent(path[i - 1], path[i]),
                $"nodes {path[i - 1]} and {path[i]} are not adjacent");
        Assert.Equal(path.Count, path.Distinct().Count()); // no repeated node
    }

    // ── Common contract: every search ends with Done ──────────────────────────

    public static IEnumerable<object[]> AllSearches =>
    [
        [nameof(GraphAlgorithms.Bfs)],
        [nameof(GraphAlgorithms.Dfs)],
        [nameof(GraphAlgorithms.Dijkstra)],
        [nameof(GraphAlgorithms.AStar)],
    ];

    private static List<GraphStep> Run(string name) => name switch
    {
        nameof(GraphAlgorithms.Bfs) => GraphAlgorithms.Bfs(Graph, Start, Goal).ToList(),
        nameof(GraphAlgorithms.Dfs) => GraphAlgorithms.Dfs(Graph, Start, Goal).ToList(),
        nameof(GraphAlgorithms.Dijkstra) => GraphAlgorithms.Dijkstra(Graph, Start, Goal).ToList(),
        nameof(GraphAlgorithms.AStar) => GraphAlgorithms.AStar(Graph, Start, Goal).ToList(),
        _ => throw new ArgumentOutOfRangeException(nameof(name)),
    };

    [Theory]
    [MemberData(nameof(AllSearches))]
    public void Search_EndsWithDone(string name)
    {
        var steps = Run(name);
        Assert.Equal(GraphStepKind.Done, steps[^1].Kind);
    }

    [Theory]
    [MemberData(nameof(AllSearches))]
    public void Search_VisitsStartFirst(string name)
    {
        var steps = Run(name);
        var firstVisit = steps.First(s => s.Kind == GraphStepKind.Visit);
        Assert.Equal(Start, firstVisit.Node);
    }

    [Theory]
    [MemberData(nameof(AllSearches))]
    public void Search_VisitsGoal(string name)
    {
        var steps = Run(name);
        Assert.Contains(steps, s => s.Kind == GraphStepKind.Visit && s.Node == Goal);
    }

    [Theory]
    [MemberData(nameof(AllSearches))]
    public void Search_ProducesValidPath(string name)
    {
        AssertValidPath(PathOf(Run(name)));
    }

    [Theory]
    [MemberData(nameof(AllSearches))]
    public void Search_NeverVisitsSameNodeTwice(string name)
    {
        var visits = Run(name).Where(s => s.Kind == GraphStepKind.Visit).Select(s => s.Node).ToList();
        Assert.Equal(visits.Count, visits.Distinct().Count());
    }

    // ── BFS: fewest hops ──────────────────────────────────────────────────────

    [Fact]
    public void Bfs_FindsFewestHopPath()
    {
        var path = PathOf(GraphAlgorithms.Bfs(Graph, Start, Goal));
        AssertValidPath(path);
        Assert.Equal(MinHops(Start, Goal), path.Count - 1);
    }

    // ── Dijkstra & A*: optimal weighted cost ──────────────────────────────────

    [Fact]
    public void Dijkstra_FindsMinimumCostPath()
    {
        var path = PathOf(GraphAlgorithms.Dijkstra(Graph, Start, Goal));
        AssertValidPath(path);
        Assert.Equal(BruteForceMinCost(Start, Goal), PathCost(path));
    }

    [Fact]
    public void AStar_FindsMinimumCostPath()
    {
        var path = PathOf(GraphAlgorithms.AStar(Graph, Start, Goal));
        AssertValidPath(path);
        Assert.Equal(BruteForceMinCost(Start, Goal), PathCost(path));
    }

    [Fact]
    public void AStar_AndDijkstra_AgreeOnCost()
    {
        var dijkstra = PathCost(PathOf(GraphAlgorithms.Dijkstra(Graph, Start, Goal)));
        var aStar = PathCost(PathOf(GraphAlgorithms.AStar(Graph, Start, Goal)));
        Assert.Equal(dijkstra, aStar);
    }

    [Fact]
    public void AStar_ExpandsNoMoreNodesThanDijkstra()
    {
        int Visited(IEnumerable<GraphStep> s) => s.Count(x => x.Kind == GraphStepKind.Visit);
        var dijkstra = Visited(GraphAlgorithms.Dijkstra(Graph, Start, Goal));
        var aStar = Visited(GraphAlgorithms.AStar(Graph, Start, Goal));
        Assert.True(aStar <= dijkstra, $"A* visited {aStar}, Dijkstra visited {dijkstra}");
    }

    // ── Dijkstra weights: relaxations are monotone from the source ────────────

    [Fact]
    public void Dijkstra_StartHasZeroDistance()
    {
        var steps = GraphAlgorithms.Dijkstra(Graph, Start, Goal).ToList();
        var startDiscover = steps.First(s => s.Kind == GraphStepKind.Discover && s.Node == Start);
        Assert.Equal(0, startDiscover.Distance);
    }

    // ── Same start and goal ───────────────────────────────────────────────────

    [Fact]
    public void Bfs_StartEqualsGoal_PathIsSingleNode()
    {
        var path = PathOf(GraphAlgorithms.Bfs(Graph, Start, Start));
        Assert.Equal([Start], path);
    }

    // ── Graph model invariants ────────────────────────────────────────────────

    [Fact]
    public void Graph_EdgesAreUndirected()
    {
        foreach (var e in Graph.Edges)
        {
            Assert.True(AreAdjacent(e.From, e.To));
            Assert.True(AreAdjacent(e.To, e.From));
            Assert.Equal(e.Weight, EdgeWeight(e.From, e.To));
            Assert.Equal(e.Weight, EdgeWeight(e.To, e.From));
        }
    }

    [Fact]
    public void Graph_NeighborsAreSortedById()
    {
        foreach (var n in Graph.Nodes)
        {
            var ids = Graph.Neighbors(n.Id).Select(x => x.To).ToList();
            var sorted = ids.OrderBy(x => x).ToList();
            Assert.Equal(sorted, ids);
        }
    }

    [Fact]
    public void Graph_HeuristicIsAdmissible()
    {
        // Straight-line distance must never exceed the true shortest-path cost.
        foreach (var n in Graph.Nodes)
        {
            var trueCost = BruteForceMinCost(n.Id, Goal);
            if (trueCost == int.MaxValue) continue; // unreachable
            Assert.True(Graph.StraightLine(n.Id, Goal) <= trueCost + 1e-9,
                $"heuristic from {n.Id} overestimates: {Graph.StraightLine(n.Id, Goal)} > {trueCost}");
        }
    }
}
