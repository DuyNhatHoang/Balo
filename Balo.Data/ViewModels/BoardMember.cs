using Balo.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class BoardMember
    {
        public string UserName{ get; set; }
        public Guid UserId{ get; set; }
        public BoardRole Role{ get; set; }
    }
}
