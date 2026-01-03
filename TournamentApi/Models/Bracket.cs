namespace TournamentApi.Models
{
    public class Bracket
    {
        public int Id { get; set; }
        public int TournamentId { get; set; } // Parent Tournament
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
