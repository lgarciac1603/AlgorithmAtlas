using AlgorithmAtlas.Models.NeuralNetworks;

namespace AlgorithmAtlas.Services.NeuralNetworks;

/// <summary>
/// Loads and caches the slim Kepler dataset used by the neural-network demos. Fetches the
/// CSV once over HTTP (Blazor WASM) and hands out the parsed <see cref="ExoplanetDataset"/>
/// plus its standardized feature matrix, so both visualizers share a single parse.
/// </summary>
public sealed class ExoplanetDataService
{
    private const string CsvPath = "data/exoplanets-koi.csv";

    private readonly HttpClient _http;
    private ExoplanetDataset? _dataset;
    private double[][]? _standardized;

    public ExoplanetDataService(HttpClient http) => _http = http;

    public async Task<ExoplanetDataset> GetDatasetAsync()
    {
        if (_dataset is null)
        {
            var csv = await _http.GetStringAsync(CsvPath);
            _dataset = ExoplanetDataset.Parse(csv);
            _standardized = _dataset.Standardize();
        }
        return _dataset;
    }

    /// <summary>The z-scored feature matrix aligned with <see cref="GetDatasetAsync"/> (same row order).</summary>
    public async Task<double[][]> GetStandardizedAsync()
    {
        await GetDatasetAsync();
        return _standardized!;
    }
}
