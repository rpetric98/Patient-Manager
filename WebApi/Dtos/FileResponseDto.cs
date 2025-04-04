namespace WebApi.Dtos
{
    public class FileResponseDto
    {
        public string ObjectId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
    }
}
