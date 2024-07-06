

namespace Clipess.DBClient.EntityModels
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int MaxLeaveCount { get; set; }
        public int HideState { get; set; }
    }
    public class EditLeaveTypeRequest
    {
        public int? MaxLeaveCount { get; set; }
        public string? LeaveTypeName { get; set; }
    }
    public class UpdateHideStateRequest
    {
        public int HideState { get; set; }
    }
}
