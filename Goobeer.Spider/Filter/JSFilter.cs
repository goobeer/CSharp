using Goobeer.Spider.ContentItem;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Goobeer.Spider.Filter
{
    public class JSFilter: BaseFilter, IFilterStrategy
    {
        public List<string> DoFilter(WebDocument document)
        {
            string urlAddress = document.UrlAddress.AbsoluteUri;
            string content = document.Html.ToString();
            List<string> list = new List<string>();
            Regex reg = new Regex("<script.*?( type=['\"].*?javascript['\"] )?src=['\"](.*?)['\"]( type=['\"].*?javascript['\"] )?.*?>", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(content);
            foreach (Match item in mc)
            {
                list.Add(item.Groups[2].Value);
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
