using Goobeer.Spider.ContentItem;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Goobeer.Spider.Filter
{
    /// <summary>
    /// 图片筛选
    /// </summary>
    public class ImgFilter: BaseFilter, IFilterStrategy
    {
        public List<string> DoFilter(WebDocument document)
        {
            string urlAddress = document.UrlAddress.AbsoluteUri;
            string content=document.Html.ToString();
            List<string> list = new List<string>();
            Regex reg = new Regex("<img.*?src=['\"](.*?)['\"]", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(content);
            foreach (Match item in mc)
            {
                list.Add(item.Groups[1].Value);
            }

            if (ShowFilterData != null)
            {
                Task.Factory.StartNew(() => {
                    ShowFilterData(document, list);
                });
            }
            return list;
        }
    }
}
