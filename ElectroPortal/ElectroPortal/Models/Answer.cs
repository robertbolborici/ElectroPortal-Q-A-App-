namespace ElectroPortal.Models
{
    public class Answer
    {
        public int Id { get; set; } // Primary key
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Question Question { get; set; }
        public ICollection<Vote> Votes { get; set; }
    }
}
