namespace ElectroPortal.Models
{
    public class QuestionTag
    {
        public int Id { get; set; } // Primary key
        public int QuestionId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public Question Question { get; set; }
        public Tag Tag { get; set; }
    }
}