using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class BoardViewModel
    {
        public string Name { get; set; }
    }

    public class BoardCreateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<BoardMember> Members { get; set; }
    }
}
