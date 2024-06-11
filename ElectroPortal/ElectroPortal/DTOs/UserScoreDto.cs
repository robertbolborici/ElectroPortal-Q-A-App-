namespace ElectroPortal.DTOs
{
    public class UserScoreDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public bool HasReward { get; set; }
    }
}
