using AlgorithmAtlas.Models.Graphs;

namespace AlgorithmAtlas.Services.Graphs;

/// <summary>
/// Metadata describing a runnable graph search: how to run it, its complexity, and
/// whether it uses edge weights (which the visualizer shows as distance labels).
/// </summary>
public sealed record GraphDescriptor(
    string Id,
    string Name,
    Func<GraphModel, int, int, IEnumerable<GraphStep>> Run,
    string TimeComplexity,
    string SpaceComplexity,
    bool Weighted,
    string Color);

/// <summary>
/// Central catalog of the graph searches the visualizer can run. Adding one is a single
/// entry here; the detail page picks it up by id, exactly like <c>SortRegistry</c>.
/// </summary>
public static class GraphRegistry
{
    public static readonly IReadOnlyList<GraphDescriptor> All =
    [
        new("bfs", "Breadth-First Search", GraphAlgorithms.Bfs,
            "O(V + E)", "O(V)", Weighted: false, Color: "#42a5f5"),
        new("dfs", "Depth-First Search", GraphAlgorithms.Dfs,
            "O(V + E)", "O(V)", Weighted: false, Color: "#66bb6a"),
        new("dijkstra", "Dijkstra's Algorithm", GraphAlgorithms.Dijkstra,
            "O((V + E) log V)", "O(V)", Weighted: true, Color: "#ab47bc"),
        new("a-star", "A* Search", GraphAlgorithms.AStar,
            "O((V + E) log V)", "O(V)", Weighted: true, Color: "#ec407a"),
    ];

    private static readonly Dictionary<string, GraphDescriptor> ById =
        All.ToDictionary(d => d.Id);

    public static bool IsGraphAlgorithm(string id) => ById.ContainsKey(id);

    public static GraphDescriptor? Get(string id) =>
        ById.TryGetValue(id, out var d) ? d : null;
}
