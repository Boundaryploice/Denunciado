namespace DenunciaDo.Core.Application.DTOs
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public int ComplaintId { get; set; }
    }
}
