using RedisUsage.Models.Questions;
using RedisUsage.Parameters;
using System;
using System.Threading.Tasks;

namespace RedisUsage.Services
{
    public interface IQuestionService
    {
        Task<PagedList<Question>> GetQuestionsAsync(QuestionDtoParameters parameters);
        Task<Question> GetQuestionAsync(Guid questionId);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(Question question);
    }
}
