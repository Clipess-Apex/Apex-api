namespace Clipess.DBClient.EntityModels
{
    public class MonthlyWorkingDay
    {
        public int Id { get; set; }
        public string Month {  get; set; }
        public DateTime Date { get; set; }
        public string DateType { get; set; }
        public string? Description { get; set; }
    }
}
