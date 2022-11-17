using Balo.Data.MongoCollections;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class AppFile : BaseMongoCollection
    {
        public IFormFile file { get; set; }


    }
}
