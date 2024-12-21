using backend.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Data
{
    public class QuestionCache : IQuestionCache
    {
        private IMemoryCache _cache { get; set; }
        public QuestionCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        private string GetCacheKey(int questionId) => $"Question-{questionId}";

        public QuestionGetSingleResponse? Get(int questionId)
        {
            QuestionGetSingleResponse? question;
            _cache.TryGetValue(GetCacheKey(questionId), out question);

            return question;
        }

        public void Remove(int questionId)
        {
            _cache.Remove(GetCacheKey(questionId));
        }

        public void Set(QuestionGetSingleResponse question)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cache.Set(GetCacheKey(question.QuestionId), question, cacheEntryOptions);
        }
    }
}
