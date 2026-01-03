// GraphQL/Query.cs
using HotChocolate.Authorization;
using System.Security.Claims;
using TournamentApi.Models;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Tournament> GetTournaments([Service] AppDbContext context) => context.Tournaments;

    [Authorize]
    public IQueryable<Match> GetMyMatches([Service] AppDbContext context, ClaimsPrincipal claimsPrincipal)
    {
        // Token içindeki ID'yi al
        var userIdString = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Enumerable.Empty<Match>().AsQueryable();

        int userId = int.Parse(userIdString);

        // Kullanıcının Player1 veya Player2 olduğu maçları getir
        return context.Matches
            .Where(m => m.Player1Id == userId || m.Player2Id == userId);
    }
}