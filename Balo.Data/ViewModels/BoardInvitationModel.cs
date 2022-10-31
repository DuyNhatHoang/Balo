using Balo.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Data.ViewModels
{
    public class BoardInvitationCreate
    {
        public Guid ReceiverId { get; set; }
        public Guid BoardId { get; set; }
    }

    public class BoardInvitationUpdate
    {
        public Guid Id { get; set; }
        public InvitationStatus Status { get; set; }
    }
}
