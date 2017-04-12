using Goobeer.DataAttributeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.Entity
{
    public class OrderBase:EntityBase
    {
        [Field]
        public int Ord { get; set; }
    }
}
