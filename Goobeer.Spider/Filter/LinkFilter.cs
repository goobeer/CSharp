using Goobeer.Spider.ContentItem;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Goobeer.Spider.Filter
{
    /// <summary>
    /// 链接过滤器
    /// </summary>
    public class LinkFilter:BaseFilter, IFilterStrategy
    {
        public List<string> DoFilter(WebDocument document)
        {
            BF.Add(document.UrlAddress.ToString());

            string content = document.Html.ToString();
            string urlAddress = document.UrlAddress.AbsoluteUri;

            List<string> list = new List<string>();
            //识别 uri 中的 陷阱
            Regex reg = new Regex("<a.*?href=['\"](?<link>([^#|javascript:|mailto:|\'|\"].*?[^\'|\"]*))(#.*?)?/?['\"]?.*?>(?<name>(.*?))</a>", RegexOptions.IgnoreCase|RegexOptions.Multiline|RegexOptions.Singleline);
            
            MatchCollection mc = reg.Matches(content);
            if (mc.Count <= 0)
            {
                //匹配meta中的url
                Regex regMeta = new Regex("<meta.*?http-equiv=[\"']refresh[\"'].*?content=.*?url=[\"'](?<link>(.*?))[\"'].*?/>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
                mc = regMeta.Matches(content);
                if (mc.Count <= 0)
                {
                    Regex regJSUri = new Regex("<meta.*?http-equiv=[\"']refresh[\"'].*?content=.*?url=[\"'](?<link>(.*?))[\"'].*?/>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
                    mc = regJSUri.Matches(content);
                }
            }
            string link = string.Empty;
            foreach (Match item in mc)
            {
                link = item.Groups["link"].Value;
                if (link.IndexOf('#') >= 0)
                {
                    link = link.Substring(0, link.IndexOf('#'));
                }
                if (string.IsNullOrEmpty(link))
                {
                    continue;
                }
                
                if (!(link.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || link.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                {
                    if (urlAddress.IndexOf(link) > 0)
                    {
                        continue;
                    }

                    if (link.StartsWith("//"))
                    {
                        link = string.Format("{1}:{0}", link, document.UrlAddress.Scheme);
                    }
                    else if (link.StartsWith("/"))
                    {
                        link = string.Format("{0}://{1}{2}", document.UrlAddress.Scheme, document.UrlAddress.Host, link);
                    }
                    else
                    {
                        link = string.Format("{0}/{1}", document.UrlAddress.AbsoluteUri, link);
                    }
                }

                if (!BF.Add(link))//(没有 爬取过)
                {
                    list.Add(string.Format("{0}\t{1}", link, item.Groups["name"].Value));
                }                
            }

            if (ShowFilterData != null)
            {
                ShowFilterData(document, list);
            }
            return list;
        }
    }
}
