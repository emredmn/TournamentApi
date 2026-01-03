// GraphQL/Mutation.cs
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TournamentApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Mutation
{
    // 1. REGISTER & LOGIN (JWT Oluşturma mantığı buraya eklenmeli)
    public async Task<string> Register(
        [Service] AppDbContext context, string email, string password, string firstName, string lastName)
    {
        var user = new User { Email = email, FirstName = firstName, LastName = lastName, PasswordHash = password }; // Şifreyi hashleyerek kaydetmelisin!
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return "User created";
    }

    public string Login([Service] AppDbContext context, [Service] IConfiguration config, string email, string password)
    {
        // 1. Kullanıcıyı veritabanında bul (Şifre hash kontrolü normalde yapılmalı, burada basit geçiyoruz)
        var user = context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) throw new Exception("User not found");

        // 2. Token oluştur
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123456789!!!")); // Program.cs'deki key ile AYNI olmalı
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID'yi Token'a gömüyoruz!
        new Claim(ClaimTypes.Email, user.Email)
    };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 2. TURNUVA YÖNETİMİ
    [Authorize]
    public async Task<Tournament> CreateTournament([Service] AppDbContext context, string name)
    {
        var t = new Tournament { Name = name, StartDate = DateTime.UtcNow, Status = "Created" };
        context.Tournaments.Add(t);
        await context.SaveChangesAsync();
        return t;
    }

    [Authorize]
    public async Task<bool> AddParticipant([Service] AppDbContext context, int tournamentId, int userId)
    {
        var tournament = await context.Tournaments.Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == tournamentId);
        var user = await context.Users.FindAsync(userId);

        if (tournament != null && user != null)
        {
            tournament.Participants.Add(user);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    // 3. START & BRACKET GENERATION (Diyagramdaki generateBracket mantığı)
    [Authorize]
    public async Task<Bracket> StartTournament([Service] AppDbContext context, int tournamentId)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null || tournament.Status != "Created") throw new Exception("Invalid Tournament");

        // Basit Fikstür Mantığı:
        var bracket = new Bracket { TournamentId = tournamentId };
        var participants = tournament.Participants.ToList();

        // Katılımcıları 2'li eşleştir (Basit versiyon)
        for (int i = 0; i < participants.Count; i += 2)
        {
            if (i + 1 < participants.Count)
            {
                bracket.Matches.Add(new Match
                {
                    Round = 1,
                    Player1 = participants[i],
                    Player2 = participants[i + 1]
                });
            }
            else
            {
                // Tek kalan kişi (Bye) logic'i buraya eklenebilir.
            }
        }

        tournament.Status = "Started";
        tournament.Bracket = bracket;

        context.Brackets.Add(bracket);
        await context.SaveChangesAsync();
        return bracket;
    }

    // 4. MAÇ OYNAMA (PLAY)
    [Authorize]
    public async Task<Match> PlayMatch([Service] AppDbContext context, int matchId, int winnerId)
    {
        var match = await context.Matches.FindAsync(matchId);
        if (match == null) throw new Exception("Match not found");

        match.WinnerId = winnerId;
        // Kazananı bir sonraki tura taşıma mantığı buraya eklenebilir.

        await context.SaveChangesAsync();
        return match;
    }
}