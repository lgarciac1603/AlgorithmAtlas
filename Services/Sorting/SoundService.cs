using Microsoft.JSInterop;

namespace AlgorithmAtlas.Services.Sorting;

/// <summary>
/// Thin wrapper over the <c>sortAudio.js</c> module (JS isolation). Lazily imports the
/// module and forwards tone requests. All calls are no-ops when sound is disabled, so
/// callers don't need to branch. Registered as a scoped service.
/// </summary>
public sealed class SoundService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public SoundService(IJSRuntime js)
    {
        _js = js;
    }

    public bool Enabled { get; set; } = true;

    private async Task<IJSObjectReference?> ModuleAsync()
    {
        try
        {
            return _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/sortAudio.js");
        }
        catch
        {
            return null; // prerender / unsupported environment
        }
    }

    /// <summary>Unlock the audio context from a user gesture (e.g. the Run click).</summary>
    public async Task UnlockAsync()
    {
        if (!Enabled) return;
        var module = await ModuleAsync();
        if (module is not null)
        {
            await module.InvokeVoidAsync("unlock");
        }
    }

    /// <summary>Play a blip whose pitch tracks <paramref name="normalized"/> (0..1).</summary>
    public async Task ToneAsync(double normalized, string kind, int durationMs = 55)
    {
        if (!Enabled) return;
        var module = await ModuleAsync();
        if (module is not null)
        {
            await module.InvokeVoidAsync("tone", normalized, kind, durationMs);
        }
    }

    public async Task FinishChimeAsync()
    {
        if (!Enabled) return;
        var module = await ModuleAsync();
        if (module is not null)
        {
            await module.InvokeVoidAsync("finishChime");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); } catch { /* ignore */ }
        }
    }
}
