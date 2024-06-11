namespace ElectroPortal.DTOs
{
    public class QuestionUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
    }
}
