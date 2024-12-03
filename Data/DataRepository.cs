using Dapper;
using Microsoft.Data.SqlClient;

using backend.Data.Models;

namespace backend.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public AnswerGetResponse GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<AnswerGetResponse>(
                    @"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @answerId", new { answerId }
                );
            }
        }

        public QuestionGetSingleResponse GetQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var question = connection.QueryFirstOrDefault<QuestionGetSingleResponse>(
                    @"EXEC dbo.Question_GetSingle @QuestionId = @questionId", new { questionId }
                );

                if (question != null)
                {
                    question.Answers = connection.Query<AnswerGetResponse>(
                        @"EXEC dbo.Answer_Get_ByQuestionId @QuestionId = @questionId", new { questionId }
                    );
                }

                return question;
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany"
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany_BySearch @Search = @search", new { search }
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    "EXEC dbo.Question_GetUnanswered"
                );
            }
        }

        public bool QuestionExists(int questionId)
        {
            using ( var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<bool>(
                    @"EXEC dbo.Question_Exists @QuestionId = @questionId", new { questionId }
                );
            }
        }
    }
}
