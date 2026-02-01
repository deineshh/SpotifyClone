using SpotifyClone.Catalog.Domain.Aggregates.Moods.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Moods;

public sealed class Mood : AggregateRoot<MoodId, Guid>
{
    public string Name { get; private set; } = null!;
    public MoodCoverImage Cover { get; private set; } = null!;

    public static Mood Create(MoodId id, string name, MoodCoverImage cover)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(cover);

        MoodNameRules.Validate(name);

        return new Mood(id, name, cover);
    }

    public void ChangeName(string name)
    {
        MoodNameRules.Validate(name);
        Name = name;
    }

    private Mood(MoodId id, string name, MoodCoverImage cover)
        : base(id)
    {
        Name = name;
        Cover = cover;
    }

    private Mood()
    {
    }
}
