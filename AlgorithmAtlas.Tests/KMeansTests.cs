using AlgorithmAtlas.Models.NeuralNetworks;
using AlgorithmAtlas.Services.NeuralNetworks;

namespace AlgorithmAtlas.Tests;

public class KMeansTests
{
    /// <summary>Three tight, well-separated blobs — k-means should recover them cleanly.</summary>
    private static double[][] ThreeBlobs(int seed = 1)
    {
        var rng = new Random(seed);
        var centers = new[] { (0.0, 0.0), (10.0, 10.0), (0.0, 10.0) };
        var points = new List<double[]>();
        foreach (var (cx, cy) in centers)
        {
            for (var i = 0; i < 40; i++)
            {
                points.Add([cx + rng.NextDouble() - 0.5, cy + rng.NextDouble() - 0.5]);
            }
        }
        return points.ToArray();
    }

    [Fact]
    public void Run_ConvergesAndEmitsSeedingFirst()
    {
        var states = KMeans.Run(ThreeBlobs(), k: 3, seed: 2).ToList();

        Assert.NotEmpty(states);
        Assert.Equal(0, states[0].Iteration);        // first state is the k-means++ seeding
        Assert.True(states[^1].Converged);           // last state converged
    }

    [Fact]
    public void Run_InertiaIsNonIncreasing()
    {
        var states = KMeans.Run(ThreeBlobs(), k: 3, seed: 2).ToList();
        for (var i = 1; i < states.Count; i++)
        {
            Assert.True(states[i].Inertia <= states[i - 1].Inertia + 1e-9,
                $"inertia rose at step {i}: {states[i - 1].Inertia} -> {states[i].Inertia}");
        }
    }

    [Fact]
    public void Run_SeparatesWellFormedBlobs()
    {
        var data = ThreeBlobs();
        var final = KMeans.Run(data, k: 3, seed: 2).Last();

        // Every one of the three 40-point blobs should end up in a single cluster.
        for (var b = 0; b < 3; b++)
        {
            var clustersInBlob = Enumerable.Range(b * 40, 40)
                .Select(i => final.Assignments[i])
                .Distinct()
                .Count();
            Assert.Equal(1, clustersInBlob);
        }
    }

    [Fact]
    public void Run_IsDeterministicForSameSeed()
    {
        var data = ThreeBlobs();
        var a = KMeans.Run(data, 3, seed: 7).Last();
        var b = KMeans.Run(data, 3, seed: 7).Last();
        Assert.Equal(a.Assignments, b.Assignments);
    }

    [Fact]
    public void Run_RejectsInvalidK()
    {
        var data = ThreeBlobs();
        Assert.Throws<ArgumentOutOfRangeException>(() => KMeans.Run(data, 0).ToList());
        Assert.Throws<ArgumentOutOfRangeException>(() => KMeans.Run(data, data.Length + 1).ToList());
    }
}

public class ExoplanetDatasetTests
{
    private const string Csv =
        "label,koi_period,koi_prad,koi_teq\n" +
        "1,9.48,2.26,793\n" +
        "0,19.9,14.6,638\n" +
        "1,2.52,2.75,1406\n" +
        "bad,row,skip,me\n" +   // unparseable label → skipped
        "0,1.73,33.46,1395\n";

    [Fact]
    public void Parse_ReadsRowsAndSkipsBadOnes()
    {
        var ds = ExoplanetDataset.Parse(Csv);
        Assert.Equal(4, ds.Count);
        Assert.Equal(3, ds.FeatureCount);
        Assert.Equal(new[] { "koi_period", "koi_prad", "koi_teq" }, ds.FeatureNames);
        Assert.Equal(new[] { 1, 0, 1, 0 }, ds.Labels);
    }

    [Fact]
    public void Standardize_ProducesZeroMeanUnitVariance()
    {
        var ds = ExoplanetDataset.Parse(Csv);
        var z = ds.Standardize();

        for (var c = 0; c < ds.FeatureCount; c++)
        {
            var mean = Enumerable.Range(0, ds.Count).Average(i => z[i][c]);
            var std = Math.Sqrt(Enumerable.Range(0, ds.Count).Average(i => Math.Pow(z[i][c] - mean, 2)));
            Assert.Equal(0, mean, 6);
            Assert.Equal(1, std, 6);
        }
    }

    [Fact]
    public void StratifiedSplit_PreservesEveryExampleOnce()
    {
        var ds = ExoplanetDataset.Parse(Csv);
        var (train, test) = ds.StratifiedSplit(0.25, seed: 1);
        Assert.Equal(ds.Count, train.Length + test.Length);
        Assert.Empty(train.Intersect(test));
    }
}
