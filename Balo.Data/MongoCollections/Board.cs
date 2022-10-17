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
        public List<BoardMember> Members { get; set; }
        public BoardStatus Status{ get; set; }  
    }
}
