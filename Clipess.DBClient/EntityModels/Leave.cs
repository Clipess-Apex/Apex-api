

namespace Clipess.DBClient.EntityModels
{
    public class Leave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
       public Employee? employee { get; set; }
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }
        public DateTime CreatedDate {  get; set; }
        public DateTime LeaveDate { get; set; }
        public int StatusId { get; set; }
        public string Reason {  get; set; }
        public DateTime? ConsideredDate { get; set; }
    }
    public class UpdateLeaveStatusRequest
    {
        public int StatusId { get; set; }
    }
}
