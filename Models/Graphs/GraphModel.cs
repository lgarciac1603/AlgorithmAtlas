namespace AlgorithmAtlas.Models.Graphs;

/// <summary>A graph vertex with a label and a 2-D position (percentages, 0–100) for layout.</summary>
public sealed record GraphNode(int Id, string Label, double X, double Y);

/// <summary>An undirected weighted edge. <see cref="Weight"/> is derived from the node distance.</summary>
public sealed record GraphEdge(int From, int To, int Weight);

/// <summary>
/// An undirected, weighted graph with positioned vertices. Edge weights are the
/// (scaled, rounded-up) straight-line distance between endpoints, which keeps the
/// straight-line <see cref="StraightLine"/> heuristic admissible for A* — it never
/// overestimates the real cost, so A* stays optimal.
/// </summary>
public sealed class GraphModel
{
    private const double DistanceScale = 0.1;

    private readonly Dictionary<int, GraphNode> _nodes;
    private readonly Dictionary<int, List<(int To, int Weight)>> _adj;

    public IReadOnlyList<GraphNode> Nodes { get; }
    public IReadOnlyList<GraphEdge> Edges { get; }

    public GraphModel(IReadOnlyList<GraphNode> nodes, IReadOnlyList<(int From, int To)> connections)
    {
        Nodes = nodes;
        _nodes = nodes.ToDictionary(n => n.Id);
        _adj = nodes.ToDictionary(n => n.Id, _ => new List<(int, int)>());

        var edges = new List<GraphEdge>();
        foreach (var (from, to) in connections)
        {
            var w = Math.Max(1, (int)Math.Ceiling(StraightLine(from, to)));
            _adj[from].Add((to, w));
            _adj[to].Add((from, w));
            edges.Add(new GraphEdge(from, to, w));
        }

        // Deterministic neighbour order → reproducible (and testable) traversals.
        foreach (var list in _adj.Values)
        {
            list.Sort((x, y) => x.To.CompareTo(y.To));
        }
        Edges = edges;
    }

    public GraphNode Node(int id) => _nodes[id];

    public IReadOnlyList<(int To, int Weight)> Neighbors(int id) => _adj[id];

    /// <summary>Scaled straight-line distance between two nodes — an admissible A* heuristic.</summary>
    public double StraightLine(int a, int b)
    {
        var na = _nodes[a];
        var nb = _nodes[b];
        var dx = na.X - nb.X;
        var dy = na.Y - nb.Y;
        return Math.Sqrt(dx * dx + dy * dy) * DistanceScale;
    }
}

/// <summary>A ready-to-run graph together with the start and goal vertices for a search.</summary>
public sealed record GraphScenario(GraphModel Graph, int Start, int Goal);

/// <summary>Fixed demo graphs the visualizer and tests share.</summary>
public static class GraphSamples
{
    /// <summary>
    /// A small road-map-like graph with several distinct paths from A to G, chosen so
    /// BFS (fewest hops), Dijkstra (least weight) and A* (guided) explore it differently.
    /// </summary>
    public static GraphScenario Pathfinding()
    {
        GraphNode[] nodes =
        [
            new(0, "A", 8, 50),
            new(1, "B", 28, 18),
            new(2, "C", 30, 82),
            new(3, "D", 52, 46),
            new(4, "E", 72, 16),
            new(5, "F", 74, 82),
            new(6, "G", 93, 50),
        ];

        (int, int)[] connections =
        [
            (0, 1), (0, 2), (1, 3), (2, 3), (1, 4),
            (2, 5), (3, 4), (3, 5), (4, 6), (5, 6),
        ];

        return new GraphScenario(new GraphModel(nodes, connections), Start: 0, Goal: 6);
    }
}
