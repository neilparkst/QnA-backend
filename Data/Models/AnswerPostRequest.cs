using System.ComponentModel.DataAnnotations;

namespace backend.Data.Models
{
    public class AnswerPostRequest
    {
        [Required]
        public string Content { get; set; }
    }
}
