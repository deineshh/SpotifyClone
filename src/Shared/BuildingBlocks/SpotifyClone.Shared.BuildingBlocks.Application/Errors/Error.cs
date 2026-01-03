namespace SpotifyClone.Shared.BuildingBlocks.Application.Errors;

public sealed record Error(string Code, string Description)
{
    public override string ToString()
        => $"{Code}: {Description}";
}
