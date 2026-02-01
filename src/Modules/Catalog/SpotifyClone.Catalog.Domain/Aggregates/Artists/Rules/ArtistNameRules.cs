using System.Text.RegularExpressions;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Exceptions;

namespace SpotifyClone.Catalog.Domain.Aggregates.Artists.Rules;

public static class ArtistNameRules
{
    public const short MaxLength = 18;

    public static void Validate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!Regex.IsMatch(@"^[a-zA-Z0-9\s.,?!-_]*$", name))
        {
            throw new InvalidArtistNameDomainException("Name contains invalid characters.");
        }

        if (name.Length > MaxLength)
        {
            throw new InvalidArtistNameDomainException($"Name exceeds maximum length of {MaxLength} characters.");
        }
    }
}
