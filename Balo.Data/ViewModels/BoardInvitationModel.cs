using Balo.Data.Enums;

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
