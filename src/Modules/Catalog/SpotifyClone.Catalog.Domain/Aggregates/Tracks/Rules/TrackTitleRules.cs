using System.Text.RegularExpressions;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Exceptions;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks.Rules;

public static partial class TrackTitleRules
{
    public const short MaxLength = 128;

    public static void Validate(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (!TrackTitleRegex().IsMatch(title))
        {
            throw new InvalidTrackTitleDomainException("Title contains invalid characters.");
        }

        if (title.Length > MaxLength)
        {
            throw new InvalidTrackTitleDomainException($"Title exceeds maximum length of {MaxLength} characters.");
        }
    }

    // This regex allows letters (a-z, A-Z), numbers (0-9), spaces, and common punctuation like .,?!-_
    [GeneratedRegex(@"^[a-zA-Z0-9\s.,?!-_]*$")]
    private static partial Regex TrackTitleRegex();
}
