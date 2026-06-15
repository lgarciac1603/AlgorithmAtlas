namespace AlgorithmAtlas.Models.Sorting;

/// <summary>
/// Final outcome of one algorithm run, reported by the visualizer when it finishes.
/// The comparison tool collects one of these per algorithm to rank the field.
/// </summary>
/// <param name="Id">Algorithm id.</param>
/// <param name="Name">Display name.</param>
/// <param name="Comparisons">Total comparisons performed.</param>
/// <param name="Swaps">Total swaps performed.</param>
/// <param name="Writes">Total array writes (swaps count as two).</param>
/// <param name="Operations">Comparisons + writes — the "time" proxy.</param>
/// <param name="Steps">Total instrumented steps (animation length).</param>
/// <param name="AuxSpaceRank">Auxiliary-memory rank (lower = lighter).</param>
public readonly record struct SortResult(
    string Id,
    string Name,
    int Comparisons,
    int Swaps,
    int Writes,
    int Operations,
    int Steps,
    int AuxSpaceRank);
