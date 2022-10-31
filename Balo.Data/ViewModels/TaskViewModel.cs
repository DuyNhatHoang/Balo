using Balo.Data.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class CreateTaskModel
    {
        public string? Label { get; set; }
        public string? Notes { get; set; }
        public string? Comments { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        public ICollection<Guid> Members { get; set; } = new List<Guid>();
        public ICollection<Guid> Groups { get; set; } = new List<Guid>();
    }

    public class GetTaskModel
    {
        public Guid GroupId  { get; set; }
        public Guid MemberId  { get; set; }
        public int? Priority  { get; set; }
    }
}
