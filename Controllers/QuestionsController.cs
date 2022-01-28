using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisUsage.Models.Questions;
using RedisUsage.Parameters;
using RedisUsage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace FiremanCloudClass.Controllers
{
    /// <summary>
    /// 题库资源
    /// </summary>
    [Route("api/Questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        /// <summary>
        /// 获取多个题目
        /// </summary>
        /// <param name="parameters">查询参数</param>
        /// <response code="200">200状态码则返回多个题目,并通过Header中的X-Pagination返回分页信息</response>
        [HttpGet(Name =nameof(GetQuestionsAsync))]
        [ProducesResponseType(typeof(List<QuestionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<QuestionDto>>> GetQuestionsAsync([FromQuery] QuestionDtoParameters parameters)
        {
            //查到数据
            var questions = await _questionService.GetQuestionsAsync(parameters);
            //映射
            var dtosToReturn = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Content = q.Content,
                KnowledgeCategoryName = q.KnowledgeCategoryName,
                QuestionType = q.QuestionType,
                QuestionAnalysis = q.QuestionAnalysis,
                StripLineCategoryId = q.StripLineCategoryId,
                PositionCategoryIds = q.PositionCategoryIds,
                Options = q.Options.Select(o => new OptionDto { IsRight = o.IsRight, OptionKey = o.OptionKey, OptionValue = o.OptionValue }).ToList()
            }).ToList();
            //分页信息
            var paginationMetadata = new
            {
                totalCount = questions.TotalCount,
                pageSize = questions.PageSize,
                currentPage = questions.CurrentPage,
                totalPages = questions.TotalPages
            };
            //通过Header：X-Pagination返回分页元数据
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata,
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            return Ok(dtosToReturn);
        }
        /// <summary>
        /// 获取单个题目的详细信息
        /// </summary>
        /// <param name="questionId">题目的Id（Guid）</param>
        /// <returns></returns>
        /// <response code="404">如果传入的questionId对应的资源没有找到，则返回404</response>
        /// <response code="200">200状态码则返回对应的资源</response>
        [HttpGet("{questionId}", Name = nameof(GetQuestionAsync))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuestionDto>> GetQuestionAsync([FromRoute] Guid questionId)
        {
            var questionEntity = await _questionService.GetQuestionAsync(questionId);
            if (questionEntity == null) return NotFound($"没找到题目（QuestionId：{questionId}）");
            var dtoToReturn = new QuestionDto
            {
                Id = questionEntity.Id,
                Content = questionEntity.Content,
                KnowledgeCategoryName = questionEntity.KnowledgeCategoryName,
                PositionCategoryIds = questionEntity.PositionCategoryIds,
                QuestionAnalysis = questionEntity.QuestionAnalysis,
                QuestionType = questionEntity.QuestionType,
                StripLineCategoryId = questionEntity.StripLineCategoryId,
                Options = questionEntity.Options.Select(o => new OptionDto { IsRight = o.IsRight, OptionKey = o.OptionKey, OptionValue = o.OptionValue }).ToList()
            };
            return Ok(dtoToReturn);
        }
        /// <summary>
        /// 批量创建题目
        /// </summary>
        /// <param name="dtos">上传的题目信息</param>
        /// <returns>资源创建成功则返回201，同时返回创建的资源，</returns>
        /// <response code="400">上传的题目信息不符合要求则返回400,同时携带不符合要求的原因</response>
        /// <response code="201">资源创建成功则返回201,并返回创建的资源</response>
        [HttpPost]
        [ProducesResponseType(typeof(QuestionDto),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] List<QuestionAddDto> dtos)
        {
            var timeNow = DateTime.Now.ToString();
            List<QuestionDto> dtosToReturn = new List<QuestionDto>();
            //初始化一个Question Entity
            foreach (var dto in dtos)
            {
                //循环创建单个题目，不推荐这么做
                var questionEntity = new Question
                {
                    Content = dto.Content,
                    CreateTime = timeNow,
                    Creator = "Creator",
                    KnowledgeCategoryName = dto.KnowledgeCategoryName,
                    PositionCategoryIds = dto.PositionCategoryIds,
                    QuestionAnalysis = dto.QuestionAnalysis,
                    QuestionType = dto.QuestionType,
                    UpdateTime = timeNow,
                    StripLineCategoryId = dto.StripLineCategoryId,
                    Options = dto.Options.Select(o => new Option
                    {
                        IsRight = o.IsRight,
                        OptionKey = o.OptionKey,
                        OptionValue = o.OptionValue,
                    }).ToList(),
                };
                //异步添加一个Question
                await _questionService.AddQuestionAsync(questionEntity);
                var dtoToReturn = new QuestionDto
                {
                    Id = questionEntity.Id,
                    Content = questionEntity.Content,
                    KnowledgeCategoryName = questionEntity.KnowledgeCategoryName,
                    PositionCategoryIds = questionEntity.PositionCategoryIds,
                    QuestionAnalysis = questionEntity.QuestionAnalysis,
                    StripLineCategoryId = questionEntity.StripLineCategoryId,
                    QuestionType = questionEntity.QuestionType,
                    Options = questionEntity.Options.Select(o => new OptionDto { IsRight = o.IsRight, OptionKey = o.OptionKey, OptionValue = o.OptionValue }).ToList()
                };
                dtosToReturn.Add(dtoToReturn);
            }
            return CreatedAtRoute(nameof(GetQuestionsAsync), dtosToReturn);
        }
        /// <summary>
        /// 编辑一个题目
        /// </summary>
        /// <param name="questionId">题目的Id（Guid）</param>
        /// <param name="dto">修改的内容</param>
        /// <returns></returns>
        /// <response code="204">更新成功返回204（无内容）</response>
        /// <response code="404">如果传入的questionId对应的资源没有找到，则返回404</response>
        [HttpPut("{questionId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateQuestionAsync([FromRoute] Guid questionId, [FromBody]QuestionUpdateDto dto)
        {
            //验证一下上传的Id
            if (questionId != dto.Id) return BadRequest($"{nameof(questionId)}:{questionId},dto.Id:{dto.Id}");
            //查找要修改的Question
            var questionEntity = await _questionService.GetQuestionAsync(questionId);
            //没找到对应的Question
            if (questionEntity == null) return NotFound($"没找到题目（QuestionId：{questionId}）");
            //更新内容
            questionEntity.Content = dto.Content;
            questionEntity.KnowledgeCategoryName = dto.KnowledgeCategoryName;
            questionEntity.PositionCategoryIds = dto.PositionCategoryIds;
            questionEntity.QuestionAnalysis = dto.QuestionAnalysis;
            questionEntity.QuestionType = dto.QuestionType;
            questionEntity.StripLineCategoryId = dto.StripLineCategoryId;
            questionEntity.UpdateTime = DateTime.Now.ToString();
            questionEntity.Options = dto.Options.Select(o => new Option { IsRight = o.IsRight, OptionKey = o.OptionKey, OptionValue = o.OptionValue, QuestionId = questionId }).ToList();
            await _questionService.UpdateQuestionAsync(questionEntity);
            return NoContent();
        }
        /// <summary>
        /// 删除一个题目
        /// </summary>
        /// <param name="questionId">题目的Id（Guid）</param>
        /// <returns></returns>
        /// <response code="204">删除成功返回204（无内容）</response>
        [HttpDelete("{questionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteQuestionAsync([FromRoute] Guid questionId)
        {
            var questionEntity = await _questionService.GetQuestionAsync(questionId);
            if (questionEntity == null) return NotFound($"没找到题目（QuestionId：{questionId}）");

            await _questionService.DeleteQuestionAsync(questionEntity);
            return NoContent();
        }
    }
}
