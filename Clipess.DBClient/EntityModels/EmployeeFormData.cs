namespace Clipess.DBClient.EntityModels
{
    public class EmployeeFormData
    {
        public string OtherData { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? File { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }
    }
}
