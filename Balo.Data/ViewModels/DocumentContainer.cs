using Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Balo.Data.ViewModels
{
  
    public class DocumentContainer
    {
    }

    public class Document : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public long FileSize { get; set; }

        public string MimeType { get; set; }

        public Guid? TenantId { get; set; } = new Guid();

        protected Document()
        {
        }

        public Document(
            Guid id,
            long fileSize,
            string mimeType
        )
        {
            FileSize = fileSize;
            MimeType = mimeType;
        }
    }
}
