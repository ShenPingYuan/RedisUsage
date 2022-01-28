using StackExchange.Redis;

namespace RedisUsage
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public static class RedisHelper
    {
        private static ConnectionMultiplexer rCnn;

        static RedisHelper()
        {
            string cnn = "94.191.83.150:6379,password=2439739932";
            rCnn = ConnectionMultiplexer.Connect(cnn);
        }

        #region 同步方法

        /// <summary>
        /// 删除Key下的内容
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static bool ClearKeyContent(string key, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.KeyDelete(key, flags);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 记录内容单字段保存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="when">操作条件</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static bool FieldContentSave(string key, string field, RedisValue value, int dbid = 0, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(field) && !value.IsNull)
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashSet(key, field, value, when, flags);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 记录内容整条保存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entries">对象</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static void ContentSave(string key, HashEntry[] entries, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && entries != null && entries.Length > 0)
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rDB.HashSet(key, entries, flags);
            }
        }

        /// <summary>
        /// 记录内容字段自增
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="value">自增值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentFieldInc(string key, string field, long value = 1, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashIncrement(key, field, value, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 记录内容字段自减
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="value">自减值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentFieldDec(string key, string field, long value = 1, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashDecrement(key, field, value, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取记录单字段值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static RedisValue FieldContentGet(string key, string field, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashGet(key, field, flags);
            }
            else
            {
                return default(RedisValue);
            }
        }

        /// <summary>
        /// 获取记录多字段值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fields">多字段</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static RedisValue[] FieldsContentGet(string key, RedisValue[] fields, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key) && fields != null && fields.Length > 0)
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashGet(key, fields, flags);
            }
            else
            {
                return new RedisValue[0];
            }
        }

        /// <summary>
        /// 获取整条记录内容
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static HashEntry[] ContentGet(string key, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashGetAll(key, flags);
            }
            else
            {
                return new HashEntry[0];
            }
        }

        /// <summary>
        /// 内容字段删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段名</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static bool ContentFieldDelete(string key, string field, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashDelete(key, field);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 内容多字段删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fields">字段</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentFieldsDelete(string key, RedisValue[] fields, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.HashDelete(key, fields);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 内容列表添加，放在最前
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="when">操作条件</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListAdd(string key, string value, int dbid = 0, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                long r = rDB.ListRemove(key, value);
                rDB.ListLeftPush(key, value, when, flags);
                return r;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 内容列表批量添加，放在最前
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListAddRange(string key, RedisValue[] values, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.ListLeftPush(key, values, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 内容列表批量保存(覆盖)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListSaveRange(string key, RedisValue[] values, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rDB.KeyDelete(key, flags);
                return rDB.ListRightPush(key, values, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 内容列表添加，放在最后
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="when">操作条件</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListAppend(string key, string value, int dbid = 0, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                long r = rDB.ListRemove(key, value);
                rDB.ListRightPush(key, value, when, flags);
                return r;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 内容列表批量添加，放在最后
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListAppendRange(string key, RedisValue[] values, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.ListRightPush(key, values, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 变换位置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="src">源值</param>
        /// <param name="dst">目标值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListChangePosition(string key, string src, string dst, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rDB.ListRemove(key, src, 0, flags);
                return rDB.ListInsertBefore(key, dst, src, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取内容列表
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="rCount">记录条数</param>
        /// <param name="pageindex">页面序数，从1开始</param>
        /// <param name="pagesize">页面容量</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static RedisValue[] ContentListGet(string key, out long rCount, int pageindex = 1, int pagesize = 10, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (pageindex < 1)
                {
                    pageindex = 1;
                }
                if (pagesize < 1)
                {
                    pagesize = 10;
                }
                int bIndex = (pageindex - 1) * pagesize;
                int eIndex = pageindex * pagesize - 1;
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rCount = rDB.ListLength(key);
                return rDB.ListRange(key, bIndex, eIndex, flags);
            }
            else
            {
                rCount = 0;
                return new RedisValue[0];
            }
        }
        /// <summary>
        /// 获取内容列表全部内容
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="rCount">记录条数</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <returns></returns>
        public static RedisValue[] ContentListGetAll(string key, out long rCount, int dbid = 0)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rCount = rDB.ListLength(key);
                return rDB.ListRange(key);
            }
            rCount = 0;
            return new RedisValue[0];
        }
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListCountGet(string key, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.ListLength(key, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除内容列表中值为“value”的元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="count">删除数量，当count>0时，从左向右删除count个，当count<0时，从右向左删除count个，当count=0时，删除所有的。</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long ContentListDelete(string key, string value, int dbid = 0, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                long r = rDB.ListRemove(key, value, count, flags);
                return r;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns>是否忽略，忽略表示已存在</returns>
        public static bool SetAdd(string key, string value, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                bool r = rDB.SetAdd(key, value, flags);
                return r;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取集合长度
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static long SetLength(string key, int dbid = 0, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                return rDB.SetLength(key, flags);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static bool SetContains(string key, string value, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                bool r = rDB.SetContains(key, value, flags);
                return r;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static bool SetDelete(string key, string value, int dbid = 0, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                bool r = rDB.SetRemove(key, value, flags);
                return r;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 批量添加有序集合
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entries">有序集合内容</param>
        /// <param name="when">操作条件</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="flags">命令标记</param>
        /// <returns>是否忽略，忽略表示已存在</returns>
        public static long SortedSetRangeAdd(string key, SortedSetEntry[] entries, int dbid = 0, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                IDatabase rDB = rCnn.GetDatabase(dbid);
                long r = rDB.SortedSetAdd(key, entries, when, flags);
                return r;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取排序集合列表
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="rCount">总记录条数</param>
        /// <param name="dbid">数据库ID，从0到15</param>
        /// <param name="order">顺序</param>
        /// <param name="pageIndex">页面序数，从1开始</param>
        /// <param name="pageSize">页面容量</param>
        /// <param name="start">最小值</param>
        /// <param name="stop">最大值</param>
        /// <param name="exclude">边界值规则</param>
        /// <param name="flags">命令标记</param>
        /// <returns></returns>
        public static RedisValue[] SortedSetGetList(string key, out long rCount, int dbid = 0, Order order = Order.Descending, int pageIndex = 1, int pageSize = 10, double start = double.MinValue, double stop = double.MaxValue, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.PreferSlave)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                if (pageSize < 1)
                {
                    pageSize = 10;
                }
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                IDatabase rDB = rCnn.GetDatabase(dbid);
                rCount = rDB.SortedSetLength(key, start, stop, exclude, flags);
                return rDB.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);

            }
            else
            {
                rCount = 0;
                return new RedisValue[0];
            }
        }

        /// <summary>
        /// 清除数据库
        /// </summary>
        public static void ClearDatabase()
        {
            rCnn.GetServer("192.168.2.3:6001").FlushAllDatabases();
        }
        #endregion

        #region 异步方法

        #endregion
    }
}
