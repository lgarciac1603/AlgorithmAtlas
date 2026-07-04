namespace AlgorithmAtlas.Models.Graphs;

/// <summary>
/// The kind of action a graph search performed in a single instrumented step.
/// Steps are emitted as a stream so a visualizer can replay the search, staying in
/// sync with the algorithm that produced them.
/// </summary>
public enum GraphStepKind
{
    /// <summary>A node was discovered and added to the frontier (queue/stack/priority queue).</summary>
    Discover,

    /// <summary>A node was taken from the frontier and is now being processed.</summary>
    Visit,

    /// <summary>An edge improved a node's tentative distance (Dijkstra / A*).</summary>
    Relax,

    /// <summary>A node (and its incoming edge) belongs to the final reconstructed path.</summary>
    Path,

    /// <summary>The search finished.</summary>
    Done
}

/// <summary>
/// One instrumented action produced by a graph search. Immutable delta — the visualizer
/// applies it to its node/edge state.
/// </summary>
/// <param name="Kind">What happened.</param>
/// <param name="Node">The node the step is about.</param>
/// <param name="From">The node the edge came from, or -1 when there is no edge (e.g. the start).</param>
/// <param name="Distance">Tentative distance to <paramref name="Node"/>, for weighted searches.</param>
public readonly record struct GraphStep(GraphStepKind Kind, int Node, int From = -1, double Distance = 0)
{
    public static GraphStep Discover(int node, int from = -1, double distance = 0) => new(GraphStepKind.Discover, node, from, distance);
    public static GraphStep Visit(int node) => new(GraphStepKind.Visit, node);
    public static GraphStep Relax(int node, int from, double distance) => new(GraphStepKind.Relax, node, from, distance);
    public static GraphStep Path(int node, int from = -1) => new(GraphStepKind.Path, node, from);
    public static GraphStep Done() => new(GraphStepKind.Done, -1);
}
