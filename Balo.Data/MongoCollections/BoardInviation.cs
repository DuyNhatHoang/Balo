using Balo.Data.Enums;
using Balo.Data.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class BoardInviation : BaseMongoCollection
    {
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public Board Board { get; set; }
        public InvitationStatus Status { get; set; }
        public int ExprireIn { get; set; } = 10000;

    }
}
