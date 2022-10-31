using Balo.Data.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class PlannedTask : BaseMongoCollection
    {
  
        public string Label { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public string Notes { get; set; }
        public string Comments { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
    