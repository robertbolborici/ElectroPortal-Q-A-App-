namespace ElectroPortal.Models
{
    public class Question
    {
        public int Id { get; set; } // Primary key
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Category Category { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
