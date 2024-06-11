namespace ElectroPortal.Models
{
    public class Reward
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUsed { get; set; }
    }
}
