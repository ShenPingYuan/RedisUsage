using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace RedisUsage
{
    public class FCCService
    {
        /// <summary>
        /// 分类信息保存
        /// </summary>
        /// <param name="pClassifyID">父分类ID</param>
        /// <param name="classifyID">分类ID</param>
        /// <param name="classifyName">分类名</param>
        /// <returns></returns>
        public string ClassifySave(string pClassifyID, string classifyID, string classifyName)
        {
            bool isNew = false;
            bool isFirstLevel = false;
            if (string.IsNullOrWhiteSpace(classifyID))
            {
                classifyID = Guid.NewGuid().ToString("n");
                isNew = true;
            }
            if (pClassifyID == null)
            {
                pClassifyID = "";
            }
            if (pClassifyID == "")
            {
                isFirstLevel = true;
            }
            List<HashEntry> entries = new List<HashEntry>();
            HashEntry entry = new HashEntry("ClassifyName", classifyName);
            entries.Add(entry);
            entry = new HashEntry("RState", true);
            entries.Add(entry);
            entry = new HashEntry("PClassifyID", pClassifyID);
            entries.Add(entry);
            RedisHelper.ContentSave(classifyID, entries.ToArray());
            if (isFirstLevel)
            {
                //CDic.AddOrUpdate(classifyID, classifyName, (m, n) => n);
                RedisHelper.ContentListAppend("FClassifyList", classifyID);
            }
            if (isNew)
            {
                RedisHelper.ContentListAppend("ClassifyList", classifyID);
                if (pClassifyID != "")
                {
                    RedisHelper.ContentListAppend("CClassifyList" + pClassifyID, classifyID);
                    RedisHelper.ContentListAppend("CClassifyList", classifyID);
                }
            }
            return classifyID;
            //return SuccessJsonData("ClassifySave", "0", "{\"Classify_ID\":\"" + classifyID + "\"}");
        }
    }
}
