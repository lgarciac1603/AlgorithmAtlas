using AlgorithmAtlas.Services;

namespace AlgorithmAtlas.Tests;

public class SkipListTests
{
    private static SkipList Make(int seed = 42) => new(new Random(seed));

    // ── Count & basic invariants ──────────────────────────────────────────────

    [Fact]
    public void Empty_CountIsZero()
    {
        var sl = Make();
        Assert.Equal(0, sl.Count);
    }

    [Fact]
    public void Insert_IncreasesCount()
    {
        var sl = Make();
        sl.Insert(10);
        sl.Insert(20);
        sl.Insert(30);
        Assert.Equal(3, sl.Count);
    }

    [Fact]
    public void Insert_Duplicate_CountUnchangedAndReturnsZero()
    {
        var sl = Make();
        sl.Insert(5);
        var result = sl.Insert(5);
        Assert.Equal(0, result);
        Assert.Equal(1, sl.Count);
    }

    [Fact]
    public void Insert_ReturnsHeight_AtLeastOne()
    {
        var sl = Make();
        var h = sl.Insert(42);
        Assert.True(h >= 1 && h <= SkipList.MaxLevel);
    }

    // ── Contains ─────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_AfterInsert_ReturnsTrue()
    {
        var sl = Make();
        sl.Insert(7);
        Assert.True(sl.Contains(7));
    }

    [Fact]
    public void Contains_NotInserted_ReturnsFalse()
    {
        var sl = Make();
        sl.Insert(7);
        Assert.False(sl.Contains(99));
    }

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        var sl = Make();
        Assert.False(sl.Contains(0));
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_ExistingValue_ReturnsTrue()
    {
        var sl = Make();
        sl.Insert(15);
        Assert.True(sl.Delete(15));
    }

    [Fact]
    public void Delete_ExistingValue_DecreasesCount()
    {
        var sl = Make();
        sl.Insert(15);
        sl.Delete(15);
        Assert.Equal(0, sl.Count);
    }

    [Fact]
    public void Delete_ExistingValue_NotContainedAfterwards()
    {
        var sl = Make();
        sl.Insert(15);
        sl.Delete(15);
        Assert.False(sl.Contains(15));
    }

    [Fact]
    public void Delete_NonExistent_ReturnsFalse()
    {
        var sl = Make();
        sl.Insert(10);
        Assert.False(sl.Delete(99));
    }

    [Fact]
    public void Delete_NonExistent_CountUnchanged()
    {
        var sl = Make();
        sl.Insert(10);
        sl.Delete(99);
        Assert.Equal(1, sl.Count);
    }

    // ── Order invariant ───────────────────────────────────────────────────────

    [Fact]
    public void Values_AreSortedAscending()
    {
        var sl = Make();
        int[] toInsert = [50, 10, 80, 30, 70, 20, 60, 40];
        foreach (var v in toInsert) sl.Insert(v);

        var values = sl.Values().ToList();
        for (var i = 1; i < values.Count; i++)
            Assert.True(values[i] > values[i - 1], $"Values not sorted at index {i}");
    }

    [Fact]
    public void Values_ContainsAllInserted()
    {
        var sl = Make();
        int[] toInsert = [3, 1, 4, 1, 5, 9, 2, 6]; // duplicate 1 is deduplicated
        foreach (var v in toInsert) sl.Insert(v);

        var values = sl.Values().ToList();
        int[] expected = [1, 2, 3, 4, 5, 6, 9];
        Assert.Equal(expected, values);
    }

    // ── Stress / bulk operations ──────────────────────────────────────────────

    [Fact]
    public void InsertMany_AllContained()
    {
        var sl = Make();
        var expected = Enumerable.Range(1, 50).ToList();
        foreach (var v in expected) sl.Insert(v);

        foreach (var v in expected)
            Assert.True(sl.Contains(v), $"Missing {v}");
    }

    [Fact]
    public void DeleteAll_EmptiesList()
    {
        var sl = Make();
        int[] values = [10, 20, 30, 40, 50];
        foreach (var v in values) sl.Insert(v);
        foreach (var v in values) sl.Delete(v);

        Assert.Equal(0, sl.Count);
        Assert.Empty(sl.Values());
    }

    [Fact]
    public void InsertAndDelete_RemainingItemsIntact()
    {
        var sl = Make();
        foreach (var v in new[] { 10, 20, 30, 40, 50 }) sl.Insert(v);
        sl.Delete(20);
        sl.Delete(40);

        Assert.Equal(3, sl.Count);
        Assert.True(sl.Contains(10));
        Assert.False(sl.Contains(20));
        Assert.True(sl.Contains(30));
        Assert.False(sl.Contains(40));
        Assert.True(sl.Contains(50));
    }

    // ── Level bounds ──────────────────────────────────────────────────────────

    [Fact]
    public void Level_NeverExceedsMaxLevel()
    {
        var sl = Make(seed: 0);
        for (var i = 1; i <= 200; i++) sl.Insert(i);
        Assert.True(sl.Level < SkipList.MaxLevel, $"Level {sl.Level} exceeded MaxLevel {SkipList.MaxLevel}");
    }

    [Fact]
    public void Level_DropsAfterDeletingTallNodes()
    {
        // Insert many values so higher levels likely exist, then delete all.
        var sl = Make(seed: 1);
        for (var i = 1; i <= 100; i++) sl.Insert(i);
        for (var i = 1; i <= 100; i++) sl.Delete(i);
        Assert.Equal(0, sl.Level);
    }
}
