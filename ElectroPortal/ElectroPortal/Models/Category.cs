namespace ElectroPortal.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public ICollection<Question> Questions { get; set; }
        public ICollection<Tag> Tags { get; set; } // Each Category has many Tags
    }
}
