using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class CreateUserModel
    {
        public string FullName { get; set; }
    }

    public class UpdateUserModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}
