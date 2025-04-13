using shared.Enums;

namespace shared.Entities
{
    public class FileUpload
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }
        public StatusEnum Status { get; set; }
    }
}
