using Balo.Data.Enums;
using Balo.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.MongoCollections
{
    public class Board : BaseMongoCollection
    {        
        public string Name { get; set; }
        public ICollection<User> Members { get; set; } = new List<User>(); 

         // public ICollection<Group> Groups { get; set; } = new List<Group>();
        public BoardStatus Status{ get; set; }
    }
}
