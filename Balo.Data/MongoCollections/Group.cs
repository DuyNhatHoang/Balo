using Balo.Data.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class Group : BaseMongoCollection
    {
        //  public ICollection<PlannedTask> Tasks { get; set; } = new List<PlannedTask>();
        public string Label { get; set; }
        public User Creater { get; set; }
        public Guid BoardId { get; set; }

    }
}
