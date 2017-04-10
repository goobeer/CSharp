using Goobeer.Spider.ContentItem;
using System.Collections.Generic;

namespace Goobeer.Spider.Filter
{
    /// <summary>
    /// 筛选策略
    /// </summary>
    public interface IFilterStrategy
    {
        /// <summary>
        /// 筛选操作
        /// </summary>
        /// <param name="document">源内容</param>
        /// <returns>匹配到的数据集合</returns>
        List<string> DoFilter(WebDocument document);
    }
}
