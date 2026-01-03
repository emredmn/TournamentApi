namespace TournamentApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Login için gerekli

        // İlişkiler
        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    }
}
