using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class CreateGroupModel
    {
        public Guid BoardId { get; set; }
        public string Label  { get; set; }


    }

    public class GetGroupModel
    {
        public Guid? BoardId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? CreateId { get; set; }
        public string? Label { get; set; }


    }
}
