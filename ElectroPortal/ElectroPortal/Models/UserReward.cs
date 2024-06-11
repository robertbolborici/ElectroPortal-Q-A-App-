using System.Text.Json.Serialization;

namespace ElectroPortal.Models
{
    public class UserReward
    {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; }
        public int RewardId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Reward? Reward { get; set; }
    }
}
