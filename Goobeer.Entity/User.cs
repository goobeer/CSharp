using Goobeer.DB.DataAttributeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goobeer.Entity
{
    [Table(TableName ="[User]")]
    public class User
    {
        [Field(AutoCreate = true, IsPK = true)]
        public int ID { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public bool State { get; set; }

        public int UGID { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime Birthday { get; set; }
    }
}
