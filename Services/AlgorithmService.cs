using System.Net.Http.Json;
using AlgorithmAtlas.Models;

namespace AlgorithmAtlas.Services;

public class AlgorithmService
{
    private readonly HttpClient _http;
    private List<Algorithm>? _cache;

    public AlgorithmService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Algorithm>> GetAlgorithmsAsync()
    {
        return _cache ??= await _http.GetFromJsonAsync<List<Algorithm>>("data/algorithms.json") ?? [];
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var algorithms = await GetAlgorithmsAsync();
        return algorithms.Select(a => a.Category).Distinct().ToList();
    }

    public async Task<List<Algorithm>> GetByCategoryAsync(string category)
    {
        var algorithms = await GetAlgorithmsAsync();
        return algorithms.Where(a => a.Category == category).ToList();
    }

    public async Task<Algorithm?> GetByIdAsync(string id)
    {
        var algorithms = await GetAlgorithmsAsync();
        return algorithms.FirstOrDefault(a => a.Id == id);
    }

    public async Task<string?> GetContentAsync(string id)
    {
        var response = await _http.GetAsync($"data/content/{id}.md");
        return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
    }
}
