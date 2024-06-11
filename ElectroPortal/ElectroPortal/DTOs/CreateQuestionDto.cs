namespace ElectroPortal.DTOs
{
    public class CreateQuestionDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
    }

}
