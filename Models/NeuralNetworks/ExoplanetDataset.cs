using System.Globalization;

namespace AlgorithmAtlas.Models.NeuralNetworks;

/// <summary>
/// The Kepler "objects of interest" data prepared for learning: a matrix of physical
/// transit/stellar features plus a binary label (1 = CONFIRMED exoplanet, 0 = FALSE POSITIVE).
///
/// Loaded from the slim <c>wwwroot/data/exoplanets-koi.csv</c> that the build script derives
/// from the raw NASA table. Deliberately excludes <c>koi_score</c> and the <c>koi_fpflag_*</c>
/// columns: those are downstream vetting verdicts that would let the model "cheat" instead of
/// learning from the physics of each candidate.
/// </summary>
public sealed class ExoplanetDataset
{
    public required string[] FeatureNames { get; init; }

    /// <summary>Raw feature values, <c>[sample][feature]</c>.</summary>
    public required double[][] Features { get; init; }

    /// <summary>Binary label per sample: 1 = CONFIRMED, 0 = FALSE POSITIVE.</summary>
    public required int[] Labels { get; init; }

    public int Count => Features.Length;
    public int FeatureCount => FeatureNames.Length;

    /// <summary>Parse the slim CSV (header: <c>label,feature1,…</c>). Rows with unparseable cells are skipped.</summary>
    public static ExoplanetDataset Parse(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2)
        {
            throw new FormatException("CSV has no data rows.");
        }

        var header = lines[0].Split(',');
        var featureNames = header[1..]; // column 0 is the label

        var features = new List<double[]>(lines.Length - 1);
        var labels = new List<int>(lines.Length - 1);

        for (var r = 1; r < lines.Length; r++)
        {
            var cells = lines[r].Split(',');
            if (cells.Length != header.Length)
            {
                continue;
            }

            if (!int.TryParse(cells[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var label))
            {
                continue;
            }

            var row = new double[featureNames.Length];
            var ok = true;
            for (var c = 0; c < featureNames.Length; c++)
            {
                if (!double.TryParse(cells[c + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out row[c]))
                {
                    ok = false;
                    break;
                }
            }
            if (!ok)
            {
                continue;
            }

            features.Add(row);
            labels.Add(label);
        }

        return new ExoplanetDataset
        {
            FeatureNames = featureNames,
            Features = features.ToArray(),
            Labels = labels.ToArray()
        };
    }

    /// <summary>
    /// Column-wise z-score standardization (mean 0, unit variance). Skewed astrophysical
    /// quantities (period, depth, insolation) are log-compressed first so a handful of giant
    /// values don't dominate the scale. Returns the standardized matrix ready for the network
    /// or k-means, both of which assume comparably-scaled inputs.
    /// </summary>
    public double[][] Standardize()
    {
        var n = Count;
        var d = FeatureCount;
        var work = new double[n][];
        for (var i = 0; i < n; i++)
        {
            work[i] = new double[d];
            for (var c = 0; c < d; c++)
            {
                var v = Features[i][c];
                work[i][c] = LogScaled(c) ? Math.Log10(Math.Max(v, 1e-6)) : v;
            }
        }

        for (var c = 0; c < d; c++)
        {
            double mean = 0;
            for (var i = 0; i < n; i++)
            {
                mean += work[i][c];
            }
            mean /= n;

            double variance = 0;
            for (var i = 0; i < n; i++)
            {
                var diff = work[i][c] - mean;
                variance += diff * diff;
            }
            var std = Math.Sqrt(variance / n);
            if (std < 1e-9)
            {
                std = 1;
            }

            for (var i = 0; i < n; i++)
            {
                work[i][c] = (work[i][c] - mean) / std;
            }
        }

        return work;
    }

    /// <summary>Heavy-tailed columns that benefit from a log transform before standardizing.</summary>
    private bool LogScaled(int column) => FeatureNames[column] is
        "koi_period" or "koi_depth" or "koi_insol" or "koi_model_snr" or "koi_prad";

    /// <summary>
    /// Deterministic stratified split into train/test index sets, preserving the class ratio.
    /// </summary>
    public (int[] train, int[] test) StratifiedSplit(double testFraction = 0.25, int seed = 1)
    {
        var rng = new Random(seed);
        var byClass = new Dictionary<int, List<int>>();
        for (var i = 0; i < Count; i++)
        {
            (byClass.TryGetValue(Labels[i], out var list) ? list : byClass[Labels[i]] = []).Add(i);
        }

        var train = new List<int>();
        var test = new List<int>();
        foreach (var indices in byClass.Values)
        {
            var shuffled = indices.OrderBy(_ => rng.Next()).ToArray();
            var testCount = (int)Math.Round(shuffled.Length * testFraction);
            for (var i = 0; i < shuffled.Length; i++)
            {
                (i < testCount ? test : train).Add(shuffled[i]);
            }
        }

        return (train.ToArray(), test.ToArray());
    }
}
