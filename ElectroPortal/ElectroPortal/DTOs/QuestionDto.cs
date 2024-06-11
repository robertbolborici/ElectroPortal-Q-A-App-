namespace ElectroPortal.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CategoryId { get; set; }
        public List<AnswerDto> Answers { get; set; }
        public List<TagDto> QuestionTags { get; set; }
    }
}
