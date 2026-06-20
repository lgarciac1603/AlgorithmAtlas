namespace AlgorithmAtlas.Models.Sorting;

/// <summary>
/// The kind of action a sorting algorithm performed in a single instrumented step.
/// Steps are emitted as deltas so a visualizer can replay them on its own copy of
/// the array while staying in sync with the algorithm that produced them.
/// </summary>
public enum SortStepKind
{
    /// <summary>Two indices were read and compared.</summary>
    Compare,

    /// <summary>The values at two indices were swapped.</summary>
    Swap,

    /// <summary>A single index was overwritten with a value (shift / merge writes).</summary>
    Overwrite,

    /// <summary>An index is highlighted as the current pivot / key.</summary>
    Pivot,

    /// <summary>An index has reached its final sorted position.</summary>
    MarkSorted
}

/// <summary>
/// One instrumented action produced by a sorting algorithm. Immutable delta — the
/// visualizer applies it to its working array and the metric counters.
/// </summary>
/// <param name="Kind">What happened.</param>
/// <param name="I">Primary index.</param>
/// <param name="J">Secondary index (for compares/swaps); -1 when unused.</param>
/// <param name="Value">Value written, for <see cref="SortStepKind.Overwrite"/>.</param>
public readonly record struct SortStep(SortStepKind Kind, int I, int J = -1, int Value = 0)
{
    public static SortStep Compare(int i, int j) => new(SortStepKind.Compare, i, j);
    public static SortStep Swap(int i, int j) => new(SortStepKind.Swap, i, j);
    public static SortStep Overwrite(int i, int value) => new(SortStepKind.Overwrite, i, Value: value);
    public static SortStep Pivot(int i) => new(SortStepKind.Pivot, i);
    public static SortStep MarkSorted(int i) => new(SortStepKind.MarkSorted, i);
}
