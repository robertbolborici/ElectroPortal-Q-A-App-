namespace ElectroPortal.DTOs
{
    public class CreateAnswerDto
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }
    }

}
