using Microsoft.AspNetCore.Mvc;
using RedisUsage.Models.Positions;
using RedisUsage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Controllers
{
    [Route("api/positions")]
    [ApiController]
    public class PositionsController:ControllerBase
    {
        private readonly IPositionService _positionService;
        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        }
        /// <summary>
        /// 获取根岗位树
        /// </summary>
        /// <returns></returns>
        [HttpGet("tree")]
        public async Task<IActionResult> GetPositionTreeForRootAsync()
        {
            var position =await _positionService.GetPositionAsync(_positionService.PositionRootKey);
            if (position == null) return NotFound();

            var tree = await _positionService.GetPositionTreeAsync(position);

            return Ok(tree);
        }
        /// <summary>
        /// 获取某个岗位下的岗位数树
        /// </summary>
        /// <returns></returns>
        [HttpGet("tree/{positionId}")]
        public async Task<IActionResult> GetPositionTreeForPositionAsync([FromRoute] Guid positionId)
        {
            var position = await _positionService.GetPositionAsync(positionId);
            if (position == null) return NotFound();

            var tree = await _positionService.GetPositionTreeAsync(position);

            return Ok(tree);
        }
        /// <summary>
        /// 获取一个岗位下的所有子岗位
        /// </summary>
        /// <param name="positionId">父岗位Id</param>
        /// <returns></returns>
        [HttpGet("{positionId}/positions")]
        public async Task<ActionResult<List<PositionDto>>> GetPositionsForParentAsync([FromRoute] Guid positionId)
        {
            var position =await _positionService.GetPositionAsync(positionId);
            if(position==null) return NotFound($"{nameof(positionId)}:{positionId}");

            var positions = await _positionService.GetPositionsForParentAsync(positionId);
            var dtosToReturn = positions.Select(p => new PositionDto { Id = p.Id, Name = p.Name, Children = p.Children });

            return Ok(dtosToReturn);
        }
        /// <summary>
        /// 获取一个岗位
        /// </summary>
        /// <param name="positionId">岗位Id</param>
        /// <returns></returns>
        [HttpGet("{positionId}",Name =nameof(GetPositionAsync))]
        public async Task<ActionResult<PositionDto>> GetPositionAsync([FromRoute] Guid positionId)
        {
            var position =await _positionService.GetPositionAsync(positionId);
            if(position==null) return NotFound($"{nameof(positionId)}:{positionId}");
            PositionDto dtoToReturn = new PositionDto
            {
                Id = position.Id,
                Name = position.Name,
                Children = position.Children,
            };
            return Ok(dtoToReturn);
        }
        /// <summary>
        /// 在一个岗位下创建一个子岗位
        /// </summary>
        /// <param name="positionId">父岗位Id</param>
        /// <param name="dto">岗位的基本信息</param>
        /// <returns></returns>
        [HttpPost("{positionId}/positions")]
        public async Task<IActionResult> AddPositionAsync([FromRoute] Guid positionId, PositionAddDto dto)
        {
            var position = await _positionService.GetPositionAsync(positionId);
            if (position == null) NotFound($"{nameof(positionId)}:{positionId}");
            Position childPosition = new Position
            {
                Name = dto.Name
            };
            await _positionService.AddPositionForParentAsync(positionId, childPosition);
            var dtoToReturn = new PositionDto { Name = childPosition.Name, Id = childPosition.Id, Children = childPosition.Children };
            return CreatedAtRoute(nameof(GetPositionAsync),new { positionId=dtoToReturn.Id},dtoToReturn);
        }
        /// <summary>
        /// 编辑一个岗位
        /// </summary>
        /// <param name="positionId">岗位Id</param>
        /// <param name="dto">岗位更改的岗位信息</param>
        /// <returns></returns>
        [HttpPut("{positionId}")]
        public async Task<IActionResult> UpdatePositionAsync([FromRoute] Guid positionId, [FromBody] PositionUpdateDto dto)
        {
            var position = await _positionService.GetPositionAsync(positionId);
            if (position == null) NotFound($"{nameof(positionId)}:{positionId}");
            position.Name = dto.Name;
            await _positionService.UpdatePositionAsync(position);
            return NoContent();
        }
        /// <summary>
        /// 删除一个岗位（级联删除-慎重）
        /// </summary>
        /// <param name="positionId">岗位Id</param>
        /// <returns></returns>
        [HttpDelete("{positionId}")]
        public async Task<IActionResult> DeletePositionAsync([FromRoute] Guid positionId)
        {
            var position =await _positionService.GetPositionAsync(positionId);
            if(position==null) return NotFound($"{nameof(positionId)}:{positionId}");

            await _positionService.DeletePositionAsync(position);
            return NoContent();
        }
    }
}
