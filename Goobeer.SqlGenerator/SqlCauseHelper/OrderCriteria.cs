using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.DB.SqlCauseHelper
{
    public class OrderCriteria
    {
        public IDictionary<string,OrderMode> Orders { get; set; }

        public OrderCriteria(string name,OrderMode mode)
        {
            Orders = new Dictionary<string, OrderMode>();
            Orders.Add(name, mode);
        }
    }

    public enum OrderMode
    {
        ASC,DESC
    }
}
