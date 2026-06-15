using AlgorithmAtlas.Models.Sorting;

namespace AlgorithmAtlas.Services.Sorting;

/// <summary>
/// Metadata describing a sortable algorithm: how to run it, its complexity, and how
/// much auxiliary memory it needs. <see cref="AuxSpaceRank"/> orders algorithms on the
/// "computer resources" axis (lower = lighter), independent of the operation count.
/// </summary>
public sealed record SortDescriptor(
    string Id,
    string Name,
    Func<int[], IEnumerable<SortStep>> Run,
    string TimeComplexity,
    string SpaceComplexity,
    int AuxSpaceRank,
    string Color,
    bool Stable);

/// <summary>
/// Central catalog of the sorting algorithms the visualizer and comparison tool can run.
/// Adding a new sort is a single entry here — every consumer is data-driven from it.
/// </summary>
public static class SortRegistry
{
    public static readonly IReadOnlyList<SortDescriptor> All =
    [
        new("bubble-sort", "Bubble Sort", SortAlgorithms.BubbleSort,
            "O(n²)", "O(1)", AuxSpaceRank: 0, Color: "#ef5350", Stable: true),
        new("selection-sort", "Selection Sort", SortAlgorithms.SelectionSort,
            "O(n²)", "O(1)", AuxSpaceRank: 0, Color: "#ffa726", Stable: false),
        new("insertion-sort", "Insertion Sort", SortAlgorithms.InsertionSort,
            "O(n²)", "O(1)", AuxSpaceRank: 0, Color: "#ffca28", Stable: true),
        new("merge-sort", "Merge Sort", SortAlgorithms.MergeSort,
            "O(n log n)", "O(n)", AuxSpaceRank: 2, Color: "#66bb6a", Stable: true),
        new("quick-sort", "Quick Sort", SortAlgorithms.QuickSort,
            "O(n log n) avg", "O(log n)", AuxSpaceRank: 1, Color: "#42a5f5", Stable: false),
        new("heap-sort", "Heap Sort", SortAlgorithms.HeapSort,
            "O(n log n)", "O(1)", AuxSpaceRank: 0, Color: "#ab47bc", Stable: false),
    ];

    private static readonly Dictionary<string, SortDescriptor> ById =
        All.ToDictionary(d => d.Id);

    public static bool IsSortable(string id) => ById.ContainsKey(id);

    public static SortDescriptor? Get(string id) =>
        ById.TryGetValue(id, out var d) ? d : null;
}
