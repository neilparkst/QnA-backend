﻿using System.ComponentModel.DataAnnotations;

namespace backend.Data.Models
{
    public class AnswerPostFullRequest
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
    }
}
