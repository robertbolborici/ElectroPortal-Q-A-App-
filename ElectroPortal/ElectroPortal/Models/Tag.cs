namespace ElectroPortal.Models
{
    public class Tag
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; } // Foreign key for Category

        // Navigation properties
        public Category Category { get; set; } // Each Tag belongs to one Category
        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
