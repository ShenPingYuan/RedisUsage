using Newtonsoft.Json;
using RedisUsage.Models.Questions;
using RedisUsage.Parameters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RedisUsage.Services
{
    public class QuestionService : IQuestionService
    {
        /// <summary>
        /// 题库相关的使用db 3
        /// </summary>
        private readonly int dbid = 3;
        /// <summary>
        /// 题目的Id列表
        /// </summary>
        private readonly string QuestionKeys = "QuestionKeys";
        public QuestionService()
        {
        }
        /// <summary>
        /// 获取多个题目
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<PagedList<Question>> GetQuestionsAsync(QuestionDtoParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            //asynchronous to get questions
            var questions = await Task.Run(() =>
            {
                //get all question.Ids
                RedisValue[] questionkeys = RedisHelper.ContentListGetAll(QuestionKeys, out long count, dbid);
                //get all questions
                List<Question> questionsInTask = new List<Question>();
                foreach (var key in questionkeys)
                {
                    var question = HashEntriesToQuestion(RedisHelper.ContentGet(key, dbid));
                    questionsInTask.Add(question);
                }
                return Task.FromResult(questionsInTask);
            });
            IQueryable<Question> questionsQuery = questions.AsQueryable();
            //filter
            if (parameters.PositionCategoryIds?.Count > 0) questionsQuery = questionsQuery.Where(q => parameters.PositionCategoryIds.Where(p => q.PositionCategoryIds.Contains(p)).Any());
            if (parameters.KnowledgeCategory != null) questionsQuery = questionsQuery.Where(q => string.Equals(q.KnowledgeCategoryName, parameters.KnowledgeCategory));
            if (parameters.QuestionTypes?.Count > 0) questionsQuery = questionsQuery.Where(q => parameters.QuestionTypes.Contains(q.QuestionType));
            if (parameters.StripLineCategoryId != null) questionsQuery = questionsQuery.Where(q => string.Equals(q.StripLineCategoryId, parameters.StripLineCategoryId));
            //query
            if (parameters.Q != null) questionsQuery = questionsQuery
                        .Where(q => q.Content.Contains(parameters.Q) || q.Creator.Contains(parameters.Q) || q.CreateTime.Contains(parameters.Q));

            if (parameters.IsRandom)
            {//disorder
                ListRandom(questions);
            }
            else
            {//order
                questionsQuery = questionsQuery.OrderBy(q => q.CreateTime);
            }
            //paging
            return PagedList<Question>.Create(questionsQuery.ToList(), parameters.PageNumber, parameters.PageSize);
        }
        /// <summary>
        /// 获取一个Question Entity
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<Question> GetQuestionAsync(Guid questionId)
        {
            if (questionId == Guid.Empty)
                throw new ArgumentNullException(nameof(questionId));

            return await Task.Run(() =>
            {
                HashEntry[] fields = RedisHelper.ContentGet(questionId.ToString(), dbid);
                return HashEntriesToQuestion(fields);
            });
        }
        /// <summary>
        /// 把一个Question Entity存入数据库
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task AddQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            question.Id = Guid.NewGuid();
            question.Options.ForEach(o =>
            {
                if (question == null)
                    o.QuestionId = question.Id;
            });
            List<HashEntry> fields = QuestionToHashEntryList(question);
            await Task.Run(() =>
            {
                RedisHelper.ContentSave(question.Id.ToString(), fields.ToArray(), dbid);
                RedisHelper.ContentListAdd(QuestionKeys, question.Id.ToString(), dbid);
            });
            question = await GetQuestionAsync(question.Id);
        }
        /// <summary>
        /// 更新一个Question Entity
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task UpdateQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            List<HashEntry> fields = QuestionToHashEntryList(question);

            await Task.Run(() => RedisHelper.ContentSave(question.Id.ToString(), fields.ToArray(), dbid));
        }
        /// <summary>
        /// 删除一个Question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public Task DeleteQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (question.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(question.Id));
            }

            return Task.Run(() =>
            {
                if (RedisHelper.ContentListDelete(QuestionKeys, question.Id.ToString(), dbid) == 1)
                    RedisHelper.ClearKeyContent(question.Id.ToString(), dbid);
            });
        }
        /// <summary>
        /// 把Question Entity转换成HashEntry List
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public List<HashEntry> QuestionToHashEntryList(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            var props = question.GetType().GetProperties();
            List<HashEntry> entries = new List<HashEntry>();
            List<Type> listTypes = new List<Type>() { typeof(List<string>), typeof(List<Option>) };
            foreach (var prop in props)
            {
                RedisValue value;
                if (listTypes.Contains(prop.PropertyType))
                {
                    value = JsonConvert.SerializeObject(prop.GetValue(question));
                }
                else
                {
                    value= prop.GetValue(question)?.ToString();
                }
                if (value != RedisValue.Null)
                {
                    entries.Add(new HashEntry(prop.Name, value));
                }
            }
            return entries;
        }
        /// <summary>
        /// 把HashEntry数组转换成Question
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Question HashEntriesToQuestion(HashEntry[] entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            if (entries.Count() == 0) return null;
            Question question = new Question();
            PropertyInfo[] props = question.GetType().GetProperties();
            foreach (var entry in entries)
            {
                var prop = props.FirstOrDefault(p => p.Name.Equals(entry.Name.ToString(), StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    List<Type> listTypes = new List<Type>() { typeof(List<string>), typeof(List<Option>) };
                    if (listTypes.Contains(prop.PropertyType))
                    {
                        object value = JsonConvert.DeserializeObject(entry.Value, prop.PropertyType);
                        prop.SetValue(question, value);
                    }
                    else
                    {
                        object value;
                        value = (prop.PropertyType == typeof(Guid))
                            ? Guid.Parse(entry.Value)
                            : Convert.ChangeType(entry.Value, prop.PropertyType);
                        prop.SetValue(question, value);
                    }
                }
            }
            return question;
        }
        /// <summary>
        /// 打乱list sources中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        public void ListRandom<T>(List<T> sources)
        {
            if (sources == null) sources = new List<T>();

            Random rd = new Random();
            T temp;
            for (int i = 0; i < sources.Count; i++)
            {
                int index = rd.Next(0, sources.Count - 1);
                if (index != i)
                {
                    temp = sources[i];
                    sources[i] = sources[index];
                    sources[index] = temp;
                }
            }
        }
    }
}
