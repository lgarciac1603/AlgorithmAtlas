namespace AlgorithmAtlas.Models.Sorting;

/// <summary>
/// Running tally of the work a sorting algorithm performs. Comparisons and array
/// writes are the deterministic, machine-independent proxy for "time"; auxiliary
/// memory (see <see cref="AuxSpace"/> on the descriptor) is the "resource" axis.
/// </summary>
public sealed class SortMetrics
{
    public int Comparisons { get; private set; }
    public int Swaps { get; private set; }
    public int Writes { get; private set; }
    public int Reads { get; private set; }

    /// <summary>Total array operations — the headline "time" proxy (lower is faster).</summary>
    public int Operations => Comparisons + Writes;

    public void Apply(SortStep step)
    {
        switch (step.Kind)
        {
            case SortStepKind.Compare:
                Comparisons++;
                Reads += 2;
                break;
            case SortStepKind.Swap:
                Swaps++;
                Reads += 2;
                Writes += 2;
                break;
            case SortStepKind.Overwrite:
                Writes++;
                break;
        }
    }

    public void Reset()
    {
        Comparisons = Swaps = Writes = Reads = 0;
    }
}
