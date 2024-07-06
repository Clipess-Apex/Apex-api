namespace Clipess.DBClient.EntityModels
{
    public class TimeEntryNotification
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Message { get; set; }
        public int ReadBy { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
