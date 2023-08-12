namespace ExcelCreator.Web.Entities
{
    // olusturulan excel dosyalari api uzerinden buraya kayit edilecek
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string? FilePath { get; set; }
        // worker servisin olusturdugu tarih
        public DateTime? CreatedDate { get; set; }
        public FileStatus FileStatus { get; set; }

        public string GetCreateDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";

    }

    public enum FileStatus
    {
        Creating = 1,
        Completed = 2,
    }
}
