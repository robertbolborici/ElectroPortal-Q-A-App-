namespace ElectroPortal.DTOs
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int UserVote { get; set; }
        public string UserName { get; set; }
    }
}
