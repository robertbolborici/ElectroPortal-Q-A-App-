namespace ElectroPortal.DTOs
{
    public class AnswerWithVotesDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }
        public string UserName { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int UserVote { get; set; }
    }
}
