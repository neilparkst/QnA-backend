namespace backend.Data.Models
{
    public class QuestionGetManyResponse
    {
        public int QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public List<AnswerGetResponse> Answers { get; set; } = new List<AnswerGetResponse>();
    }
}
