using Goobeer.Spider.ContentItem;
using System;
using System.Collections.Generic;

namespace Goobeer.Spider.Filter
{
    public abstract class BaseFilter
    {
        public BloomFilter BF { get; set; }

        public Action<WebDocument, List<string>> ShowFilterData { get; set; }
    }
}
