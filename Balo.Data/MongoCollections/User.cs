using Balo.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.MongoCollections
{
    public class User : BaseMongoCollection
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Role Role { get; set; }
    }
}
