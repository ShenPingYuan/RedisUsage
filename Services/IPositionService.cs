using RedisUsage.Models.Positions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisUsage.Services
{
    public interface IPositionService
    {
        Guid PositionRootKey { get; }
        string PositionKeys { get; }
        Task<Position> GetPositionAsync(Guid positionId);
        Task AddPositionAsync(Position position);
        Task AddPositionForParentAsync(Guid parentPositionId, Position position);
        Task<List<Position>> GetPositionsForParentAsync(Guid positionId);
        Task UpdatePositionAsync(Position position);
        Task DeletePositionAsync(Position position);

        Task<TreeNode<Position>> GetPositionTreeAsync(Position root);
    }
}
