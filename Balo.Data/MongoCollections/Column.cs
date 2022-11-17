using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.MongoCollections
{
    public class Column : BaseMongoCollection
    {
        public string Label { get; set; }
        public Guid BoardId { get; set; }
        public User Creater { get; set; }
    }
}
