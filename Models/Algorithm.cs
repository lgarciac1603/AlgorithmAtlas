namespace AlgorithmAtlas.Models;

public class Algorithm
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public string TimeComplexity { get; set; } = "";
    public string SpaceComplexity { get; set; } = "";
    public Dictionary<string, string>? Operations { get; set; }
}
