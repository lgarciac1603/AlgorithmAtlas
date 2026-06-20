# AlgorithmAtlas

An interactive reference for algorithms and data structures, built with Blazor WebAssembly and MudBlazor. Every entry has an animated visualizer, complexity tables, and a markdown explanation.

## Running locally

```bash
dotnet watch run
```

The app opens at `https://localhost:5001` (or the port printed in the console).

## Running tests

```bash
dotnet test AlgorithmAtlas.Tests
```

## What's implemented

### Data Structures

| Name | Difficulty | Visualizer |
|---|---|---|
| Array | Easy | ✓ |
| Linked List | Easy | ✓ |
| Stack | Easy | ✓ |
| Queue | Easy | ✓ |
| Hash Table | Medium | ✓ |
| Graph | Medium | ✓ |
| Skip List | Hard | ✓ |

### Trees

| Name | Difficulty | Visualizer |
|---|---|---|
| Binary Search Tree | Medium | ✓ |
| AVL Tree | Hard | ✓ |
| Binary Heap | Medium | ✓ |
| Trie | Medium | ✓ |

### Sorting

| Name | Time | Difficulty | Visualizer |
|---|---|---|---|
| Bubble Sort | O(n²) | Easy | ✓ |
| Selection Sort | O(n²) | Easy | ✓ |
| Insertion Sort | O(n²) | Easy | ✓ |
| Merge Sort | O(n log n) | Medium | ✓ |
| Quick Sort | O(n log n) avg | Medium | ✓ |
| Heap Sort | O(n log n) | Medium | ✓ |
| Counting Sort | O(n + k) | Medium | ✓ |
| Radix Sort | O(d·(n+k)) | Medium | ✓ |

Sorting algorithms can also be compared side-by-side in the **Sorting Race** view (`/compare`).

## Adding a new algorithm

1. **`wwwroot/data/algorithms.json`** — add a JSON entry (id, name, category, difficulty, complexities, operations).
2. **`wwwroot/data/content/<id>.md`** — write the markdown explanation.
3. **`Visualizers/<Name>Visualizer.razor` + `.razor.css`** — build the interactive visualizer (optional but encouraged).
4. **`Pages/AlgorithmDetail.razor`** — add a `case "<id>"` to the visualizer switch and register the id in `HasVisualizer`.

## Tech stack

- [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/) — .NET 10
- [MudBlazor](https://mudblazor.com/) — Material Design component library
- [Markdig](https://github.com/xoofx/markdig) — Markdown rendering
- [xUnit](https://xunit.net/) — unit tests (`AlgorithmAtlas.Tests/`)
