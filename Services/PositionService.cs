using Newtonsoft.Json;
using RedisUsage.Models.Positions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Services
{
    public class PositionService : IPositionService
    {
        /// <summary>
        /// 岗位分类相关的使用db 3
        /// </summary>
        private readonly int dbid = 3;
        /// <summary>
        /// 题目的Id列表
        /// </summary>
        private readonly Guid _positionRootKey = Guid.Parse("a9d9f105-52c9-4586-ba01-c8ffe65c1f6b");
        private readonly string _positionKeys = "positionKeys";

        public Guid PositionRootKey => _positionRootKey;
        public string PositionKeys => _positionKeys;

        public PositionService()
        {
            Position root = GetPositionAsync(PositionRootKey).GetAwaiter().GetResult();
            if (root == null)
            {
                root = new Position
                {
                    Id = PositionRootKey,
                    Name = "Root",
                    ParentId = Guid.Empty,
                };
                AddPositionAsync(root).Wait();
            }
        }
        /// <summary>
        /// 获取一个Position entity
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public async Task<Position> GetPositionAsync(Guid positionId)
        {
            if (positionId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(positionId));
            }

            return await Task.Run(() =>
            {
                HashEntry[] fields = RedisHelper.ContentGet(positionId.ToString(), dbid);
                return HashEntriesToPosition(fields);
            });
        }

        public async Task AddPositionAsync(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            List<HashEntry> fields = PositionToHashEntryList(position);
            await Task.Run(() =>
            {
                RedisHelper.ContentSave(position.Id.ToString(), fields.ToArray(), dbid);
                //RedisHelper.ContentListAdd(PositionKeys, position.Id.ToString(), dbid);
            });
            position = await GetPositionAsync(position.Id);
        }
        /// <summary>
        /// 在父Position下添加一个子Position
        /// </summary>
        /// <param name="parentPositionId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task AddPositionForParentAsync(Guid parentPositionId, Position position)
        {
            if (parentPositionId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(parentPositionId));
            }

            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            Position positionParent = await GetPositionAsync(parentPositionId);
            if (positionParent == null) throw new KeyNotFoundException($"entity not found:{nameof(parentPositionId)}:{parentPositionId}");

            position.Id = Guid.NewGuid();
            position.ParentId = positionParent.Id;

            positionParent.Children.Add(position.Id);

            List<HashEntry> childFields = PositionToHashEntryList(position);
            List<HashEntry> parentFields = PositionToHashEntryList(positionParent);
            await Task.Run(() =>
            {
                RedisHelper.ContentSave(position.Id.ToString(), childFields.ToArray(), dbid);
                RedisHelper.ContentListAdd(PositionKeys, position.Id.ToString(), dbid);
                RedisHelper.ContentSave(positionParent.Id.ToString(), parentFields.ToArray(), dbid);
            });
            position = await GetPositionAsync(position.Id);
        }
        /// <summary>
        /// 获取父Position下的子Positions
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public async Task<List<Position>> GetPositionsForParentAsync(Guid positionId)
        {
            if (positionId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(positionId));
            }

            Position positionParent = await GetPositionAsync(positionId);
            if (positionParent == null) throw new KeyNotFoundException($"entity not found:{nameof(positionId)}:{positionId}");

            List<Position> children = new List<Position>();

            foreach (Guid childId in positionParent.Children)
            {
                Position childPosition = await GetPositionAsync(childId);
                if (childPosition != null)
                {
                    children.Add(childPosition);
                }
            }
            return children;
        }
        /// <summary>
        /// 更新一个Position entity
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task UpdatePositionAsync(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            if (position.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(position.Id));
            }

            Position positionEntity = await GetPositionAsync(position.Id);
            if (positionEntity == null)
            {
                throw new KeyNotFoundException($"entity not found:{nameof(position.Id)}:{position.Id}");
            }

            await Task.Run(() =>
            {
                List<HashEntry> fields = PositionToHashEntryList(position);
                RedisHelper.ContentSave(positionEntity.Id.ToString(), fields.ToArray(), dbid);
            });
        }
        /// <summary>
        /// 删除一个Position entity（级联删除，谨慎调用）
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task DeletePositionAsync(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            await Task.Run(async () =>
            {
                TreeNode<Position> positionTree = await GetPositionTreeAsync(position);
                List<Position> children = positionTree.Travel();
                foreach (Position child in children)
                {
                    RedisHelper.ClearKeyContent(child.Id.ToString(), dbid);
                }
                RedisHelper.ClearKeyContent(position.Id.ToString(), dbid);
            });
            Position parent = await GetPositionAsync(position.Id);
            if(parent!=null)
            {
                parent.Children.Remove(position.Id);
                await UpdatePositionAsync(parent);
            }
        }
        /// <summary>
        /// 把Position Entity转换成HashEntry List
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<HashEntry> PositionToHashEntryList(Position position)
        {
            var props = position.GetType().GetProperties();
            List<HashEntry> entries = new List<HashEntry>();
            foreach (var prop in props)
            {
                RedisValue value = prop.Name == nameof(Position.Children)
                    ? (RedisValue)JsonConvert.SerializeObject(prop.GetValue(position))
                    : prop.GetValue(position)?.ToString();
                entries.Add(new HashEntry(prop.Name, value));
            }
            return entries;
        }
        /// <summary>
        /// 把HashEntry数组转换成Position
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Position HashEntriesToPosition(HashEntry[] entries)
        {
            if (entries.Count() == 0) return null;
            Position position = new Position();
            var props = position.GetType().GetProperties();
            foreach (var entry in entries)
            {
                var prop = props.FirstOrDefault(p => p.Name.Equals(entry.Name.ToString(), StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    if (nameof(Position.Children).Equals(prop.Name.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var value = JsonConvert.DeserializeObject<List<Guid>>(entry.Value);
                        prop.SetValue(position, value);
                    }
                    else
                    {
                        object value;
                        value = (prop.PropertyType == typeof(Guid))
                            ? Guid.Parse(entry.Value)
                            : Convert.ChangeType(entry.Value, prop.PropertyType);
                        prop.SetValue(position, value);
                    }
                }
            }
            return position;
        }
        /// <summary>
        /// 获取一颗以root为根节点的数
        /// </summary>
        /// <param name="rootId">树根</param>
        /// <returns></returns>
        public async Task<TreeNode<Position>> GetPositionTreeAsync(Position root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            TreeNode<Position> rootTree = new TreeNode<Position>(root);
            foreach (Guid childId in root.Children)
            {
                Position child = await GetPositionAsync(childId);
                if (child == null)
                {
                    throw new KeyNotFoundException($"entity not found:{nameof(childId)}:{childId}");
                }
                TreeNode<Position> childTree = await GetPositionTreeAsync(child);
                rootTree.AddChildTree(childTree);
            }
            return rootTree;
        }
    }
}
