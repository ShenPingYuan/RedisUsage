using System.Collections.Generic;

namespace RedisUsage.Models.Positions
{
    /// <summary>
    /// 数据结构-树（自由树/无序树）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T>
    {
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();
        public T Item { get; set; }
        /// <summary>
        /// 构造以item为跟节点的树
        /// </summary>
        /// <param name="item">根节点</param>
        public TreeNode(T item) => Item = item;
        /// <summary>
        /// 添加根据item添加子节点
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TreeNode<T> AddChild(T item)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }
        /// <summary>
        /// 添加子树
        /// </summary>
        /// <param name="childTreeNode"></param>
        public void AddChildTree(TreeNode<T> childTreeNode) => Children.Add(childTreeNode);
        /// <summary>
        /// 遍历子孙-后根遍历
        /// </summary>
        /// <param name="tree">树根</param>
        /// <returns></returns>
        public List<T> Travel()
        {
            List<T> treeNodesToReturn = new List<T>();
            foreach (TreeNode<T> treeNode in Children)
            {
                treeNodesToReturn.Add(treeNode.Item);
                treeNodesToReturn.AddRange(treeNode.Travel());
            }
            return treeNodesToReturn;
        }
    }
}
