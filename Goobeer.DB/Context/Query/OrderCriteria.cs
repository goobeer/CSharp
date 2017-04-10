using System.Collections.Generic;

namespace Goobeer.DB.Context.Query
{
    public class OrderCriteria
    {
        public IDictionary<string, OrderMode> Orders { get; set; }

        public OrderCriteria(string name, OrderMode mode)
        {
            Orders = new Dictionary<string, OrderMode>();
            AddOrderCriteria(name, mode);
        }

        public void AddOrderCriteria(string name, OrderMode mode)
        {
            Orders.Add(name, mode);
        }
    }

    public enum OrderMode
    {
        ASC, DESC
    }
}
