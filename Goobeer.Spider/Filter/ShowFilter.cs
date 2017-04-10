using Goobeer.Spider.ContentItem;
using System.Collections.Generic;

namespace Goobeer.Spider.Filter
{
    public class ShowFilter : BaseFilter, IFilterStrategy
    {
        public List<string> DoFilter(WebDocument document)
        {
            if (ShowFilterData!=null)
            {
                ShowFilterData(document, null);
            }
            return null;
        }
    }
}
