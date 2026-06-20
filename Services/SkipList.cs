namespace AlgorithmAtlas.Services;

/// <summary>
/// Probabilistic multi-level linked list. O(log n) expected search, insert, and delete.
/// Kept free of Blazor dependencies so it can be exercised by unit tests directly.
/// </summary>
public sealed class SkipList
{
    public const int MaxLevel = 6;
    private const double P = 0.5;

    private readonly Node _head = new(MaxLevel);
    private int _level;
    private readonly Random _rng;

    public SkipList(Random? rng = null) => _rng = rng ?? Random.Shared;

    public int Level => _level;
    public int Count { get; private set; }

    public bool Contains(int value)
    {
        var cur = _head;
        for (var i = _level; i >= 0; i--)
        {
            while (cur.Fwd[i]?.Value < value) cur = cur.Fwd[i]!;
            if (cur.Fwd[i]?.Value == value) return true;
        }
        return false;
    }

    /// <summary>
    /// Insert <paramref name="value"/>. Returns the height assigned (1-based), or 0 if duplicate.
    /// </summary>
    public int Insert(int value)
    {
        var update = new Node[MaxLevel];
        var cur = _head;
        for (var i = _level; i >= 0; i--)
        {
            while (cur.Fwd[i]?.Value < value) cur = cur.Fwd[i]!;
            update[i] = cur;
        }
        if (cur.Fwd[0]?.Value == value) return 0;

        var newLevel = RandomLevel();
        if (newLevel > _level)
        {
            for (var i = _level + 1; i <= newLevel; i++) update[i] = _head;
            _level = newLevel;
        }

        var node = new Node(newLevel + 1) { Value = value };
        for (var i = 0; i <= newLevel; i++)
        {
            node.Fwd[i] = update[i].Fwd[i];
            update[i].Fwd[i] = node;
        }
        Count++;
        return newLevel + 1;
    }

    /// <summary>Delete <paramref name="value"/>. Returns true if it existed.</summary>
    public bool Delete(int value)
    {
        var update = new Node[MaxLevel];
        var cur = _head;
        for (var i = _level; i >= 0; i--)
        {
            while (cur.Fwd[i]?.Value < value) cur = cur.Fwd[i]!;
            update[i] = cur;
        }
        var target = cur.Fwd[0];
        if (target?.Value != value) return false;

        for (var i = 0; i <= _level; i++)
        {
            if (update[i].Fwd[i] != target) break;
            update[i].Fwd[i] = target.Fwd[i];
        }
        while (_level > 0 && _head.Fwd[_level] is null) _level--;
        Count--;
        return true;
    }

    /// <summary>Values at level 0 in ascending sorted order.</summary>
    public IEnumerable<int> Values()
    {
        var cur = _head.Fwd[0];
        while (cur is not null) { yield return cur.Value; cur = cur.Fwd[0]; }
    }

    private int RandomLevel()
    {
        var level = 0;
        while (level < MaxLevel - 1 && _rng.NextDouble() < P) level++;
        return level;
    }

    public sealed class Node
    {
        public int Value;
        public readonly Node?[] Fwd;
        public Node(int levels) => Fwd = new Node?[levels];
    }
}
