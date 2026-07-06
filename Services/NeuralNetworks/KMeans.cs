namespace AlgorithmAtlas.Services.NeuralNetworks;

/// <summary>
/// A snapshot of Lloyd's algorithm after one iteration: where the centroids are and
/// which cluster each point currently belongs to. The visualizer replays these to
/// animate the centroids sliding into place — the same "stream of steps" idea the
/// graph searches use.
/// </summary>
/// <param name="Iteration">1-based iteration index (0 = the initial seeding).</param>
/// <param name="Centroids">Cluster centers, <c>[k][features]</c>.</param>
/// <param name="Assignments">Cluster index per data point.</param>
/// <param name="Inertia">Sum of squared distances of points to their centroid (lower is tighter).</param>
/// <param name="Converged">True once assignments stopped changing.</param>
public sealed record KMeansState(
    int Iteration,
    double[][] Centroids,
    int[] Assignments,
    double Inertia,
    bool Converged);

/// <summary>
/// Unsupervised clustering by Lloyd's algorithm with k-means++ seeding — no labels used.
/// Pure C# so it runs in the browser and under xUnit unchanged.
/// </summary>
public static class KMeans
{
    /// <summary>
    /// Cluster <paramref name="data"/> into <paramref name="k"/> groups, yielding one
    /// <see cref="KMeansState"/> per iteration (starting with the k-means++ seeding) until
    /// assignments stabilize or <paramref name="maxIterations"/> is reached.
    /// </summary>
    public static IEnumerable<KMeansState> Run(double[][] data, int k, int seed = 1, int maxIterations = 30)
    {
        if (data.Length == 0)
        {
            yield break;
        }
        if (k < 1 || k > data.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(k), "k must be between 1 and the number of points.");
        }

        var rng = new Random(seed);
        var dims = data[0].Length;
        var centroids = SeedPlusPlus(data, k, rng);

        var assignments = new int[data.Length];
        Array.Fill(assignments, -1);

        // Iteration 0: report the initial seeding with its first assignment.
        var (initialAssign, initialInertia) = Assign(data, centroids);
        assignments = initialAssign;
        yield return new KMeansState(0, Clone(centroids), (int[])assignments.Clone(), initialInertia, false);

        for (var iter = 1; iter <= maxIterations; iter++)
        {
            centroids = Recompute(data, assignments, centroids, k, dims);
            var (nextAssign, inertia) = Assign(data, centroids);

            var converged = nextAssign.SequenceEqual(assignments);
            assignments = nextAssign;

            yield return new KMeansState(iter, Clone(centroids), (int[])assignments.Clone(), inertia, converged);
            if (converged)
            {
                yield break;
            }
        }
    }

    /// <summary>k-means++ seeding: spread initial centers apart to avoid poor local minima.</summary>
    private static double[][] SeedPlusPlus(double[][] data, int k, Random rng)
    {
        var centroids = new double[k][];
        centroids[0] = (double[])data[rng.Next(data.Length)].Clone();

        var dist = new double[data.Length];
        Array.Fill(dist, double.PositiveInfinity);

        for (var c = 1; c < k; c++)
        {
            double total = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var d = SquaredDistance(data[i], centroids[c - 1]);
                if (d < dist[i])
                {
                    dist[i] = d;
                }
                total += dist[i];
            }

            // Pick the next center with probability proportional to squared distance.
            var target = rng.NextDouble() * total;
            var chosen = data.Length - 1;
            double running = 0;
            for (var i = 0; i < data.Length; i++)
            {
                running += dist[i];
                if (running >= target)
                {
                    chosen = i;
                    break;
                }
            }
            centroids[c] = (double[])data[chosen].Clone();
        }

        return centroids;
    }

    private static (int[] assignments, double inertia) Assign(double[][] data, double[][] centroids)
    {
        var assignments = new int[data.Length];
        double inertia = 0;
        for (var i = 0; i < data.Length; i++)
        {
            var best = 0;
            var bestDist = double.PositiveInfinity;
            for (var c = 0; c < centroids.Length; c++)
            {
                var d = SquaredDistance(data[i], centroids[c]);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c;
                }
            }
            assignments[i] = best;
            inertia += bestDist;
        }
        return (assignments, inertia);
    }

    private static double[][] Recompute(double[][] data, int[] assignments, double[][] previous, int k, int dims)
    {
        var sums = new double[k][];
        var counts = new int[k];
        for (var c = 0; c < k; c++)
        {
            sums[c] = new double[dims];
        }

        for (var i = 0; i < data.Length; i++)
        {
            var c = assignments[i];
            counts[c]++;
            var point = data[i];
            for (var d = 0; d < dims; d++)
            {
                sums[c][d] += point[d];
            }
        }

        var centroids = new double[k][];
        for (var c = 0; c < k; c++)
        {
            if (counts[c] == 0)
            {
                // Keep an emptied cluster where it was rather than collapsing it to the origin.
                centroids[c] = (double[])previous[c].Clone();
                continue;
            }
            centroids[c] = new double[dims];
            for (var d = 0; d < dims; d++)
            {
                centroids[c][d] = sums[c][d] / counts[c];
            }
        }
        return centroids;
    }

    private static double SquaredDistance(double[] a, double[] b)
    {
        double sum = 0;
        for (var i = 0; i < a.Length; i++)
        {
            var diff = a[i] - b[i];
            sum += diff * diff;
        }
        return sum;
    }

    private static double[][] Clone(double[][] rows) => rows.Select(r => (double[])r.Clone()).ToArray();
}
