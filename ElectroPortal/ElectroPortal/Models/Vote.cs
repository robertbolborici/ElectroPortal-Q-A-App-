namespace ElectroPortal.Models
{
    public class Vote
    {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; }
        public int AnswerId { get; set; }
        public bool Upvote { get; set; }
        public DateTime Date { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Answer Answer { get; set; }
    }
}
