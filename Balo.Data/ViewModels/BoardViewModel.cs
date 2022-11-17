using Balo.Data.Enums;
using Microsoft.AspNetCore.Http;
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
        public string Name { get; set; }
        public BoardStatus Status { get; set; }
    }
}
