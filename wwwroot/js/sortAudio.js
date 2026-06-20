// Lightweight Web Audio helper for the sorting visualizer.
// Maps a value to a pitch and plays a short blip on compares/swaps.
// One shared AudioContext, created lazily on first user-initiated sound
// (browsers require a user gesture before audio can start).

let ctx = null;

function ensureContext() {
    if (!ctx) {
        const Ctor = window.AudioContext || window.webkitAudioContext;
        if (!Ctor) return null;
        ctx = new Ctor();
    }
    if (ctx.state === "suspended") {
        ctx.resume();
    }
    return ctx;
}

// value 0..1 (normalized) -> frequency in a pleasant range.
function freqFor(normalized) {
    const min = 220;   // A3
    const max = 880;   // A5
    return min + Math.max(0, Math.min(1, normalized)) * (max - min);
}

// Play a tone. `normalized` is the value's height (0..1); `kind` tweaks timbre.
export function tone(normalized, kind, durationMs) {
    const audio = ensureContext();
    if (!audio) return;

    const osc = audio.createOscillator();
    const gain = audio.createGain();

    osc.type = kind === "swap" ? "square" : "sine";
    osc.frequency.value = freqFor(normalized);

    const peak = kind === "swap" ? 0.06 : 0.035;
    const now = audio.currentTime;
    const dur = (durationMs || 60) / 1000;

    // quick attack + exponential release to avoid clicks
    gain.gain.setValueAtTime(0.0001, now);
    gain.gain.exponentialRampToValueAtTime(peak, now + 0.005);
    gain.gain.exponentialRampToValueAtTime(0.0001, now + dur);

    osc.connect(gain);
    gain.connect(audio.destination);
    osc.start(now);
    osc.stop(now + dur);
}

// A short rising arpeggio played when a sort finishes.
export function finishChime() {
    const audio = ensureContext();
    if (!audio) return;
    [0.25, 0.5, 0.75, 1.0].forEach((n, i) => {
        setTimeout(() => tone(n, "compare", 120), i * 70);
    });
}

// Unlock audio from a user gesture (call on the Run click).
export function unlock() {
    ensureContext();
}
